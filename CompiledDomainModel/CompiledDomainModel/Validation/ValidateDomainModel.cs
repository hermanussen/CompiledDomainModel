/*
    CompiledDomainModel Sitecore module
    Copyright (C) 2010-2011  Robin Hermanussen

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CompiledDomainModel.Utils;
using Sitecore.Data.Items;
using CompiledDomainModel.Attributes;
using System.Web.Caching;
using Sitecore.Diagnostics;
using Sitecore.Data.Fields;
using Sitecore.Data;
using Sitecore.SecurityModel;

namespace CompiledDomainModel.Validation
{
    public class ValidateDomainModel : IValidateDomainModelProcessor
    {
        public const string validationResultCacheKey = "ValidateDomainModelResult";

        public void Process(ValidateDomainModelPipelineArgs args)
        {
            using (new SecurityDisabler())
            {
                // set an application wide value indicating validation errors (can be used to check validation result later)
                if (HttpContext.Current.Cache[validationResultCacheKey] == null)
                {
                    HttpContext.Current.Cache.Add(validationResultCacheKey, bool.TrueString, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(0), CacheItemPriority.AboveNormal, null);
                }
                else
                {
                    HttpContext.Current.Cache[validationResultCacheKey] = bool.TrueString;
                }

                CheckModuleVersion(args.Errors, args.Warnings);

                IDictionary<DomainObjectAttribute, List<DomainObjectFieldAttribute>> attributes = new Dictionary<DomainObjectAttribute, List<DomainObjectFieldAttribute>>();

                CheckDataModel(args.Errors, args.Warnings, attributes);

                CheckFixedPaths(args.Errors, args.Warnings, attributes);

                if (args.Errors.Count > 0)
                {
                    HttpContext.Current.Cache[validationResultCacheKey] = bool.FalseString;
                }
            }
        }

        private static void CheckFixedPaths(List<string> errors, List<string> warnings, IDictionary<DomainObjectAttribute, List<DomainObjectFieldAttribute>> attributesMapping)
        {
            var typesWithMyAttribute =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in ConfigurationUtil.ExecuteIgnoreExceptions<Type[]>(() => a.GetTypes()) ?? new Type[0]
                let attributes = ConfigurationUtil.ExecuteIgnoreExceptions<object[]>(() => t.GetCustomAttributes(typeof(FixedPathAttribute), true)) ?? new object[0]
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<FixedPathAttribute>() };
            foreach (var typeWithMyAttribute in typesWithMyAttribute)
            {
                foreach (FixedPathAttribute attribute in typeWithMyAttribute.Attributes)
                {
                    attribute.Validate(errors, warnings);
                }
            }
        }

        private static void CheckDataModel(List<string> errors, List<string> warnings, IDictionary<DomainObjectAttribute, List<DomainObjectFieldAttribute>> attributesMapping)
        {
            // Check domain model mappings with sitecore database(s)
            var typesWithMyAttribute =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in ConfigurationUtil.ExecuteIgnoreExceptions<Type[]>(() => a.GetTypes()) ?? new Type[0]
                let attributes = ConfigurationUtil.ExecuteIgnoreExceptions<object[]>(() => t.GetCustomAttributes(typeof(DomainObjectAttribute), true)) ?? new object[0]
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<DomainObjectAttribute>() };

            foreach (var typeWithMyAttribute in typesWithMyAttribute)
            {
                foreach (DomainObjectAttribute attribute in typeWithMyAttribute.Attributes)
                {
                    attributesMapping.Add(attribute, new List<DomainObjectFieldAttribute>());

                    IEnumerable<string> messages = attribute.Validate(typeWithMyAttribute.Type, attributesMapping[attribute]);
                    if (messages != null)
                    {
                        errors.AddRange(messages);
                    }
                }
            }

            IEnumerable<string> fixedPathsConfiguredInCode =
                (from a in AppDomain.CurrentDomain.GetAssemblies()
                 from t in ConfigurationUtil.ExecuteIgnoreExceptions<Type[]>(() => a.GetTypes()) ?? new Type[0]
                 let attributes = ConfigurationUtil.ExecuteIgnoreExceptions<object[]>(() => t.GetCustomAttributes(typeof(FixedPathAttribute), true)) ?? new object[0]
                where attributes != null && attributes.Length > 0
                select attributes.Cast<FixedPathAttribute>().Select(attr => attr.FullPath)).SelectMany(attrs => attrs);

            // Check sitecore databases with the domain model (non-critical)
            if (warnings != null)
            {
                Item settingsItem = null;
                try
                {
                    settingsItem = ConfigurationUtil.SettingsItem;
                }
                catch (Exception exc)
                {
                    warnings.Add(string.Format("The CompiledDomainModel configuration could not be found. You should reinstall and configure the CompiledDomainModel. Exception message: {0}", exc.Message));
                }
                if (settingsItem != null)
                {
                    List<TemplateItem> allTemplateItems = new List<TemplateItem>();

                    foreach (Item childItem in settingsItem.GetChildren())
                    {
                        if ("DomainObjectSet".Equals(childItem.TemplateName))
                        {
                            Assert.IsNotNull(childItem.Fields["Templates to map to Domain Model"], "The 'Templates to map to Domain Model' field could not be found.");
                            MultilistField templatesField = FieldTypeManager.GetField(childItem.Fields["Templates to map to Domain Model"]) as MultilistField;
                            Assert.IsNotNull(templatesField, "The 'Templates to map to Domain Model' field could not be found or is not a MultilistField.");
                            Item[] templateItems = templatesField.GetItems();
                            if (templateItems != null)
                            {
                                allTemplateItems.AddRange(templateItems.Select(templateItem => new TemplateItem(templateItem)));
                            }

                            Assert.IsNotNull(childItem.Fields["Templates that contribute to Domain Model"], "The 'Templates that contribute to Domain Model' field could not be found.");
                            MultilistField contribTemplatesField = FieldTypeManager.GetField(childItem.Fields["Templates that contribute to Domain Model"]) as MultilistField;
                            Assert.IsNotNull(contribTemplatesField, "The 'Templates that contribute to Domain Model' field could not be found or is not a MultilistField.");
                            Item[] contribTemplateItems = contribTemplatesField.GetItems();
                            if (contribTemplateItems != null)
                            {
                                allTemplateItems.AddRange(contribTemplateItems.Select(templateItem => new TemplateItem(templateItem)));
                            }
                        }
                        else if ("FixedPathSet".Equals(childItem.TemplateName))
                        {
                            Assert.IsNotNull(childItem.Fields["Fixed tree locations"], "The 'Fixed tree locations' field could not be found.");
                            MultilistField fixedTreeLocationsField = FieldTypeManager.GetField(childItem.Fields["Fixed tree locations"]) as MultilistField;
                            Assert.IsNotNull(fixedTreeLocationsField, "The 'Fixed tree locations' field could not be found or is not a MultilistField.");
                            Item[] fixedTreeLocationItems = fixedTreeLocationsField.GetItems();
                            if (fixedTreeLocationItems != null)
                            {
                                foreach (string notFoundPath in fixedTreeLocationItems.Select(fixedTreeLocationItem => fixedTreeLocationItem.Paths.FullPath).Where(fullPath => ! fixedPathsConfiguredInCode.Contains(fullPath)))
                                {
                                    warnings.Add(string.Format("The path '{0}' was in the configuration, but no code for it has been generated (or the path in the code is invalid).", notFoundPath));
                                }
                            }
                        }
                    }

                    if (allTemplateItems != null)
                    {
                        // Check for templates that are in the configuration, but not in the code
                        IEnumerable<string> unmappedTemplateNames = attributesMapping.Keys.Select(key => key.TemplateName).Except(allTemplateItems.Select(template => template.Name));
                        foreach (string unmappedTemplateName in unmappedTemplateNames)
                        {
                            warnings.Add(string.Format("Template with name '{0}' is included as a domain object in the configuration, but no domain object for it is available.", unmappedTemplateName));
                        }

                        // Check template fields that are in the configuration, but not in the code
                        foreach (DomainObjectAttribute mappedTemplate in attributesMapping.Keys.Where(mappedTemplate => allTemplateItems.Select(template => template.Name).Contains(mappedTemplate.TemplateName)))
                        {
                            IEnumerable<ID> mappedTemplateFieldIds = attributesMapping[mappedTemplate].Select(mappedTemplateField => mappedTemplateField.FieldId);
                            foreach(TemplateItem template in allTemplateItems.Where(template => template.Name.Equals(mappedTemplate.TemplateName)))
                            {
                                IEnumerable<TemplateFieldItem> templateFields = template.OwnFields ?? new TemplateFieldItem[0];
                                foreach (TemplateFieldItem templateField in templateFields.Where(templateField => ! mappedTemplateFieldIds.Contains(templateField.ID)))
                                {
                                    warnings.Add(string.Format("Field '{0}' ({1}) on template with name '{2}' is available in Sitecore, but no code for it has been generated.", templateField.Name, templateField.ID, template.Name));
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void CheckModuleVersion(List<string> errors, List<string> warnings)
        {
            IEnumerable<VersionAttribute> versionCheckAttribute =
                        from a in AppDomain.CurrentDomain.GetAssemblies()
                        from t in ConfigurationUtil.ExecuteIgnoreExceptions<Type[]>(() => a.GetTypes()) ?? new Type[0]
                        let attributes = ConfigurationUtil.ExecuteIgnoreExceptions<object[]>(() => t.GetCustomAttributes(typeof(VersionAttribute), true)) ?? new object[0]
                        where attributes != null && attributes.Length > 0
                        select attributes.Cast<VersionAttribute>().First();
            string currentModuleVersion = typeof(ValidationUtil).Assembly.GetName().Version.ToString();
            if (versionCheckAttribute.Count() > 0)
            {
                if (!versionCheckAttribute.First().SitecoreModuleVersion.Equals(currentModuleVersion))
                {
                    errors.Add(string.Format("The domain model was created with CompiledDomainModel version '{0}', but you are running version '{1}'.", versionCheckAttribute.First().SitecoreModuleVersion, currentModuleVersion));
                }
            }
            else
            {
                warnings.Add("No version attribute could be found. It appears that there is no domain model available for validation.");
            }
        }

    }
}
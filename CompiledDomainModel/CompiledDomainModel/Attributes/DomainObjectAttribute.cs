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
using Sitecore.Data;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using System.Reflection;
using CompiledDomainModel.Utils;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;

namespace CompiledDomainModel.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DomainObjectAttribute : Attribute
    {
        public ID TemplateId { get; private set; }
        public string TemplateName { get; private set; }
        private string[] DatabaseNames { get; set; }
        public bool IsContributing { get; private set; }

        public DomainObjectAttribute(string templateId, string templateName, string[] databaseNames, bool isContributing = false)
        {
            TemplateId = ID.Parse(templateId);
            TemplateName = templateName;
            DatabaseNames = databaseNames;
            IsContributing = isContributing;
        }

        public IEnumerable<string> Validate(Type type, List<DomainObjectFieldAttribute> attributesList)
        {
            List<string> errors = new List<string>();

            ID templateId = ID.Parse(TemplateId);
            foreach (string databaseName in DatabaseNames)
            {
                Template template = TemplateManager.GetTemplate(templateId, Factory.GetDatabase(databaseName));
                if (template == null)
                {
                    errors.Add(string.Format("Template with ID '{0}' ({1}) could not be found in the '{2}' database, but is referenced from code.", TemplateId, TemplateName, databaseName));
                    continue;
                }

                // Check hierarchy
                IEnumerable<Template> allBaseTemplates = template.GetBaseTemplates();
                if (allBaseTemplates.Count() > 0)
                {
                    Type baseType = type.BaseType;
                    while (baseType != null && !"DomainObjectBase".Equals(baseType.Name))
                    {
                        MemberInfo[] templateName = baseType.GetMember("TEMPLATE_NAME");
                        if (templateName != null && templateName.Length > 0)
                        {
                            string templateNameValue = ((FieldInfo)templateName[0]).GetRawConstantValue().ToString();
                            if (!allBaseTemplates.Select(baseTemplate => baseTemplate.Name).Contains(templateNameValue))
                            {
                                errors.Add(string.Format("In the domain model code, the template with ID '{0}' ({1}) in the '{2}' database inherits from '{3}'. But the actual template in the database does not inherit from that template.", TemplateId, TemplateName, databaseName, templateNameValue));
                            }
                        }
                        baseType = baseType.BaseType;
                    }
                }

                // Field validations
                IEnumerable<DomainObjectFieldAttribute> fieldAttributes =
                    (from m in type.GetMembers()
                     let attributes = ConfigurationUtil.ExecuteIgnoreExceptions<object[]>(() => m.GetCustomAttributes(typeof(DomainObjectFieldAttribute), true)) ?? new object[0]
                     where attributes != null && attributes.Count() > 0
                     select attributes.Cast<DomainObjectFieldAttribute>()).SelectMany(attrs => attrs);
                foreach (DomainObjectFieldAttribute fieldAttribute in fieldAttributes)
                {
                    attributesList.Add(fieldAttribute);

                    TemplateField templateFieldItem = template.GetField(fieldAttribute.FieldId);
                    if (templateFieldItem == null)
                    {
                        errors.Add(string.Format("The field '{0}' ({1}) could not be found on template '{2}', but it is referenced from code.", fieldAttribute.FieldId, fieldAttribute.FieldName, template.Name));
                        continue;
                    }
                    if (!templateFieldItem.Type.Equals(fieldAttribute.FieldType))
                    {
                        errors.Add(string.Format("The field '{0}' ({1}) could be found on template '{2}', but its type ({3}) is different from the type referenced from code ({4}).", fieldAttribute.FieldId, fieldAttribute.FieldName, template.Name, templateFieldItem.Type, fieldAttribute.FieldType));
                        continue;
                    }
                }
            }

            return errors.Count > 0 ? errors : null;
        }
    }
}
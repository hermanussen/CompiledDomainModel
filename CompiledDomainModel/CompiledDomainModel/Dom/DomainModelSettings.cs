/*
    CompiledDomainModel Sitecore module
    Copyright (C) 2011  Robin Hermanussen

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
using Sitecore.Data.Items;
using CompiledDomainModel.Utils;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;

namespace CompiledDomainModel.Dom
{
    /// <summary>
    /// All global settings for code generation are set in this class.
    /// </summary>
    public class DomainModelSettings
    {
        /// <summary>
        /// The DomainObjectSets that are child items of the settings.
        /// </summary>
        public IEnumerable<DomainObjectSet> DomainObjectSets { get; private set; }

        /// <summary>
        /// The FixedPathSets that are child items of the settings.
        /// </summary>
        public List<FixedPathSet> FixedPathSets { get; private set; }

        public IEnumerable<TemplateInSet> AllTemplatesInSets
        {
            get
            {
                return DomainObjectSets != null
                    ? DomainObjectSets.Where(domainObjectSet => domainObjectSet.TemplatesInSet != null).SelectMany(domainObjectSet => domainObjectSet.TemplatesInSet)
                    : new List<TemplateInSet>();
            }
        }

        public IEnumerable<ContributingTemplateInSet> AllContributingTemplatesInSets
        {
            get
            {
                return DomainObjectSets != null
                    ? DomainObjectSets.Where(domainObjectSet => domainObjectSet.ContributingTemplatesInSet != null).SelectMany(domainObjectSet => domainObjectSet.ContributingTemplatesInSet)
                    : new List<ContributingTemplateInSet>();
            }
        }

        public bool RemoveDependencies { get; private set; }

        private Item SettingsItem { get; set; }

        public DomainModelSettings()
        {
            SettingsItem = ConfigurationUtil.SettingsItem;

            CheckboxField removeDependenciesField = FieldTypeManager.GetField(SettingsItem.Fields["Remove dependencies"]) as CheckboxField;
            Assert.IsNotNull(removeDependenciesField, "The field 'Remove dependencies' cannot be found on the CDM settings item");
            RemoveDependencies = removeDependenciesField.Checked;

            Item[] domainObjectSetItems = SettingsItem.Axes.SelectItems(string.Format("*[@@TemplateName='DomainObjectSet']"));
            DomainObjectSets = domainObjectSetItems != null
                ? domainObjectSetItems.Select(item => new DomainObjectSet(item, Namespace)).ToList()
                : null;

            // template hierarchy can only be determined after all domain object sets are loaded; that's here
            if (DomainObjectSets != null)
            {
                if (AllTemplatesInSets != null && AllTemplatesInSets.Count() > 0)
                {
                    DomUtil.DetermineHierarchy(AllTemplatesInSets);
                }
                if (AllContributingTemplatesInSets != null && AllContributingTemplatesInSets.Count() > 0)
                {
                    DomUtil.DetermineHierarchy(AllContributingTemplatesInSets);
                }
            }

            AddContributingTemplatesToTemplates();

            Item[] fixedPathSetItems = SettingsItem.Axes.SelectItems(string.Format("*[@@TemplateName='FixedPathSet']"));
            FixedPathSets = fixedPathSetItems != null
                ? fixedPathSetItems.Select(item => new FixedPathSet(item, AllTemplatesInSets)).ToList()
                : null;
            if (FixedPathSets != null && FixedPathSets.Count() > 0)
            {
                // Merge doubles
                int index = 1;
                foreach (IEnumerable<FixedTreeLocation> set in FixedPathSets.Where(fixedPathSet => fixedPathSet.FixedTreeLocations != null).Select(fixedPathSet => fixedPathSet.FixedTreeLocations).ToList())
                {
                    foreach (FixedTreeLocation location in set)
                    {
                        var matchingLocations = FixedPathSets.Where(fixedPathSet => fixedPathSet.FixedTreeLocations != null).Skip(index)
                            .SelectMany(otherSet => otherSet.FixedTreeLocations
                                .Where(otherLocation => location.Item.Paths.FullPath.Equals(otherLocation.Item.Paths.FullPath))
                                .Select(otherLocation => new { Set = otherSet, Location = otherLocation }));
                        foreach (var matchingLocation in matchingLocations.ToList())
                        {
                            location.Databases.AddRange(matchingLocation.Location.Databases.Except(location.Databases));
                            matchingLocation.Set.FixedTreeLocations.Remove(matchingLocation.Location);
                        }
                    }
                    index++;
                }
            }
        }

        private void AddContributingTemplatesToTemplates()
        {
            // find all contributing templates
            Dictionary<Sitecore.Data.ID, List<ContributingTemplateInSet>> mappings = new Dictionary<Sitecore.Data.ID, List<ContributingTemplateInSet>>();
            foreach (ContributingTemplateInSet contributingTemplate in AllContributingTemplatesInSets.ToList())
            {
                foreach (TemplateInSet template in AllTemplatesInSets.ToList())
                {
                    if ((from higherTemplate in ConfigurationUtil.GetBaseTemplatesDeep(template.TemplateItem)
                         where higherTemplate.ID.Equals(contributingTemplate.TemplateItem.ID)
                         select higherTemplate).Count() > 0)
                    {
                        if (!mappings.ContainsKey(template.TemplateItem.ID))
                        {
                            mappings.Add(template.TemplateItem.ID, new List<ContributingTemplateInSet>());
                        }
                        mappings[template.TemplateItem.ID].Add(contributingTemplate);
                    }
                }
            }

            // add contributing templates (interfaces do not have to be re-implemented in subclasses)
            foreach (DomainObjectSet set in DomainObjectSets ?? new DomainObjectSet[0])
            {
                set.AddContributingTemplates(mappings);
            }
        }

        public string Namespace
        {
            get
            {
                return SettingsItem.Fields["Namespace"] != null && SettingsItem.Fields["Namespace"].HasValue ? SettingsItem.Fields["Namespace"].Value : "MyDomainModel";
            }
        }

    }
}
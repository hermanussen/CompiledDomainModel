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
    public class DomainObjectSet : BaseSet
    {

        public IEnumerable<TemplateInSet> TemplatesInSet { get; private set; }

        public IEnumerable<ContributingTemplateInSet> ContributingTemplatesInSet { get; private set; }

        public string Namespace { get; private set; }

        public DomainObjectSet(Item domainObjectSetItem, string baseNamespace)
            : base(domainObjectSetItem)
        {
            Namespace = string.Format("{0}.{1}", baseNamespace, DomUtil.ConvertCaseString(domainObjectSetItem.Name));
            TemplatesInSet = CreateTemplatesInSet(domainObjectSetItem);
            ContributingTemplatesInSet = CreateContributingTemplatesInSet(domainObjectSetItem);
        }

        private IEnumerable<ContributingTemplateInSet> CreateContributingTemplatesInSet(Item domainObjectSetItem)
        {
            MultilistField templatesField = FieldTypeManager.GetField(domainObjectSetItem.Fields["Templates that contribute to Domain Model"]) as MultilistField;
            Assert.IsNotNull(templatesField, "The 'Templates that contribute to Domain Model' field could not be found.");
            Item[] templateItems = templatesField.GetItems();
            return templateItems != null ? templateItems.Select(templateItem => new ContributingTemplateInSet(templateItem, Databases, Namespace)).ToList() : null;
        }

        protected IEnumerable<TemplateInSet> CreateTemplatesInSet(Item domainObjectSetItem)
        {
            MultilistField templatesField = FieldTypeManager.GetField(domainObjectSetItem.Fields["Templates to map to Domain Model"]) as MultilistField;
            Assert.IsNotNull(templatesField, "The 'Templates to map to Domain Model' field could not be found.");
            Item[] templateItems = templatesField.GetItems();
            List<TemplateInSet> templatesInSet = templateItems != null ? templateItems.Select(templateItem => new TemplateInSet(templateItem, Databases, Namespace)).ToList() : null;
            return templatesInSet;
        }

        public void AddContributingTemplates(Dictionary<Sitecore.Data.ID, List<ContributingTemplateInSet>> mappings)
        {
            foreach (TemplateInSet template in TemplatesInSet)
            {
                if (mappings.ContainsKey(template.TemplateItem.ID))
                {
                    template.AddContributions(mappings);
                }
            }
        }
    }
}
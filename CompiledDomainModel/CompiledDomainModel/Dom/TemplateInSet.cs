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

namespace CompiledDomainModel.Dom
{
    public class TemplateInSet : TemplateInSetBase
    {
        public TemplateInSet BaseTemplateInSet { get; set; }

        public IEnumerable<TemplateSection> SectionsIncludingContributions
        {
            get
            {
                List<TemplateSection> allSections = new List<TemplateSection>();
                if (Sections != null)
                {
                    allSections.AddRange(Sections);
                }
                if (Contributions != null)
                {
                    foreach (ContributingTemplateInSet contrib in Contributions)
                    {
                        if (contrib.Sections != null)
                        {
                            allSections.AddRange(contrib.Sections);
                        }
                    }
                }
                return allSections.Count > 0 ? allSections : null;
            }
        }

        public List<ContributingTemplateInSet> Contributions { get; set; }

        /// <summary>
        /// Returns all the fields on the template and the base templates (in the domain model).
        /// </summary>
        public IEnumerable<TemplateField> AllFieldsFlat
        {
            get
            {
                IEnumerable<TemplateField> fields = (SectionsIncludingContributions ?? new List<TemplateSection>()).SelectMany(section => section.TemplateFields);
                if (BaseTemplateInSet != null)
                {
                    IEnumerable<TemplateField> allFields = fields.Concat(BaseTemplateInSet.AllFieldsFlat ?? new List<TemplateField>());
                    return allFields.Count() > 0 ? allFields : null;
                }
                return fields.Count() > 0 ? fields : null;
            }
        }

        public TemplateInSet(Item item, string[] databases, string setNamespace)
            : base(item, databases, setNamespace)
        {
        }

        public void AddContributions(Dictionary<Sitecore.Data.ID, List<ContributingTemplateInSet>> mappings)
        {
            Contributions = new List<ContributingTemplateInSet>();

            foreach (ContributingTemplateInSet contribution in mappings[TemplateItem.ID])
            {
                bool canAdd = true;
                TemplateInSet checkTemplate = BaseTemplateInSet;
                while (checkTemplate != null)
                {
                    if (mappings.ContainsKey(checkTemplate.TemplateItem.ID)
                        && mappings[checkTemplate.TemplateItem.ID].Select(contrib => contrib.TemplateItem.ID).Contains(contribution.TemplateItem.ID))
                    {
                        canAdd = false;
                        break;
                    }
                    checkTemplate = checkTemplate.BaseTemplateInSet;
                }
                if (canAdd)
                {
                    Contributions.Add(contribution);
                }
            }
            if (Contributions.Count == 0)
            {
                Contributions = null;
            }
        }

    }
}
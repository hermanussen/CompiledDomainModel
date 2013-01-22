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
    /// <summary>
    /// Represents a contributing template, for use within a DomainObjectSet.
    /// </summary>
    public class ContributingTemplateInSet : TemplateInSetBase
    {
        public List<ContributingTemplateInSet> BaseContributingTemplatesInSet { get; set; }

        /// <summary>
        /// The full class name for the interface variant of the contributing template.
        /// </summary>
        public string IFullClassName
        {
            get
            {
                int lastDot = FullClassName.LastIndexOf('.');
                return string.Format("{0}.I{1}", FullClassName.Substring(0, lastDot), FullClassName.Substring(lastDot + 1));
            }
        }

        public ContributingTemplateInSet(Item item, string[] databases, string setNamespace) : base(item, databases, setNamespace)
        {
        }

        public IEnumerable<TemplateField> AllFields
        {
            get
            {
                IEnumerable<TemplateField> allFields = (Sections ?? new List<TemplateSection>()).SelectMany(section => section.TemplateFields);
                return allFields.Count() > 0 ? allFields : null;
            }
        }
    }
}
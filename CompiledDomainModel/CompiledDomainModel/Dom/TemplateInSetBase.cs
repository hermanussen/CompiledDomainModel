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

namespace CompiledDomainModel.Dom
{
    public abstract class TemplateInSetBase
    {
        public TemplateItem TemplateItem { get; protected set; }

        public string TemplateId { get; protected set; }

        public string TemplateName { get; protected set; }

        public string ClassName { get; protected set; }

        public string FullClassName { get; protected set; }

        public string[] Databases { get; protected set; }

        public IEnumerable<TemplateSection> Sections { get; protected set; }

        public TemplateInSetBase(Item item, string[] databases, string setNamespace)
        {
            TemplateItem = new TemplateItem(item);
            TemplateId = item.ID.ToString();
            TemplateName = item.Name;
            ClassName = DomUtil.ConvertCaseString(item.Name);
            FullClassName = string.Format("{0}.{1}", setNamespace, ClassName);
            Databases = databases;
            TemplateSectionItem[] sectionItems = TemplateItem.GetSections();
            Sections = sectionItems != null ? sectionItems.Select(sectionItem => new TemplateSection(sectionItem, false)) : null;
        }
    }
}
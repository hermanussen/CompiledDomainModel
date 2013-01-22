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
    public class TemplateSection
    {
        public string RegionName { get; protected set; }

        public string RegionClassName { get; protected set; }

        public IEnumerable<TemplateField> TemplateFields { get; protected set; }

        public bool IsContributing { get; protected set; }

        public TemplateSection(TemplateSectionItem sectionItem, bool isContributing)
        {
            IsContributing = isContributing;
            RegionName = sectionItem.Name;
            RegionClassName = DomUtil.ConvertCaseString(sectionItem.Name);

            TemplateFieldItem[] fieldItems = sectionItem.GetFields();
            TemplateFields = fieldItems != null ? fieldItems.Select(fieldItem => new TemplateField(fieldItem)) : null;
        }

    }
}
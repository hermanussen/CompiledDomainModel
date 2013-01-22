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
using Sitecore.Data.Fields;
using CompiledDomainModel.Utils;

namespace CompiledDomainModel.Dom
{
    public class TemplateField
    {
        public TemplateFieldItem FieldItem { get; protected set; }

        public string FieldClassName { get; protected set; }
        public string FieldConstantName { get; protected set; }
        public string FieldId { get; protected set; }
        public string FieldName { get; protected set; }
        public string FieldType { get; protected set; }

        public bool IsUnmapped { get { return !(IsCheckbox || IsMultilist || IsInternalLink || IsReferenceField || IsLookupField || IsDate || IsUrl || IsNumeric || IsDecimal || IsImage || IsFile); } }
        public bool IsMultilist { get { return IsFieldTypeAssignableFrom(typeof(MultilistField)); } }
        public bool IsInternalLink { get { return IsFieldTypeAssignableFrom(typeof(InternalLinkField)); } }
        public bool IsReferenceField { get { return IsFieldTypeAssignableFrom(typeof(ReferenceField)); } }
        public bool IsLookupField { get { return IsFieldTypeAssignableFrom(typeof(LookupField)); } }
        public bool IsCheckbox { get { return IsFieldTypeAssignableFrom(typeof(CheckboxField)); } }
        public bool IsDate { get { return IsFieldTypeAssignableFrom(typeof(DateField)); } }
        public bool IsUrl { get { return IsFieldTypeAssignableFrom(typeof(LinkField)); } }
        public bool IsNumeric { get { return "integer".Equals(FieldType, StringComparison.InvariantCultureIgnoreCase); } }
        public bool IsDecimal { get { return "number".Equals(FieldType, StringComparison.InvariantCultureIgnoreCase); } }
        public bool IsImage { get { return IsFieldTypeAssignableFrom(typeof(Sitecore.Data.Fields.ImageField)); } }
        public bool IsFile { get { return IsFieldTypeAssignableFrom(typeof(FileField)); } }

        public TemplateField(TemplateFieldItem fieldItem)
        {
            FieldItem = fieldItem;
            FieldClassName = DomUtil.ConvertCaseString(fieldItem.Name);
            FieldConstantName = DomUtil.ConvertCaseString(fieldItem.Name, "_").ToUpperInvariant();
            FieldId = fieldItem.ID.ToString();
            FieldName = fieldItem.Name;
            FieldType = fieldItem.Type;
        }

        private bool IsFieldTypeAssignableFrom(Type type)
        {
            FieldType fieldType = FieldTypeManager.GetFieldType(FieldType);
            return fieldType != null ? type.IsAssignableFrom(fieldType.Type) : false;
        }
    }
}
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
    public abstract class FixedTreeLocationBase<T> : FixedTreeLocationBase where T : FixedTreeLocationBase
    {
        public T Parent { get; set; }

        public FixedTreeLocationBase(Item item, TemplateInSet domainObject, string[] databases)
            : base(item, domainObject, databases)
        {
        }
    }

    public abstract class FixedTreeLocationBase
    {
        public string ClassName { get; protected set; }

        public Item Item { get; private set; }

        public TemplateInSet DomainObject { get; private set; }

        public List<string> Databases { get; protected set; }

        public abstract string Namespace { get; }

        public abstract string FullNamespace { get; }

        public FixedTreeLocationBase(Item item, TemplateInSet domainObject, string[] databases)
        {
            Item = item;
            ClassName = DomUtil.ConvertCaseString(item.Name);
            DomainObject = domainObject;
            Databases = databases.ToList();
        }

        public override bool Equals(object obj)
        {
            if (obj as FixedTreeLocationBase == null)
            {
                return false;
            }
            return Item.ID.Equals(((FixedTreeLocationBase)obj).Item.ID);
        }

        public override int GetHashCode()
        {
            return Item.ID.GetHashCode();
        }
    }
}
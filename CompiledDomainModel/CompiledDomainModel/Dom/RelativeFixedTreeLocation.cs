﻿/*
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
    public class RelativeFixedTreeLocation : FixedTreeLocationBase<RelativeFixedTreeLocation>
    {
        public List<RelativeFixedTreeLocation> Children { get; set; }

        public IEnumerable<string> ChildNamespaces
        {
            get
            {
                return Children != null
                    ? Children.Select(child => child.Namespace).Distinct()
                    : null;
            }
        }

        public override string Namespace
        {
            get
            {
                return Parent != null ? Parent.FullNamespace : string.Format(".Roots.{0}_{1}", ClassName, Item.ID.ToShortID());
            }
        }

        public override string FullNamespace
        {
            get
            {
                return string.Format("{0}.{1}", Parent != null ? Parent.FullNamespace : "", Parent != null ? ClassName : string.Format("{0}_{1}", ClassName, Item.ID.ToShortID()));
            }
        }

        public RelativeFixedTreeLocation(Item item, TemplateInSet domainObject, string[] databases)
            : base(item, domainObject, databases)
        {
        }

    }
}
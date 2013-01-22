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

namespace CompiledDomainModel.Dom
{
    public class MultipleInheritanceException : Exception
    {
        public IEnumerable<string> TemplateNames { get; private set; }

        public MultipleInheritanceException(IEnumerable<string> templateNames)
            : base(string.Format("The following templates would require multiple inheritance to be supported: {0}. C# does not supoort this. You can use 'contributing templates' as a workaround to support multiple inheritance.", string.Join(", ", templateNames.ToArray())))
        {
            TemplateNames = templateNames;
        }
    }
}
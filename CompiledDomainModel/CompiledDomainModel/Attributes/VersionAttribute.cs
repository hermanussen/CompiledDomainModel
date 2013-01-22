/*
    CompiledDomainModel Sitecore module
    Copyright (C) 2010-2011  Robin Hermanussen

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

namespace CompiledDomainModel.Attributes
{
    /// <summary>
    /// Meta information about the version of the CompiledDomainModel module that a model was created with and an auto-incrementing version number for the model.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class VersionAttribute : Attribute
    {
        /// <summary>
        /// The version of the CompiledDomainModel module that a model was created with.
        /// </summary>
        public string SitecoreModuleVersion { get; private set; }

        /// <summary>
        /// An auto-incrementing version number for the model.
        /// </summary>
        public long DomainModelVersion { get; private set; }

        public VersionAttribute(string sitecoreModuleVersion, long domainModelVersion)
        {
            SitecoreModuleVersion = sitecoreModuleVersion;
            DomainModelVersion = domainModelVersion;
        }
    }
}
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
using Sitecore.Data;
using Sitecore.Configuration;
using Sitecore.Data.Items;

namespace CompiledDomainModel.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FixedPathAttribute : Attribute
    {
        public ID LocationId { get; private set; }
        public string FullPath { get; private set; }
        public string[] DatabaseNames { get; private set; }
        public Type DomainObjectType { get; set; }

        public FixedPathAttribute(string locationId, string fullPath, string[] databaseNames)
        {
            LocationId = ID.Parse(locationId);
            FullPath = fullPath;
            DatabaseNames = databaseNames;
        }

        public void Validate(List<string> errors, List<string> warnings)
        {
            if (DatabaseNames == null || DatabaseNames.Length == 0)
            {
                errors.Add(string.Format("No databases have been defined for fixed path '{0}' ({1}). The corresponding fixed path is unsafe to be used!", FullPath, LocationId));
                return;
            }
            foreach(string databaseName in DatabaseNames)
            {
                Database database = Factory.GetDatabase(databaseName);
                Item item = database.GetItem(LocationId);
                if (item == null)
                {
                    item = database.GetItem(FullPath);
                    if (item == null)
                    {
                        errors.Add(string.Format("The fixed path '{0}' ({1}) is not available in the {2} database (not by id, nor by path).", FullPath, LocationId, databaseName));
                    }
                    else
                    {
                        warnings.Add(string.Format("The fixed path '{0}' ({1}) is available at the given path in the {2} database, but the expected item ID does not match.", FullPath, LocationId, databaseName));
                    }
                }
                else
                {
                    item = database.GetItem(FullPath);
                    if (item == null)
                    {
                        warnings.Add(string.Format("The fixed path '{0}' ({1}) is not available in the {2} database, but the item is still accessible with its ID (this is a fallback and should be corrected).", FullPath, LocationId, databaseName));
                    }
                }
            }
        }
    }
}
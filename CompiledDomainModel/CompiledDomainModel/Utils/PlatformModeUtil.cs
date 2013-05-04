/*
    CompiledDomainModel Sitecore module
    Copyright (C) 2013  Robin Hermanussen

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
using CompiledDomainModel.Attributes;
using Sitecore.Data;

namespace CompiledDomainModel.Utils
{
    public static class PlatformModeUtil
    {
        /// <summary>
        /// Find all generated classes with the DomainObjectAttribute, so they can be mapped.
        /// </summary>
        /// <param name="contributing">Return normal or contributing templates</param>
        /// <returns>A dictionary that maps template IDs to the types they need to be mapped to</returns>
        public static IDictionary<ID, Type> GetAllPlatformTypeMappings(bool contributing)
        {
            IDictionary<ID, Type> result = new Dictionary<ID, Type>();

            var typesWithMyAttribute =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in ConfigurationUtil.ExecuteIgnoreExceptions<Type[]>(() => a.GetTypes()) ?? new Type[0]
                let attributes = ConfigurationUtil.ExecuteIgnoreExceptions<object[]>(() => t.GetCustomAttributes(typeof(DomainObjectAttribute), true)) ?? new object[0]
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<DomainObjectAttribute>() };
            foreach (var typeWithMyAttribute in typesWithMyAttribute)
            {
                foreach (var attribute in typeWithMyAttribute.Attributes)
                {
                    result.Add(attribute.TemplateId, typeWithMyAttribute.Type);
                }
            }

            return result;
        }
    }
}
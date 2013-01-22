/*
    CompiledDomainModel Sitecore module
    Copyright (C) 2010-2012  Robin Hermanussen

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
using Sitecore.Diagnostics;
using Sitecore.Data.Templates;

namespace CompiledDomainModel.Utils
{
    public static class ConfigurationUtil
    {
        public static Item SettingsItem
        {
            get
            {
                Sitecore.Data.ID configItemId = Sitecore.Data.ID.Parse("{3E324911-0B5C-449A-AD9F-1AE18465AB03}");
                Item item = Sitecore.Configuration.Factory.GetDatabase("master").GetItem(configItemId);
                Assert.IsNotNull(item, string.Format("There is no configuration object ({0})", configItemId));
                Assert.IsTrue("Settings".Equals(item.TemplateName), string.Format("'{0}' is not a valid template for configuring the CompiledDomainModel module", item.TemplateName));
                return item;
            }
        }

        public static IEnumerable<TemplateItem> GetBaseTemplatesDeep(TemplateItem template)
        {
            TemplateList lBaseTemplates = Sitecore.Data.Managers.TemplateManager.GetTemplate(template.ID, template.Database).GetBaseTemplates();

            if (lBaseTemplates != null)
            {
                return lBaseTemplates.Select(templ => new TemplateItem(template.Database.GetItem(templ.ID)));
            }

            return new List<TemplateItem>(0);
        }

        public static T ExecuteIgnoreExceptions<T>(Func<T> function)
        {
            try
            {
                return function();
            }
            catch(Exception)
            {
                return default(T);
            }
        }
    }
}
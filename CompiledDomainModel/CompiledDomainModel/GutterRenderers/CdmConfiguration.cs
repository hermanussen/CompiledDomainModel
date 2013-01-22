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
using Sitecore.Shell.Applications.ContentEditor.Gutters;
using Sitecore.Diagnostics;
using Sitecore.Data.Items;
using CompiledDomainModel.Utils;
using Sitecore.Data.Fields;

namespace CompiledDomainModel.GutterRenderers
{
    public class CdmConfiguration :  GutterRenderer
    {
        protected override GutterIconDescriptor GetIconDescriptor(Item item)
        {
            Assert.ArgumentNotNull(item, "item");

            Item settingsItem = ConfigurationUtil.SettingsItem;

            // Display an icon next to mapped templates
            if ("Template".Equals(item.TemplateName, StringComparison.InvariantCultureIgnoreCase))
            {
                Item[] domainObjectSetItems = settingsItem.Axes.SelectItems(string.Format("*[@@TemplateName='DomainObjectSet']")) ?? new Item[0];
                foreach (Item domainObjectSetItem in domainObjectSetItems)
                {
                    bool mapped = Sitecore.Data.ID.ParseArray(domainObjectSetItem["Templates to map to Domain Model"]).Contains(item.ID);
                    bool contributing = ! mapped && Sitecore.Data.ID.ParseArray(domainObjectSetItem["Templates that contribute to Domain Model"]).Contains(item.ID);
                    if (mapped || contributing)
                    {
                        GutterIconDescriptor descriptor = new GutterIconDescriptor();
                        descriptor.Icon = domainObjectSetItem.Appearance.Icon;
                        descriptor.Tooltip = string.Format("This template is mapped {0} in domain object set '{1}'.",
                                                            mapped ? "directly" : "as a contributing template",
                                                            domainObjectSetItem.Name);
                        return descriptor;
                    }
                }
            }

            // Display an icon next to paths that are fixed
            Item[] fixedPathSetItems = settingsItem.Axes.SelectItems(string.Format("*[@@TemplateName='FixedPathSet']")) ?? new Item[0];
            foreach(Item fixedPathSetItem in fixedPathSetItems)
            {
                MultilistField fixedTreeLocationsField = FieldTypeManager.GetField(fixedPathSetItem.Fields["Fixed tree locations"]) as MultilistField;
                Item[] fixedTreeItems = fixedTreeLocationsField.GetItems() ?? new Item[0];
                foreach (Item fixedTreeItem in fixedTreeItems)
                {
                    if (fixedTreeItem.Paths.FullPath.Contains(item.Paths.FullPath))
                    {
                        GutterIconDescriptor descriptor = new GutterIconDescriptor();
                        descriptor.Icon = fixedPathSetItem.Appearance.Icon;
                        descriptor.Tooltip = string.Format("This path is configured to be fixed in fixed path set '{0}'.",
                                                            fixedPathSetItem.Name);
                        return descriptor;
                    }
                }

                MultilistField relativeFixedTreeLocationsField = FieldTypeManager.GetField(fixedPathSetItem.Fields["Relative fixed tree locations"]) as MultilistField;
                Item[] relativeFixedTreeItems = relativeFixedTreeLocationsField.GetItems() ?? new Item[0];
                foreach (Item relativeFixedTreeItem in relativeFixedTreeItems)
                {
                    if (item.ID.Equals(relativeFixedTreeItem.ID)
                        || (item.Axes.GetAncestors() ?? new Item[0]).Select(it => it.ID).Contains(relativeFixedTreeItem.ID))
                    {
                        GutterIconDescriptor descriptor = new GutterIconDescriptor();
                        descriptor.Icon = fixedPathSetItem.Appearance.Icon;
                        descriptor.Tooltip = string.Format("This path is configured to be the base for a relative fixed path in set '{0}'.",
                                                            fixedPathSetItem.Name);
                        return descriptor;
                    }
                }
            }

            return null;
        }
    }
}
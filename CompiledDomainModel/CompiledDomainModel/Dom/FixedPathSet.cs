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
using Sitecore.Diagnostics;

namespace CompiledDomainModel.Dom
{
    public class FixedPathSet : BaseSet
    {

        public List<FixedTreeLocation> FixedTreeLocations { get; private set; }

        public List<RelativeFixedTreeLocation> RelativeFixedTreeLocations { get; private set; }

        private IEnumerable<TemplateInSet> AllTemplatesInSets { get; set; }

        public FixedPathSet(Item fixedPathSetItem, IEnumerable<TemplateInSet> allTemplatesInSets)
            : base(fixedPathSetItem)
        {
            AllTemplatesInSets = allTemplatesInSets;
            FixedTreeLocations = CreateFixedTreeLocations(fixedPathSetItem);
            RelativeFixedTreeLocations = CreateRelativeFixedTreeLocations(fixedPathSetItem);
        }

        public List<FixedTreeLocation> CreateFixedTreeLocations(Item fixedPathSetItem)
        {
            MultilistField fixedTreeLocationsField = FieldTypeManager.GetField(fixedPathSetItem.Fields["Fixed tree locations"]) as MultilistField;
            Assert.IsNotNull(fixedTreeLocationsField, "The 'Fixed tree locations' field could not be found.");
            Item[] fixedTreeLocationItems = fixedTreeLocationsField.GetItems();

            List<FixedTreeLocation> fixedTreeLocations = new List<FixedTreeLocation>();
            if (fixedTreeLocationItems != null)
            {
                foreach (Item fixedTreeLocationItem in fixedTreeLocationItems)
                {
                    AddToFixedTree(fixedTreeLocations, fixedTreeLocationItem);
                }
            }

            return fixedTreeLocations != null && fixedTreeLocations.Count > 0 ? fixedTreeLocations : null;
        }

        public List<RelativeFixedTreeLocation> CreateRelativeFixedTreeLocations(Item fixedPathSetItem)
        {
            MultilistField relativeFixedTreeLocationsField = FieldTypeManager.GetField(fixedPathSetItem.Fields["Relative fixed tree locations"]) as MultilistField;
            Assert.IsNotNull(relativeFixedTreeLocationsField, "The 'Relative fixed tree locations' field could not be found.");
            Item[] relativeFixedTreeLocationItems = relativeFixedTreeLocationsField.GetItems();

            List<RelativeFixedTreeLocation> relativeFixedTreeLocations = new List<RelativeFixedTreeLocation>();
            if (relativeFixedTreeLocationItems != null)
            {
                foreach (Item fixedTreeLocationItem in relativeFixedTreeLocationItems)
                {
                    AddToRelativeFixedTree(relativeFixedTreeLocations, fixedTreeLocationItem);
                }
            }

            return relativeFixedTreeLocations != null && relativeFixedTreeLocations.Count > 0 ? relativeFixedTreeLocations : null;
        }

        private FixedTreeLocation AddToFixedTree(List<FixedTreeLocation> fixedTreeLocations, Item fixedTreeLocationItem)
        {
            if (fixedTreeLocationItem != null && fixedTreeLocationItem.Parent != null)
            {
                IEnumerable<FixedTreeLocation> matchLocations = fixedTreeLocations.Where(fixedTreeLocation => fixedTreeLocation.Item.ID.Equals(fixedTreeLocationItem.ID));
                if (matchLocations.Count() > 0)
                {
                    return matchLocations.First();
                }

                TemplateInSet domainObject = null;
                if (AllTemplatesInSets != null && AllTemplatesInSets.Count() > 0)
                {
                    IEnumerable<TemplateInSet> potentialDomainObjects = AllTemplatesInSets.Where(templateInSet => fixedTreeLocationItem.TemplateID.Equals(templateInSet.TemplateItem.ID));
                    if (potentialDomainObjects.Count() > 0)
                    {
                        domainObject = potentialDomainObjects.First();
                    }
                }
                FixedTreeLocation newLocation = new FixedTreeLocation(fixedTreeLocationItem, domainObject, Databases);
                fixedTreeLocations.Add(newLocation);

                newLocation.Parent = AddToFixedTree(fixedTreeLocations, fixedTreeLocationItem.Parent);
                return newLocation;
            }
            return null;
        }

        private RelativeFixedTreeLocation AddToRelativeFixedTree(List<RelativeFixedTreeLocation> relativeFixedTreeLocations, Item fixedTreeLocationItem)
        {
            TemplateInSet domainObject = null;
            if (AllTemplatesInSets != null && AllTemplatesInSets.Count() > 0)
            {
                IEnumerable<TemplateInSet> potentialDomainObjects = AllTemplatesInSets.Where(templateInSet => fixedTreeLocationItem.TemplateID.Equals(templateInSet.TemplateItem.ID));
                if (potentialDomainObjects.Count() > 0)
                {
                    domainObject = potentialDomainObjects.First();
                }
            }
            RelativeFixedTreeLocation newLocation = new RelativeFixedTreeLocation(fixedTreeLocationItem, domainObject, Databases);
            relativeFixedTreeLocations.Add(newLocation);

            if (fixedTreeLocationItem.HasChildren)
            {
                newLocation.Children = new List<RelativeFixedTreeLocation>();
                foreach (Item child in fixedTreeLocationItem.Children)
                {
                    RelativeFixedTreeLocation childLocation = AddToRelativeFixedTree(relativeFixedTreeLocations, child);
                    childLocation.Parent = newLocation;
                    newLocation.Children.Add(childLocation);
                }
            }

            return newLocation;
        }
    }
}
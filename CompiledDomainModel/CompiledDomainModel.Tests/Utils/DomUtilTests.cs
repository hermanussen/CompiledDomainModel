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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompiledDomainModel.Utils;
using CompiledDomainModel.Dom;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Data.Templates;

namespace CompiledDomainModel.Tests.Utils
{
    [TestClass]
    public class DomUtilTests
    {
        [TestMethod]
        public void TestConvertCaseString()
        {
            try
            {
                TestConvertCaseString(null, null);
                Assert.Fail("Passing null should throw an exception in the ConvertCaseString method");
            }
            catch (InvalidOperationException) { }
            try
            {
                TestConvertCaseString("", "");
                Assert.Fail("Passing an empty string should throw an exception in the ConvertCaseString method");
            }
            catch (InvalidOperationException) { }

            TestConvertCaseString("MyTemplate", "My template");
            TestConvertCaseString("MyTemplate", "my .,- template");
            TestConvertCaseString("SomeUPP", "SomeUPP");
            TestConvertCaseString("SomeUPP", "-S/o*m+e\t\r\nU&&%P^#@!~P");

            TestConvertCaseString("_3_SomeUPP", "3_SomeUPP");

            TestConvertCaseString("My_Template", "My template", "_");
            TestConvertCaseString("My_Template", "My Template", "_");
            TestConvertCaseString("MyLLLTemplate", "My template", "LLL");
            TestConvertCaseString("My_Template", "MyTemplate", "_");
            TestConvertCaseString("My_Super_Complicated_TTemplate", "MySuperComplicatedTTemplate", "_");
        }

        private void TestConvertCaseString(string expectedStr, string inputStr)
        {
            Assert.AreEqual(expectedStr, DomUtil.ConvertCaseString(inputStr));
        }

        private void TestConvertCaseString(string expectedStr, string inputStr, string separator)
        {
            Assert.AreEqual(expectedStr, DomUtil.ConvertCaseString(inputStr, separator));
        }

        private class DetermineHierarchyTest
        {
            public Dictionary<ID, IEnumerable<Template>> templateMappings = new Dictionary<ID, IEnumerable<Template>>();
            public Dictionary<ID, Item> itemMappings = new Dictionary<ID, Item>();

            public Func<ID, Item> getIdFunc;

            public DetermineHierarchyTest()
            {
                // static mocks
                Sitecore.Data.Items.Moles.MTemplateItem.AllInstances.GetSections = (instance) => null;
                Sitecore.Data.Managers.Moles.MTemplateManager.GetTemplateIDDatabase = (id, database) =>
                    new Sitecore.Data.Templates.Moles.MTemplate()
                    {
                        GetBaseTemplates = () => new TemplateList(templateMappings.ContainsKey(id) ? templateMappings[id] : new Template[0])
                    };

                getIdFunc = (id) => itemMappings.ContainsKey(id) ? itemMappings[id] : null;
            }

            public void Run(IDictionary<TemplateInSetBase, IEnumerable<TemplateInSetBase>> mappings)
            {
                // mock with passed in mappings
                foreach (KeyValuePair<TemplateInSetBase, IEnumerable<TemplateInSetBase>> mapping in mappings)
                {
                    if (mapping.Value != null && mapping.Value.Count() > 0)
                    {
                        Template[] templates = new Template[mapping.Value.Count()];
                        for (int i = 0; i < mapping.Value.Count(); i++)
                        {
                            ID templateId = mapping.Value.ElementAt(i).TemplateItem.ID;
                            templates[i] = new Sitecore.Data.Templates.Moles.MTemplate() { IDGet = () => templateId };
                        }
                        templateMappings.Add(mapping.Key.TemplateItem.ID, templates );
                    }
                }

                if (mappings.Keys.OfType<TemplateInSet>().Count() > 0)
                {
                    DomUtil.DetermineHierarchy(mappings.Keys.OfType<TemplateInSet>().ToArray());
                }
                if (mappings.Keys.OfType<ContributingTemplateInSet>().Count() > 0)
                {
                    DomUtil.DetermineHierarchy(mappings.Keys.OfType<ContributingTemplateInSet>().ToArray());
                }
            }

            public ContributingTemplateInSet CreateContributingTemplateInSet(string templateName)
            {
                return new ContributingTemplateInSet(CreateMockItem(templateName), new string[0], "MyDomainModel");
            }

            public TemplateInSet CreateTemplateInSet(string templateName)
            {
                return new TemplateInSet(CreateMockItem(templateName), new string[0], "MyDomainModel");
            }

            private Item CreateMockItem(string templateName)
            {
                ID theId = ID.NewID;
                Item result = new Sitecore.Data.Items.Moles.MItem01()
                {
                    IDGet = () => theId,
                    NameGet = () => templateName,
                    TemplateGet = () => new Sitecore.Data.Items.Moles.MTemplateItem(),
                    DatabaseGet = () => new Sitecore.Data.Moles.MDatabase()
                    {
                        GetItemID = (id) => getIdFunc(id)
                    }
                };
                itemMappings.Add(theId, result);
                return result;
            }
        }

        [TestMethod]
        [HostType("Moles")]
        public void TestDetermineHierarchy01()
        {
            DetermineHierarchyTest test = new DetermineHierarchyTest();

            TemplateInSet templateInSet = test.CreateTemplateInSet("SubTemplate");
            TemplateInSet baseTemplateInSet = test.CreateTemplateInSet("BaseTemplate");

            test.Run(new Dictionary<TemplateInSetBase, IEnumerable<TemplateInSetBase>>()
                {
                    {templateInSet, new TemplateInSet[] { baseTemplateInSet }},
                    {baseTemplateInSet, null}
                });

            Assert.AreSame(baseTemplateInSet, templateInSet.BaseTemplateInSet);
            Assert.IsNull(baseTemplateInSet.BaseTemplateInSet);
        }

        [TestMethod]
        [HostType("Moles")]
        public void TestDetermineHierarchy02()
        {
            DetermineHierarchyTest test = new DetermineHierarchyTest();

            TemplateInSet templateInSet = test.CreateTemplateInSet("SubTemplate");
            TemplateInSet baseTemplateInSet = test.CreateTemplateInSet("BaseTemplate");
            TemplateInSet baseBaseTemplateInSet = test.CreateTemplateInSet("BaseBaseTemplate");

            test.Run(new Dictionary<TemplateInSetBase, IEnumerable<TemplateInSetBase>>()
                {
                    {templateInSet, new TemplateInSet[] { baseTemplateInSet }},
                    {baseTemplateInSet, new TemplateInSet[] { baseBaseTemplateInSet }},
                    {baseBaseTemplateInSet, null}
                });

            Assert.AreSame(baseTemplateInSet, templateInSet.BaseTemplateInSet);

            Assert.AreSame(baseBaseTemplateInSet, templateInSet.BaseTemplateInSet.BaseTemplateInSet);
            Assert.AreSame(baseBaseTemplateInSet, baseTemplateInSet.BaseTemplateInSet);
            Assert.IsNull(baseBaseTemplateInSet.BaseTemplateInSet);
        }

        [TestMethod]
        [HostType("Moles")]
        public void TestDetermineHierarchy03()
        {
            DetermineHierarchyTest test = new DetermineHierarchyTest();

            TemplateInSet templateInSet = test.CreateTemplateInSet("SubTemplate");
            TemplateInSet baseTemplateInSet = test.CreateTemplateInSet("BaseTemplate");
            TemplateInSet baseTemplateInSet2 = test.CreateTemplateInSet("BaseTemplate2");

            try
            {
                test.Run(new Dictionary<TemplateInSetBase, IEnumerable<TemplateInSetBase>>()
                {
                    {templateInSet, new TemplateInSet[] { baseTemplateInSet, baseTemplateInSet2 }},
                    {baseTemplateInSet, null},
                    {baseTemplateInSet2, null}
                });
                Assert.Fail("Exception should have been thrown");
            }
            catch (MultipleInheritanceException exc)
            {
                Assert.IsTrue(exc.TemplateNames.Count() == 1);
                Assert.AreEqual(templateInSet.TemplateName, exc.TemplateNames.First());
            }
        }

        [TestMethod]
        [HostType("Moles")]
        public void TestDetermineHierarchy04()
        {
            DetermineHierarchyTest test = new DetermineHierarchyTest();

            TemplateInSet templateInSet = test.CreateTemplateInSet("SubTemplate");
            TemplateInSet templateInSet2 = test.CreateTemplateInSet("SubTemplate2");
            TemplateInSet baseTemplateInSet = test.CreateTemplateInSet("BaseTemplate");
            TemplateInSet baseBaseTemplateInSet = test.CreateTemplateInSet("BaseBaseTemplate");

            test.Run(new Dictionary<TemplateInSetBase, IEnumerable<TemplateInSetBase>>()
                {
                    {templateInSet2, new TemplateInSet[] { baseTemplateInSet }},
                    {templateInSet, new TemplateInSet[] { baseTemplateInSet }},
                    {baseTemplateInSet, new TemplateInSet[] { baseBaseTemplateInSet }},
                    {baseBaseTemplateInSet, null}
                });

            Assert.AreSame(baseBaseTemplateInSet, templateInSet2.BaseTemplateInSet.BaseTemplateInSet);
            Assert.AreSame(baseBaseTemplateInSet, templateInSet.BaseTemplateInSet.BaseTemplateInSet);
            Assert.AreSame(baseBaseTemplateInSet, baseTemplateInSet.BaseTemplateInSet);

            Assert.AreSame(baseTemplateInSet, templateInSet.BaseTemplateInSet);
            Assert.AreSame(baseTemplateInSet, templateInSet2.BaseTemplateInSet);
            Assert.IsNull(baseBaseTemplateInSet.BaseTemplateInSet);
        }

        [TestMethod]
        [HostType("Moles")]
        public void TestDetermineHierarchy05()
        {
            DetermineHierarchyTest test = new DetermineHierarchyTest();

            // hierarchy mappings:
            // A > B > C
            //   > D > E > F
            // G > H

            TemplateInSet templateA = test.CreateTemplateInSet("A");
            TemplateInSet templateB = test.CreateTemplateInSet("B");
            TemplateInSet templateC = test.CreateTemplateInSet("C");
            TemplateInSet templateD = test.CreateTemplateInSet("D");
            TemplateInSet templateE = test.CreateTemplateInSet("E");
            TemplateInSet templateF = test.CreateTemplateInSet("F");
            TemplateInSet templateG = test.CreateTemplateInSet("G");
            TemplateInSet templateH = test.CreateTemplateInSet("H");

            Dictionary<TemplateInSetBase, IEnumerable<TemplateInSetBase>> mappings = new Dictionary<TemplateInSetBase, IEnumerable<TemplateInSetBase>>()
                {
                    {templateA, null},
                    {templateB, new TemplateInSet[] { templateA }},
                    {templateC, new TemplateInSet[] { templateB }},
                    {templateD, new TemplateInSet[] { templateA }},
                    {templateE, new TemplateInSet[] { templateD }},
                    {templateF, new TemplateInSet[] { templateE }},
                    {templateG, null},
                    {templateH, new TemplateInSet[] { templateG }}
                };
            mappings.Reverse();
            test.Run(mappings);

            Assert.IsNull(templateA.BaseTemplateInSet);
            Assert.IsNull(templateG.BaseTemplateInSet);

            Assert.AreSame(templateA, templateB.BaseTemplateInSet);
            Assert.AreSame(templateB, templateC.BaseTemplateInSet);
            Assert.AreSame(templateA, templateD.BaseTemplateInSet);
            Assert.AreSame(templateD, templateE.BaseTemplateInSet);
            Assert.AreSame(templateE, templateF.BaseTemplateInSet);
            Assert.AreSame(templateG, templateH.BaseTemplateInSet);
        }

        [TestMethod]
        [HostType("Moles")]
        public void TestDetermineHierarchyForContributingTemplates01()
        {
            DetermineHierarchyTest test = new DetermineHierarchyTest();

            // hierarchy mappings:
            // A > B > C
            //   > D > E > F
            // G >
            //     H > J
            // I >

            ContributingTemplateInSet templateA = test.CreateContributingTemplateInSet("A");
            ContributingTemplateInSet templateB = test.CreateContributingTemplateInSet("B");
            ContributingTemplateInSet templateC = test.CreateContributingTemplateInSet("C");
            ContributingTemplateInSet templateD = test.CreateContributingTemplateInSet("D");
            ContributingTemplateInSet templateE = test.CreateContributingTemplateInSet("E");
            ContributingTemplateInSet templateF = test.CreateContributingTemplateInSet("F");
            ContributingTemplateInSet templateG = test.CreateContributingTemplateInSet("G");
            ContributingTemplateInSet templateH = test.CreateContributingTemplateInSet("H");
            ContributingTemplateInSet templateI = test.CreateContributingTemplateInSet("I");
            ContributingTemplateInSet templateJ = test.CreateContributingTemplateInSet("J");

            Dictionary<TemplateInSetBase, IEnumerable<TemplateInSetBase>> mappings = new Dictionary<TemplateInSetBase, IEnumerable<TemplateInSetBase>>()
                {
                    {templateA, null},
                    {templateB, new ContributingTemplateInSet[] { templateA }},
                    {templateC, new ContributingTemplateInSet[] { templateB }},
                    {templateD, new ContributingTemplateInSet[] { templateA }},
                    {templateE, new ContributingTemplateInSet[] { templateD }},
                    {templateF, new ContributingTemplateInSet[] { templateE }},
                    {templateG, null},
                    {templateH, new ContributingTemplateInSet[] { templateG, templateI }},
                    {templateI, null},
                    {templateJ, new ContributingTemplateInSet[] { templateH }}
                };
            mappings.Reverse();
            test.Run(mappings);

            Assert.IsNull(templateA.BaseContributingTemplatesInSet);
            Assert.IsNull(templateG.BaseContributingTemplatesInSet);
            Assert.IsNull(templateI.BaseContributingTemplatesInSet);

            Assert.AreSame(templateA, templateB.BaseContributingTemplatesInSet.First());
            Assert.AreSame(templateB, templateC.BaseContributingTemplatesInSet.First());
            Assert.AreSame(templateA, templateD.BaseContributingTemplatesInSet.First());
            Assert.AreSame(templateD, templateE.BaseContributingTemplatesInSet.First());
            Assert.AreSame(templateE, templateF.BaseContributingTemplatesInSet.First());
            Assert.AreSame(templateH, templateJ.BaseContributingTemplatesInSet.First());
            Assert.IsTrue(templateH.BaseContributingTemplatesInSet.Contains(templateG));
            Assert.IsTrue(templateH.BaseContributingTemplatesInSet.Contains(templateI));

            Assert.IsFalse(templateC.BaseContributingTemplatesInSet.Contains(templateA));
            Assert.IsFalse(templateF.BaseContributingTemplatesInSet.Contains(templateD));
            Assert.IsFalse(templateF.BaseContributingTemplatesInSet.Contains(templateA));
            Assert.IsFalse(templateE.BaseContributingTemplatesInSet.Contains(templateA));
            Assert.IsFalse(templateJ.BaseContributingTemplatesInSet.Contains(templateG));
            Assert.IsFalse(templateJ.BaseContributingTemplatesInSet.Contains(templateI));
        }
    }
}

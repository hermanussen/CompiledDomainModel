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
using System.Text;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using CompiledDomainModel.Dom;

namespace CompiledDomainModel.Utils
{
    public static class DomUtil
    {
        private static readonly CodeDomProvider CodeDomProvider = CodeDomProvider.CreateProvider("C#");

        private static readonly Regex REGEX_PLACE_SPACES = new Regex("[a-z][A-Z]", RegexOptions.Compiled);

        private static readonly Regex REGEX_CHECK_MUST_PREFIX = new Regex("^[0-9]+.?", RegexOptions.Compiled);

        public static string ConvertCaseString(string phrase)
        {
            return ConvertCaseString(phrase, "");
        }

        public static string ConvertCaseString(string phrase, string separator)
        {
            Assert.IsTrue(!string.IsNullOrEmpty(phrase), "An empty/null phrase was passed to ConvertCaseString");
            string separatedPhrase = REGEX_PLACE_SPACES.Replace(phrase, new MatchEvaluator((m) => string.Format("{0} {1}", m.Value[0], m.Value[1])));
            string[] splittedPhrase = separatedPhrase.Split(' ', '-', '.');
            var sb = new StringBuilder();

            bool first = true;
            foreach (String s in splittedPhrase)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(separator);
                }
                char[] splittedPhraseChars = s.ToCharArray();
                if (splittedPhraseChars.Length > 0)
                {
                    splittedPhraseChars[0] = ((new String(splittedPhraseChars[0], 1)).ToUpper().ToCharArray())[0];
                }
                sb.Append(new String(splittedPhraseChars));
            }
            string result = sb.ToString();
            if (REGEX_CHECK_MUST_PREFIX.IsMatch(result))
            {
                result = string.Format("_{0}", result);
            }
            if (!CodeDomProvider.IsValidIdentifier(result))
            {
                result = string.Join("", (from pChar in result
                                          where CodeDomProvider.IsValidIdentifier(pChar.ToString())
                                          select pChar.ToString()).ToArray());
            }
            return result;
        }

        public static string GetHelpText(Item item, int leadingSpaces)
        {
            string result = null;
            string description = string.IsNullOrEmpty(item.Help.Text) ? item.Help.ToolTip : item.Help.Text;
            if (!string.IsNullOrEmpty(description))
            {
                result = string.Format("Description: {0}", description);
            }
            if (!string.IsNullOrEmpty(item.Help.Link))
            {
                string link = string.Format("(Check '{0}' for more info)", Sitecore.Web.UI.WebControls.FieldRenderer.Render(item, "__Help link"));
                result = result != null ? result + Environment.NewLine + link : link;
            }
            return result != null ? result.Replace(Environment.NewLine, Environment.NewLine + "/// ".PadLeft(leadingSpaces)) : null;
        }

        /// <summary>
        /// Determines the hierarchy between the selected contributing templates.
        /// </summary>
        /// <param name="allContributingTemplatesInSets">All the contributing templates that were selected for use with CDM.</param>
        public static void DetermineHierarchy(IEnumerable<ContributingTemplateInSet> allContributingTemplatesInSets)
        {
            IEnumerable<Sitecore.Data.ID> allTemplateIds = allContributingTemplatesInSets.Select(template => template.TemplateItem.ID);

            // Determine all the base templates
            IDictionary<ContributingTemplateInSet, List<ContributingTemplateInSet>> mappings =
                            (from template in allContributingTemplatesInSets
                             let baseTemplateIdsDeep = ConfigurationUtil.GetBaseTemplatesDeep(template.TemplateItem).Select(baseTemplate => baseTemplate.ID).Intersect(allTemplateIds)
                             select new { TemplateInSet = template, BaseTemplateIdsDeep = baseTemplateIdsDeep })
                                .ToDictionary(k => k.TemplateInSet, e => allContributingTemplatesInSets.Where(t => e.BaseTemplateIdsDeep.Contains(t.TemplateItem.ID)).ToList());
            foreach (KeyValuePair<ContributingTemplateInSet, List<ContributingTemplateInSet>> mappedTemplate in mappings)
            {
                mappedTemplate.Key.BaseContributingTemplatesInSet = mappedTemplate.Value.Count > 0 ? mappedTemplate.Value : null;
            }
        }

        /// <summary>
        /// Determines the hierarchy between the selected templates.
        /// Sets the base templates.
        /// </summary>
        /// <param name="allTemplatesInSets">All the templates that were selected for use with CDM.</param>
        public static void DetermineHierarchy(IEnumerable<TemplateInSet> allTemplatesInSets)
        {
            IEnumerable<Sitecore.Data.ID> allTemplateIds = allTemplatesInSets.Select(template => template.TemplateItem.ID);

            // Keep references to all deep base templates (that are also mapped)
            IDictionary<TemplateInSet,IEnumerable<Sitecore.Data.ID>> mappings =
                            (from template in allTemplatesInSets
                             let baseTemplateIdsDeep = ConfigurationUtil.GetBaseTemplatesDeep(template.TemplateItem).Select(baseTemplate => baseTemplate.ID).Intersect(allTemplateIds)
                             select new { TemplateInSet = template, BaseTemplateIdsDeep = baseTemplateIdsDeep }).ToDictionary(k => k.TemplateInSet, e => e.BaseTemplateIdsDeep);

            // Determine all the top level templates (that will not have a BaseTemplateInSet)
            List<TemplateInSet> mappedTemplates = allTemplatesInSets.Where(template => mappings[template].Count() == 0).ToList();
            
            // Determine what templates are directly beneath the top level templates
            foreach (TemplateInSet mappedTemplate in new List<TemplateInSet>(mappedTemplates))
            {
                Determine(allTemplatesInSets, mappedTemplates, mappings, mappedTemplate);
            }

            // Anything that remains is in violation of single inheritance
            IEnumerable<TemplateInSet> unmappedTemplates = allTemplatesInSets.Except(mappedTemplates);
            if (unmappedTemplates.Count() > 0)
            {
                throw new MultipleInheritanceException(unmappedTemplates.Select(templ => templ.TemplateName));
            }
        }

        private static void Determine(IEnumerable<TemplateInSet> allTemplatesInSets, List<TemplateInSet> mappedTemplates, IDictionary<TemplateInSet,IEnumerable<Sitecore.Data.ID>> mappings, TemplateInSet templateInSet)
        {
            IEnumerable<Sitecore.Data.ID> allTemplateIds = allTemplatesInSets.Select(template => template.TemplateItem.ID);

            // Get all mapped templates that are directly beneath the passed in templateInSet
            List<TemplateInSet> newMappedTemplates = (from template in allTemplatesInSets.Except(mappedTemplates)
                                                      where mappings[template].Intersect(allTemplateIds.Except(GetAllMappedTemplateIds(templateInSet))).Count() == 0
                                                     select template).ToList();
            mappedTemplates.AddRange(newMappedTemplates);
            foreach (TemplateInSet newNewMappedTemplate in newMappedTemplates)
            {
                newNewMappedTemplate.BaseTemplateInSet = templateInSet;

                // Recursively determine the next layer directly beneath
                Determine(allTemplatesInSets, mappedTemplates, mappings, newNewMappedTemplate);
            }
        }

        private static IEnumerable<Sitecore.Data.ID> GetAllMappedTemplateIds(TemplateInSet templateInSet)
        {
            List<Sitecore.Data.ID> result = new List<Sitecore.Data.ID>();
            if (templateInSet != null)
            {
                result.Add(templateInSet.TemplateItem.ID);
                result.AddRange(GetAllMappedTemplateIds(templateInSet.BaseTemplateInSet));
            }
            return result;
        }

        
    }
}
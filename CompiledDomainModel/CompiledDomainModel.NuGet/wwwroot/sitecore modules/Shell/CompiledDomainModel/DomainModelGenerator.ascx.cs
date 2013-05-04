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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using System.CodeDom.Compiler;
using CompiledDomainModel.Dom;
using CompiledDomainModel.Attributes;
using CompiledDomainModel.Utils;
using System.Web.UI;
using Sitecore.Web;

namespace CompiledDomainModel.sitecore_modules.Shell.CompiledDomainModel
{
    public partial class DomainModelGenerator : System.Web.UI.UserControl
    {
        public string ErrorMessage { get; private set; }

        protected DomainModelSettings Settings { get; private set; }

        /// <summary>
        /// Auto-incrementing version number for the currently generated DomainModel.
        /// </summary>
        public long DomainModelVersion
        {
            get
            {
                IEnumerable<VersionAttribute> typesWithMyAttribute =
                        from a in AppDomain.CurrentDomain.GetAssemblies()
                        from t in ConfigurationUtil.ExecuteIgnoreExceptions<Type[]>(() => a.GetTypes()) ?? new Type[0]
                        let attributes = ConfigurationUtil.ExecuteIgnoreExceptions<object[]>(() => t.GetCustomAttributes(typeof(VersionAttribute), true)) ?? new object[0]
                        where attributes != null && attributes.Length > 0
                        select attributes.Cast<VersionAttribute>().First();
                if (typesWithMyAttribute.Count() > 0)
                {
                    return typesWithMyAttribute.First().DomainModelVersion + 1;
                }
                return 1; // default value
            }
        }

        protected object RegenerateUrl
        {
            get
            {
                string url = Request.Url.AbsoluteUri.Contains('?')
                    ? Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.IndexOf('?'))
                    : Request.Url.AbsoluteUri;
                if (Settings.PlatformMode && ! string.IsNullOrEmpty(WebUtil.GetQueryString("platformsets")))
                {
                    url = string.Format("{0}?platformsets={1}", url, WebUtil.GetQueryString("platformsets"));
                }
                url = WebUtil.AddQueryString(url, "direct", "DomainModelGenerator");
                return url;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            try
            {
                Settings = new DomainModelSettings();

                if (Settings.PlatformMode && string.IsNullOrEmpty(WebUtil.GetQueryString("platformsets")))
                {
                    writer.WriteLine("// PLATFORM MODE IS ON - please check the CompiledDomainModel settings if this was unintentional");
                    writer.WriteLine("//");
                    writer.WriteLine("// Please use the following url and change it to your needs:");
                    writer.WriteLine(string.Format("// {0}?platformsets=<PIPE_SEPARATED_SET_NAMES>", Request.Url.AbsoluteUri.Contains('?') ? Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.IndexOf('?')) : Request.Url.AbsoluteUri));
                    writer.WriteLine("//");
                    writer.WriteLine("// Use one or more of the following names to generate for the part of the platform you need (exclude the brackets):");
                    writer.WriteLine("// [Core] (should be used in a project that all other projects have a dependency with)");
                    IEnumerable<DomainObjectSet> domainObjectSets = Settings.DomainObjectSets ?? new List<DomainObjectSet>();
                    foreach (DomainObjectSet domainObjectSet in domainObjectSets)
                    {
                        writer.WriteLine(string.Format("// [{0}] (domain objects)", domainObjectSet.Name));
                    }
                    List<FixedPathSet> fixedPathSets = Settings.FixedPathSets ?? new List<FixedPathSet>();
                    foreach (FixedPathSet fixedPathSet in fixedPathSets)
                    {
                        writer.WriteLine(string.Format("// [{0}] (fixed paths)", fixedPathSet.Name));
                    }
                    writer.WriteLine("//");
                    writer.WriteLine("// As a starting point, this is what the URL would look like if you would want to generate everything");
                    writer.WriteLine(string.Format("// {0}?platformsets=Core|{1}",
                        Request.Url.AbsoluteUri.Contains('?') ? Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.IndexOf('?')) : Request.Url.AbsoluteUri,
                        string.Join("|", domainObjectSets.OfType<BaseSet>().Concat(fixedPathSets.OfType<BaseSet>()).Select(s => s.Name).ToArray())));
                    return;
                }

                DataBind();
                base.Render(writer);
            }
            catch (MultipleInheritanceException ex)
            {
                ErrorMessage = ex.Message;
                writer.WriteLine(string.Format("// Refresh from URL: {0}?direct=DomainModelGenerator", Request.Url.AbsoluteUri.Contains('?') ? Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.IndexOf('?')) : Request.Url.AbsoluteUri));
                writer.WriteLine(string.Format("// {0}", ErrorMessage));
            }
        }

        
    }
}
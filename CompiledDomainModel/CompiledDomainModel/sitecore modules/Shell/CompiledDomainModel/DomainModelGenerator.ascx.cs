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
using Sitecore.Data.Items;
using System.CodeDom.Compiler;
using CompiledDomainModel.Dom;
using CompiledDomainModel.Attributes;
using CompiledDomainModel.Utils;
using System.Web.UI;

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

        protected override void Render(HtmlTextWriter writer)
        {
            try
            {
                Settings = new DomainModelSettings();
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
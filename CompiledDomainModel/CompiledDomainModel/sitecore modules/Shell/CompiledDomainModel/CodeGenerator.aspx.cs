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
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Data.Fields;
using System.Text;
using CompiledDomainModel.Utils;
using CompiledDomainModel.Attributes;
using CompiledDomainModel.sitecore_modules.Shell.CompiledDomainModel;
using System.IO;

namespace CompiledDomainModel
{
    public partial class CodeGenerator : System.Web.UI.Page
    {
        public IEnumerable<string> Generators
        {
            get
            {
                List<string> result = new List<string>();
                result.Add("DomainModelGenerator.ascx");
                string customGeneratorsDir = string.Format(@"{0}\CustomGenerators", Path.GetDirectoryName(Request.PhysicalPath));
                if (Directory.Exists(customGeneratorsDir))
                {
                    string[] generators = Directory.GetFiles(customGeneratorsDir, "*.ascx", SearchOption.TopDirectoryOnly);
                    if (generators != null && generators.Count() > 0)
                    {
                        foreach (string generator in generators)
                        {
                            result.Add(string.Format(@"CustomGenerators\{0}", Path.GetFileName(generator)));
                        }
                    }
                }
                return result;
            }
        }


        protected void SelectedGeneratorChanged(object sender, EventArgs e)
        {
            int selectedGenerator = ((DropDownList)sender).SelectedIndex;
            DataBind();
            ((DropDownList)sender).SelectedIndex = selectedGenerator;
            if (plhGenerator != null)
            {
                plhGenerator.Controls.Clear();
                plhGenerator.Controls.Add(LoadControl(Generators.ElementAt(selectedGenerator)));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Sitecore.Context.State.DataBind = false;

            if (!string.IsNullOrEmpty(Request.QueryString["direct"]))
            {
                string generator = HttpUtility.UrlDecode(Request.QueryString["direct"]) + ".ascx";
                if (Generators.Contains(generator))
                {
                    DownloadDirect(generator);
                    return;
                }
            }
            
            if (!IsPostBack)
            {
                DataBind();
                if (plhGenerator != null)
                {
                    plhGenerator.Controls.Clear();
                    plhGenerator.Controls.Add(LoadControl(Generators.First()));
                }
            }
        }

        protected void Download(object sender, EventArgs e)
        {
            DownloadDirect(Generators.ElementAt(lstGenerators.SelectedIndex));
        }

        private void DownloadDirect(string generator)
        {
            if (plhGenerator != null)
            {
                Response.AppendHeader("Content-Disposition", "attachment; filename=rename_this");
                Control controlToRender = LoadControl(generator);
                controlToRender.RenderControl(new HtmlTextWriter(Response.Output));
                Response.End();
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            using (System.IO.MemoryStream msOur = new System.IO.MemoryStream())
            {
                using (System.IO.StreamWriter swOur = new System.IO.StreamWriter(msOur))
                {
                    HtmlTextWriter ourWriter = new HtmlTextWriter(swOur);

                    if (plhGenerator.Controls.Count > 0 && plhGenerator.Controls[0] is DomainModelGenerator && ((DomainModelGenerator) plhGenerator.Controls[0]).ErrorMessage != null)
                    {
                        Response.Write(string.Format("<html><head><title>Error</title><body><p>Error: {0}</p></body>", ((DomainModelGenerator)plhGenerator.Controls[0]).ErrorMessage));
                    }
                    else
                    {
                        base.Render(ourWriter);
                        ourWriter.Flush();
                        msOur.Position = 0;
                        using (System.IO.StreamReader oReader = new System.IO.StreamReader(msOur))
                        {
                            bool encounteredWhiteLine = false;
                            while (!oReader.EndOfStream)
                            {
                                string sTxt = oReader.ReadLine();
                                if (string.IsNullOrEmpty(sTxt) || sTxt.Trim().Equals(""))
                                {
                                    if (encounteredWhiteLine)
                                    {
                                        continue;
                                    }
                                    encounteredWhiteLine = true;
                                }
                                else
                                {
                                    encounteredWhiteLine = false;
                                }
                                Response.Write(sTxt + Environment.NewLine);
                            }
                            oReader.Close();
                        }
                    }
                }
            }
        }
    }
}
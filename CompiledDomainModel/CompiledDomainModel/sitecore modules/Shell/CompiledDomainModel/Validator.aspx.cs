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
using CompiledDomainModel.Validation;

namespace CompiledDomainModel.sitecore_modules.Shell.CompiledDomainModel
{
    public partial class Validator : System.Web.UI.Page
    {
        public List<string> Errors { get; private set; }
        public List<string> Warnings { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Errors = new List<string>();
            Warnings = new List<string>();

            ValidationUtil.ValidateDomainModel(Errors, Warnings);

            if (Errors.Count == 0)
            {
                Errors = null;
            }
            if (Warnings.Count == 0)
            {
                Warnings = null;
            }

            DataBind();
        }
    }
}
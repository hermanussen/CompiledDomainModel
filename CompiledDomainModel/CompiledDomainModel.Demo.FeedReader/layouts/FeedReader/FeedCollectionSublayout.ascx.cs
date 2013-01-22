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
using MyDomainModel;
using Sitecore.Data.Items;

namespace CompiledDomainModel.Demo.FeedReader.layouts
{
    public partial class FeedCollectionSublayout : FeedReaderSublayoutBase
    {
        private string NewFeedTitle { get; set; }
        private string NewFeedUrl { get; set; }
        
        protected override void OnPreRender(EventArgs e)
        {
            DataBind();
            base.OnLoad(e);
        }

        public void AddFeed(object sender, CommandEventArgs e)
        {
            if ("Add".Equals(e.CommandName))
            {
                string feedTitle = Request.Form["NewFeedTitle"];
                string feedUrl = Request.Form["NewFeedUrl"];
                if (!string.IsNullOrEmpty(feedTitle) && !string.IsNullOrEmpty(feedUrl))
                {
                    FeedCollection.AddFeed(feedTitle, feedUrl);
                }
            }
        }

    }
}
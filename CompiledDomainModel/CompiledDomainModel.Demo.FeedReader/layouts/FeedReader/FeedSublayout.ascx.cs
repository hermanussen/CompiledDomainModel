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
using MyDomainModel.FeedReader;
using Sitecore.Data.Items;
using System.ComponentModel;
using Sitecore.Links;

namespace CompiledDomainModel.Demo.FeedReader.layouts
{
    public partial class FeedSublayout : FeedReaderSublayoutBase
    {

        private Feed feed;

        [Bindable(true)]
        public Feed Feed
        {
            get
            {
                if (feed == null)
                {
                    feed = ItemWrapper.CreateTypedWrapper(Sitecore.Context.Item) as Feed;
                }
                return feed;
            }
            set
            {
                feed = value;
            }
        }

        public bool IsStandalone
        {
            get
            {
                return Feed != null && Sitecore.Context.Item.ID.Equals(Feed.Item.ID);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (IsStandalone)
            {
                DataBind();
            }
            base.OnLoad(e);
        }

        public void RefreshFeed(object sender, CommandEventArgs e)
        {
            if ("Refresh".Equals(e.CommandName))
            {
                IEnumerable<Feed> feeds = FeedCollection.GetChildren<Feed>() ?? new List<Feed>();
                Feed feedToRefresh = feeds.Where(feed => feed.Item.ID.ToString().Equals(e.CommandArgument)).FirstOrDefault();
                if (feedToRefresh != null)
                {
                    feedToRefresh.Refresh();
                }
            }
        }

        public void RemoveFeed(object sender, CommandEventArgs e)
        {
            if ("Remove".Equals(e.CommandName))
            {
                IEnumerable<Feed> feeds = FeedCollection.GetChildren<Feed>() ?? new List<Feed>();
                Feed feedToRemove = feeds.Where(feed => feed.Item.ID.ToString().Equals(e.CommandArgument)).FirstOrDefault();
                if (feedToRemove != null)
                {
                    // you cannot display a feed that is removed, so redirect to the overview
                    string redirectUrl = IsStandalone ? LinkManager.GetItemUrl(feedToRemove.Parent.Item) : null;

                    feedToRemove.Remove();

                    if (redirectUrl != null)
                    {
                        Response.Redirect(redirectUrl);
                    }
                }
            }
        }

    }
}
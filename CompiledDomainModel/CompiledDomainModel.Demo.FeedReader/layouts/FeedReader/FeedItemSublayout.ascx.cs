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

namespace CompiledDomainModel.Demo.FeedReader.layouts
{
    public partial class FeedItemSublayout : FeedReaderSublayoutBase
    {

        private FeedItem feedItem;

        [Bindable(true)]
        public FeedItem FeedItem
        {
            get
            {
                if (feedItem == null)
                {
                    feedItem = ItemWrapper.CreateTypedWrapper(Sitecore.Context.Item) as FeedItem;
                }
                return feedItem;
            }
            set
            {
                feedItem = value;
            }
        }

        public bool IsStandalone
        {
            get
            {
                return FeedItem != null && Sitecore.Context.Item.ID.Equals(FeedItem.Item.ID);
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

        public void AddComment(object sender, CommandEventArgs e)
        {
            if ("Add".Equals(e.CommandName))
            {
                IEnumerable<FeedItem> feedItems = (FeedCollection.GetChildren<Feed>() ?? new List<Feed>()).SelectMany(feed => feed.GetChildren<FeedItem>() ?? new List<FeedItem>());
                IEnumerable<FeedItem> feedItemsToCommentOn = feedItems.Where(feed => feed.Item.ID.ToString().Equals(e.CommandArgument));
                FeedItem feedItemToCommentOn = feedItemsToCommentOn.Count() > 0 ? feedItemsToCommentOn.First() : null;
                string formValue = Request.Form[string.Format("CommentText_{0}", feedItemToCommentOn.Item.ID)];
                if (feedItemToCommentOn != null && !string.IsNullOrEmpty(formValue))
                {
                    feedItemToCommentOn.AddComment(formValue);
                }
            }
        }

    }
}
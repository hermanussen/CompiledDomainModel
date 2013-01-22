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
using Sitecore.SecurityModel;
using System.ServiceModel.Syndication;
using MyDomainModel.FixedPaths.Content;

namespace MyDomainModel.FeedReader
{
    public partial class FeedItem
    {
        public FeedItem(Feed feed, SyndicationItem syndicationItem)
            : this(feed.Item.Add(ItemUtil.ProposeValidItemName(GetTitle(syndicationItem)), new Sitecore.Data.TemplateID(FeedItem.TEMPLATE_ID)))
        {
            int maxSortOrder;
            if (feed.Children != null)
            {
                maxSortOrder = feed.Children.Select(child => child.Item.Appearance.Sortorder).Max();
            }
            else
            {
                maxSortOrder = 0;
            }
            UpdateFromSyndicationItem(syndicationItem, maxSortOrder + 10);
        }

        public void UpdateFromSyndicationItem(SyndicationItem syndicationItem)
        {
            UpdateFromSyndicationItem(syndicationItem, 0);
        }

        public void UpdateFromSyndicationItem(SyndicationItem syndicationItem, int sortOrder)
        {
            using (new EditContext(Item, SecurityCheck.Disable))
            {
                Title = GetTitle(syndicationItem);

                if (syndicationItem.Links != null
                    && syndicationItem.Links.Count > 0
                    && syndicationItem.Links.First().Uri != null)
                {
                    Link = syndicationItem.Links.First().Uri.ToString();
                }

                if (syndicationItem.Summary != null)
                {
                    Description = syndicationItem.Summary.Text;
                }
                else if (syndicationItem.Content != null && syndicationItem.Content is TextSyndicationContent)
                {
                    Description = ((TextSyndicationContent) syndicationItem.Content).Text;
                }

                if (sortOrder > 0)
                {
                    Item.Appearance.Sortorder = sortOrder;
                }
            }
        }

        public void AddComment(string text)
        {
            using (new SecurityDisabler())
            {
                new Comment(CommentsFixed.ItemWrapper.Item, text, this);
            }
        }

        public IEnumerable<Comment> Comments
        {
            get
            {
                IEnumerable<Comment> comments = FixedPaths.Content.CommentsFixed.ItemWrapper.GetChildren<Comment>();
                IEnumerable<Comment> result = comments != null
                    ? comments.Where(comment => comment.FeedItem != null && this.Item.ID.Equals(comment.FeedItem.Item.ID))
                    : null;
                return result != null && result.Count() > 0 ? result : null;
            }
        }

        public void Remove()
        {
            IEnumerable<Comment> comments = Comments ?? new Comment[0];
            foreach (Comment comment in comments)
            {
                comment.Item.Delete();
            }
            Item.Delete();
        }

        private static string GetTitle(SyndicationItem syndicationItem)
        {
            return syndicationItem.Title != null
                ? syndicationItem.Title.Text
                : "No title";
        }

    }
}
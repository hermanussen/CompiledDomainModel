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
using System.ServiceModel.Syndication;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System.Net;
using System.Xml;

namespace MyDomainModel.FeedReader
{
    public partial class Feed
    {

        public Feed(FeedCollection feedCollection, string feedTitle, string feedUrl)
            : this(feedCollection.Item.Add(ItemUtil.ProposeValidItemName(feedTitle), new Sitecore.Data.TemplateID(Feed.TEMPLATE_ID)))
        {
            using (new EditContext(Item, SecurityCheck.Disable))
            {
                this.Name = feedTitle;
                this.Url = feedUrl;
            }
        }

        public void Refresh()
        {
            IEnumerable<FeedItem> currentItems = GetChildren<FeedItem>() ?? new List<FeedItem>();
            SyndicationFeed feed = GetFeed(this.Url);
            using (new SecurityDisabler())
            {
                // clean up old items if needed
                FeedCollection feedCollection = this.Parent as FeedCollection;
                if (feedCollection != null && feedCollection.RemoveOldItems)
                {
                    IEnumerable<FeedItem> oldItems = from currentItem in currentItems where ! feed.Items.Select(item => item.Links.First().Uri.ToString()).Contains(currentItem.Link) select currentItem;
                    foreach (FeedItem oldItem in oldItems)
                    {
                        oldItem.Remove();
                    }
                }

                // add new and update existing entries
                foreach (SyndicationItem item in feed.Items.Reverse())
                {
                    if (item.Links.Count > 0)
                    {
                        IEnumerable<FeedItem> matchingItems = from currentItem in currentItems where item.Links.First().Uri.ToString().Equals(currentItem.Link) select currentItem;
                        if (matchingItems.Count() > 0)
                        {
                            matchingItems.First().UpdateFromSyndicationItem(item);
                        }
                        else
                        {
                            new FeedItem(this, item);
                        }
                    }
                }
            }
        }

        public void Remove()
        {
            using (new SecurityDisabler())
            {
                // Remove any comments first
                IEnumerable<FeedItem> feedItems = FeedItems ?? new FeedItem[0];
                foreach (FeedItem feedItem in feedItems)
                {
                    feedItem.Remove();
                }

                // Remove the feed itself
                Item.Delete();
            }
        }

        public IEnumerable<FeedItem> FeedItems
        {
            get
            {
                IEnumerable<FeedItem> lResult = GetChildren<FeedItem>();
                return lResult != null ? lResult.Reverse().Take(5) : null;
            }
        }

        /// <summary>
        /// Retrieve the Syndication from a parsed in URL.
        /// SyndicationFeed supports RSS and Atom.
        /// </summary>
        /// <param name="url">URL of the feed.</param>
        /// <returns>A loaded feed.</returns>
        private static SyndicationFeed GetFeed(string url)
        {
            SyndicationFeed feed = null;

            //WebClient is used in case the computer is sitting behind a proxy
            WebClient client = new WebClient();
            client.Proxy = WebRequest.DefaultWebProxy;
            client.Proxy.Credentials = CredentialCache.DefaultCredentials;
            client.Credentials = CredentialCache.DefaultCredentials;

            // Read the feed using an XmlReader
            using (XmlReader reader = XmlReader.Create(client.OpenRead(url)))
            {
                // Load the feed into a SyndicationFeed
                feed = SyndicationFeed.Load(reader);
            }

            return feed;
        }

    }
}
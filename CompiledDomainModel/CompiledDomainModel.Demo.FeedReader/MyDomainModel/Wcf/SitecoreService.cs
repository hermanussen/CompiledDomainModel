using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.SecurityModel;
using System.ServiceModel;

namespace MyDomainModel.Wcf
{
    /// <summary>
    /// Additions to the service that can not be generated, specific to the FeedReader.
    /// </summary>
    public partial class SitecoreService
    {
        [OperationContract]
        public void RefreshFeed(Feed feed)
        {
            Sitecore.Data.Items.Item sitecoreItem = Database.GetItem(new ID(feed.ItemID));
            // TODO: Remove security disabler for production scenarios
            using (new SecurityDisabler())
            {
                using (new Sitecore.Data.Items.EditContext(sitecoreItem))
                {
                    MyDomainModel.FeedReader.Feed domainFeed = ItemWrapper.CreateTypedWrapper(sitecoreItem) as MyDomainModel.FeedReader.Feed;
                    domainFeed.Refresh();
                }
            }
        }
    }
}
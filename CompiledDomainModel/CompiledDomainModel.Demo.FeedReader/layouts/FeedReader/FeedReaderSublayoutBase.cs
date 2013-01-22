using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Links;
using MyDomainModel;
using MyDomainModel.FeedReader;

namespace CompiledDomainModel.Demo.FeedReader.layouts
{
    public abstract class FeedReaderSublayoutBase : System.Web.UI.UserControl
    {
        private FeedCollection feedCollection;
        public FeedCollection FeedCollection
        {
            get
            {
                if (feedCollection == null)
                {
                    ItemWrapper item = ItemWrapper.CreateTypedWrapper(Sitecore.Context.Item);
                    feedCollection = item as FeedCollection;
                    if (feedCollection == null)
                    {
                        feedCollection = item.GetFirstAncestor<FeedCollection>();
                    }
                }
                return feedCollection;
            }
        }

        public string GetLink(IItemWrapper item)
        {
            return LinkManager.GetItemUrl(item.Item);
        }
    }
}
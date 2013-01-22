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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CompiledDomainModel.Demo.SLFeedReader.FeedReaderService;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace CompiledDomainModel.Demo.SLFeedReader
{
    public partial class MainPage : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public FeedCollection FeedCollection { get; private set; }

        private Feed selectedFeed;
        public Feed SelectedFeed
        {
            get
            {
                return selectedFeed;
            }
            set
            {
                if (selectedFeed != value)
                {
                    selectedFeed = value;
                    RaisePropertyChanged("SelectedFeed");
                }
            }
        }

        private FeedItem selectedFeedItem;
        public FeedItem SelectedFeedItem
        {
            get
            {
                return selectedFeedItem;
            }
            set
            {
                if (selectedFeedItem != value)
                {
                    selectedFeedItem = value;
                    RaisePropertyChanged("SelectedFeedItem");
                }
            }
        }

        private bool initialized;
        private SitecoreServiceClient sitecoreServiceClient;
        private IEnumerable<Comment> allComments;

        public MainPage()
        {
            sitecoreServiceClient = new SitecoreServiceClient();
            sitecoreServiceClient.GetFixedPathsCompleted += (sender, e) =>
                {
                    e.Result.Content_Comments.Children.LoadChildren((commentChildren) =>
                        {
                            allComments = commentChildren.LoadedChildren.OfType<Comment>();

                            FeedCollection = e.Result.Content_Feeds;
                            FeedCollection.Children.LoadChildren((feedCollectionChildren) =>
                                {
                                    foreach (Feed feed in feedCollectionChildren.LoadedChildren.OfType<Feed>())
                                    {
                                        feed.Children.LoadChildren((feedChildren) =>
                                            {
                                                foreach (FeedItem feedItem in feedChildren.LoadedChildren.OfType<FeedItem>())
                                                {
                                                    feedItem.Comments
                                                        = new ObservableCollection<Comment>(from comment in allComments
                                                            where comment.FeedItem != null && comment.FeedItem.ReferenceId == feedItem.ItemID
                                                            select comment);
                                                }
                                            });
                                    }
                                    if (!initialized)
                                    {
                                        InitializeComponent();
                                        initialized = true;
                                    }
                                });
                        });
                };
            sitecoreServiceClient.GetFixedPathsAsync();
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void lbxFeeds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedFeed = e.AddedItems.Count > 0 ? e.AddedItems.Cast<Feed>().First() : null;
        }

        private void lbxFeedItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedFeedItem = e.AddedItems.Count > 0 ? e.AddedItems.Cast<FeedItem>().First() : null;
            scrFeedItem.Visibility = SelectedFeedItem != null ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnRefreshFeed_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFeed != null)
            {
                btnRefreshFeed.IsEnabled = false;
                sitecoreServiceClient.RefreshFeedCompleted += RefreshFeedCompleted;
                sitecoreServiceClient.RefreshFeedAsync(SelectedFeed);
            }
        }


        public void RefreshFeedCompleted(object sender, AsyncCompletedEventArgs e)
        {
            sitecoreServiceClient.RefreshFeedCompleted -= RefreshFeedCompleted;
            SelectedFeed.Children.LoadChildren((feedChildren) =>
            {
                foreach (FeedItem feedItem in feedChildren.LoadedChildren.OfType<FeedItem>())
                {
                    feedItem.Comments
                        = new ObservableCollection<Comment>(from comment in allComments
                                                            where comment.FeedItem != null && comment.FeedItem.ReferenceId == feedItem.ItemID
                                                            select comment);
                }
                btnRefreshFeed.IsEnabled = true;
            });
        }
    }
}

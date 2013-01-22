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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace CompiledDomainModel.Demo.SLFeedReader.FeedReaderService
{
    public partial class ChildItemCollectionRef
    {
        private ObservableCollection<Item> loadedChildren;
        public ObservableCollection<Item> LoadedChildren
        {
            get
            {
                return loadedChildren;
            }
            set
            {
                loadedChildren = value;
                RaisePropertyChanged("LoadedChildren");
            }
        }

        public void LoadChildren(Action<ChildItemCollectionRef> callback)
        {
            SitecoreServiceClient client = new SitecoreServiceClient();
            client.GetChildrenCompleted += (sender, e) =>
            {
                LoadedChildren = e.Result != null
                    ? new ObservableCollection<Item>(e.Result)
                    : new ObservableCollection<Item>();
                if (callback != null)
                {
                    callback(this);
                }
            };
            client.GetChildrenAsync(ParentId);
        }
    }
}

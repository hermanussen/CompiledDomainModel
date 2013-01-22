<%--
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
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FeedCollectionSublayout.ascx.cs" Inherits="CompiledDomainModel.Demo.FeedReader.layouts.FeedCollectionSublayout" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="cdm" TagName="Feed" Src="~/layouts/FeedReader/FeedSublayout.ascx" %>

<asp:Repeater ID="rptFeeds" runat="server" DataSource="<%# FeedCollection.GetChildren<MyDomainModel.FeedReader.Feed>() %>">
<HeaderTemplate>
<div class="feedcollection">
    <div class="feed">
        Title: <input name="NewFeedTitle" />
        Url: <input name="NewFeedUrl" />
        <asp:Button runat="server" CommandName="Add" OnCommand="AddFeed" Text="Add feed" CssClass="buttonright" />
    </div>
    <asp:Repeater runat="server" DataSource="<%# FeedCollection.GetChildren<MyDomainModel.FeedReader.Feed>() %>">
        <HeaderTemplate>
            <div class="feed">
        </HeaderTemplate>
        <ItemTemplate><a href="#feed_<%# ((MyDomainModel.FeedReader.Feed) Container.DataItem).Item.ID.ToString() %>"><%# ((MyDomainModel.FeedReader.Feed) Container.DataItem).Name %></a></ItemTemplate>
        <SeparatorTemplate> - </SeparatorTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>
</HeaderTemplate>
<ItemTemplate>
    <a class="openlink" href="<%# GetLink(Container.DataItem as MyDomainModel.ItemWrapper) %>">Open feed</a><br />
    <cdm:Feed runat="server" Feed="<%# (MyDomainModel.FeedReader.Feed) Container.DataItem %>" />
</ItemTemplate>
<FooterTemplate>
</div>
</FooterTemplate>
</asp:Repeater>
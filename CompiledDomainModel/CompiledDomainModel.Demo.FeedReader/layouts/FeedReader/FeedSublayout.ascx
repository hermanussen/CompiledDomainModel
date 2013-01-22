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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FeedSublayout.ascx.cs" Inherits="CompiledDomainModel.Demo.FeedReader.layouts.FeedSublayout" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="cdm" TagName="FeedItem" Src="~/layouts/FeedReader/FeedItemSublayout.ascx" %>

<div class="feedcollection">
    <asp:PlaceHolder runat="server" Visible="<%# IsStandalone %>">
        <a class="openlink" href="<%# GetLink(FeedCollection) %>">Go to overview</a><br />
    </asp:PlaceHolder>
    <div class="feed">
        <span class="feedtitle">
            <a href="<%# Feed.Url %>" target="_blank" name="feed_<%# Feed.Item.ID.ToString() %>">
                <%# Feed.Name %>
            </a>
            <asp:Button runat="server" CommandName="Remove" CommandArgument="<%# Feed.Item.ID %>" OnCommand="RemoveFeed" Text="Remove" CssClass="buttonright" />
            <asp:Button runat="server" CommandName="Refresh" CommandArgument="<%# Feed.Item.ID %>" OnCommand="RefreshFeed" Text="Refresh" CssClass="buttonright" />
        </span>
        <br />
        <br />
        <asp:Repeater runat="server" DataSource="<%# Feed.FeedItems %>">
        <ItemTemplate>
            <a class="openlink" href="<%# GetLink(Container.DataItem as MyDomainModel.ItemWrapper) %>">Open </a><br />
            <cdm:FeedItem runat="server" FeedItem="<%# (MyDomainModel.FeedReader.FeedItem) Container.DataItem %>" />
        </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
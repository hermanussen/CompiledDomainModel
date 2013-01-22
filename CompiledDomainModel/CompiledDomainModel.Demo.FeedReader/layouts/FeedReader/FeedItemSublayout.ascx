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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FeedItemSublayout.ascx.cs" Inherits="CompiledDomainModel.Demo.FeedReader.layouts.FeedItemSublayout" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<div class="feeditem">
    <asp:PlaceHolder runat="server" Visible="<%# IsStandalone %>">
        <a class="openlink" href="<%# GetLink(FeedItem.Parent) %>">Go to feed</a><br />
    </asp:PlaceHolder>
    <span class="feeditemtitle">
        <a href="<%# FeedItem.Link %>" target="_blank">
            <%# FeedItem.Title %>
        </a>
    </span>
    <p><%# FeedItem.Description %></p>
    <asp:Repeater runat="server" DataSource="<%# FeedItem.Comments %>">
        <HeaderTemplate>
            <ol>
        </HeaderTemplate>
        <ItemTemplate>
                <li><%# ((MyDomainModel.FeedReader.Comment) Container.DataItem).Item.Statistics.CreatedBy %> - <%# ((MyDomainModel.FeedReader.Comment) Container.DataItem).Text %></li>
        </ItemTemplate>
        <FooterTemplate>
            </ol>
        </FooterTemplate>
    </asp:Repeater>
    <p><input name="CommentText_<%# FeedItem.Item.ID %>" /> <asp:Button runat="server" CommandName="Add" CommandArgument="<%# FeedItem.Item.ID %>" OnCommand="AddComment" Text="Add comment" CssClass="buttonright" /></p>
</div>
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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Validator.aspx.cs" Inherits="CompiledDomainModel.sitecore_modules.Shell.CompiledDomainModel.Validator" %>
<html>
<head>
    <title>Validate generated code</title>
    <style type="text/css">
        h1
        {
        	font-size: medium;
        	font-weight: bold;
        }
        h1.valid
        {
        	color: Green;
        }
        li.error
        {
        	color: Red;
        }
        li.warning
        {
        	color: Olive;
        }
    </style>
</head>
<body>
<div>
    <button onclick="document.location.reload()">Reload</button>
    <a href="/sitecore modules/Shell/CompiledDomainModel/Documentation/default.htm" target="_blank" style="float: right;">CompiledDomainModel documentation</a>
</div>
<asp:PlaceHolder runat="server" Visible="<%# Errors == null && Warnings == null %>">
    <h1 class="valid">The domain model is consistent with the Sitecore database(s)</h1>
</asp:PlaceHolder>
<asp:Repeater runat="server" DataSource="<%# Errors %>">
    <HeaderTemplate>
        <h1>Errors (must be corrected to ensure reliable behavior):</h1>
        <ul>
    </HeaderTemplate>
    <ItemTemplate>
            <li class="error"><%# Container.DataItem %></li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>
<asp:Repeater runat="server" DataSource="<%# Warnings %>">
    <HeaderTemplate>
        <h1>Warnings:</h1>
        <ul>
    </HeaderTemplate>
    <ItemTemplate>
            <li class="warning"><%# Container.DataItem %></li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>
<h1>Reports for the loaded Domain Model:</h1>
<ul>
    <li><a href="/sitecore modules/Shell/CompiledDomainModel/Reports/DomainObjectsDiagram.aspx" target="_blank">Structure for loaded domain model</a></li>
    <li><a href="/sitecore modules/Shell/CompiledDomainModel/Reports/FixedPathsDiagram.aspx" target="_blank">Fixed paths overview</a></li>
</ul>
</body>
</html>
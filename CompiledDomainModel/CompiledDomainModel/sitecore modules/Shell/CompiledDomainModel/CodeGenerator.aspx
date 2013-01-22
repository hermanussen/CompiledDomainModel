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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CodeGenerator.aspx.cs" Inherits="CompiledDomainModel.CodeGenerator" %>
<html>
<head>
    <title>Generated code</title>
    <script language="JavaScript">
        function ClipBoard() {
            Copied = document.getElementById('generatedcode').createTextRange();
            Copied.execCommand("Copy");
        }
    </script>
</head>
<body>
<form id="Form1" runat="server">
    <div style="height: 10%;">
        <button onClick="ClipBoard();">Copy to Clipboard</button>
        <button onclick="document.location.reload()">Reload</button>
        <asp:Button runat="server" OnClick="Download" Text="Download" />
        Generator: <asp:DropDownList ID="lstGenerators" runat="server" DataSource="<%# Generators %>" AutoPostBack="true" OnSelectedIndexChanged="SelectedGeneratorChanged" />
        <a href="/sitecore modules/Shell/CompiledDomainModel/Documentation/default.htm" target="_blank" style="float: right;">CompiledDomainModel documentation</a>
    </div>
    <textarea id="generatedcode" cols="200" rows="20" style="height: 85%; width: 100%;"><asp:PlaceHolder ID="plhGenerator" runat="server" />
    </textarea>
</form>
</body>
</html>
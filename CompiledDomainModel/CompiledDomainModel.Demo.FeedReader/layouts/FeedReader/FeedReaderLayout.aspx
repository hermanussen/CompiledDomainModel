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
<%@ Page Language="C#" Codepage="65001" AutoEventWireup="true" CodeBehind="FeedReaderLayout.aspx.cs" Inherits="CompiledDomainModel.Demo.FeedReader.layouts.FeedReaderLayout" EnableEventValidation="false" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>
<%@ OutputCache Location="None" VaryByParam="none" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html>
  <head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <link rel="stylesheet" type="text/css" href="/layouts/FeedReader/css/FeedReader.css"/>
  </head>
  <body>
      <form method="post" runat="server" id="mainform">
        <sc:Placeholder runat="server" Key="content" />
      </form>
  </body>
</html>
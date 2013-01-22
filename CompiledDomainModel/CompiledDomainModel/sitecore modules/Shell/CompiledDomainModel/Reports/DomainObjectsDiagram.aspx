<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DomainObjectsDiagram.aspx.cs" Inherits="CompiledDomainModel.sitecore_modules.Shell.CompiledDomainModel.Reports.DomainObjectsDiagram" %>
<%@ Register TagPrefix="domainobjects" TagName="Diagram" Src="~/sitecore modules/Shell/CompiledDomainModel/Reports/Diagram.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Structure for loaded domain model</title>
    <style type="text/css">
        div.diagram div
        {
        	margin: 15px;
        	vertical-align: top;
        }
        
        div.diagram div p.template
        {
        	font-weight: bold;
        }
    </style>
</head>
<body>
    <h1>Structure for loaded domain model</h1>
    <div>
        <domainobjects:Diagram runat="server" />
    </div>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FixedPathsDiagram.aspx.cs" Inherits="CompiledDomainModel.sitecore_modules.Shell.CompiledDomainModel.Reports.FixedPathsDiagram" %>
<%@ Register TagPrefix="domainobjects" TagName="FixedPaths" Src="~/sitecore modules/Shell/CompiledDomainModel/Reports/FixedPaths.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Fixed paths overview</title>
    <style type="text/css">
        div.fixedpath
        {
        	margin-left: 20px;
        }
        span.domainobjectname
        {
        	color: Green;
        	font-style: italic;
        	font-weight: bold;
        }
    </style>
</head>
<body>
    <h1>Fixed paths overview</h1>
    <div>
        <domainobjects:FixedPaths runat="server" />
    </div>
</body>
</html>

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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="../DomainModelGenerator.ascx.cs" Inherits="CompiledDomainModel.sitecore_modules.Shell.CompiledDomainModel.DomainModelGenerator" %>
<%@ Import Namespace="CompiledDomainModel.Dom" %>
<%@ Import Namespace="CompiledDomainModel.Utils" %>
<%@ Import Namespace="TemplateField=CompiledDomainModel.Dom.TemplateField" %>
-- Refresh from URL: <%# Request.Url.AbsoluteUri.Contains('?') ? Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.IndexOf('?')) : Request.Url.AbsoluteUri %>?direct=CustomGenerators%5cSqlViewsGenerator
-- First, remove the schema (including all the views in it) if it exists
if exists(select 1 from information_schema.schemata where
schema_name='DomainModelViews')
begin

DECLARE @ViewName varchar(100)

DECLARE my_cursor CURSOR FOR
select TABLE_NAME from INFORMATION_SCHEMA.VIEWS where TABLE_SCHEMA = 'DomainModelViews'

OPEN my_cursor

FETCH NEXT FROM my_cursor
INTO @ViewName

WHILE @@FETCH_STATUS = 0
BEGIN
    EXEC ('drop view DomainModelViews.' + @ViewName);

    FETCH NEXT FROM my_cursor
    INTO @ViewName
END

CLOSE my_cursor
DEALLOCATE my_cursor

EXEC('drop schema DomainModelViews');

end

EXEC('create schema DomainModelViews');
go

-- Helper view that unions all fields
create view DomainModelViews.AllFields
as
SELECT     Id, ItemId, '' AS Language, FieldId, Value, Created, Updated, 0 as Version, NULL as IsLatestVersion
FROM         dbo.SharedFields
UNION ALL
SELECT     Id, ItemId, Language, FieldId, Value, Created, Updated, 0 as Version, NULL as IsLatestVersion
FROM         dbo.UnversionedFields
UNION ALL
SELECT     vf1.Id, vf1.ItemId, vf1.Language, vf1.FieldId, vf1.Value, 
                      vf1.Created, vf1.Updated, vf1.[Version], Case vf1.[Version] when (select max(vf2.Version) from dbo.VersionedFields vf2 where vf2.ItemId = vf1.ItemId and vf1.Language = vf2.Language)  then 'Yes' else 'No' end as IsLatestVersion
FROM         dbo.VersionedFields vf1
go

<asp:Repeater runat="server" DataSource="<%# Settings.AllTemplatesInSets %>">
    <ItemTemplate>
-- <%# ((TemplateInSet) Container.DataItem).TemplateName %> (<%# ((TemplateInSet) Container.DataItem).TemplateId %>)
create view DomainModelViews.<%# ((TemplateInSet) Container.DataItem).ClassName %>
as
select it.ID as ItemID, it.Name as ItemName, it.Created as ItemCreated, it.Updated as ItemUpdated, af.Language as ItemLanguage, af.Version as ItemVersion, af.IsLatestVersion<asp:Repeater runat="server" DataSource="<%# ((TemplateInSet) Container.DataItem).AllFieldsFlat %>">
        <ItemTemplate>,
        (select fl.Value from DomainModelViews.AllFields fl where fl.FieldId = '<%# ((TemplateField)Container.DataItem).FieldId.Trim(new char[] { '{', '}' })%>' and fl.ItemId = it.ID and fl.Language = af.Language and fl.Version = af.Version) as '<%# ((TemplateField)Container.DataItem).FieldName%>'</ItemTemplate>
    </asp:Repeater>
    from Items it
    inner join (select allF.ItemID, allF.Language, allF.Version, allF.IsLatestVersion 
					 from DomainModelViews.AllFields allF
					 where allF.Language != ''
					 group by allF.ItemID, allF.Language, allF.Version, allF.IsLatestVersion) as af on it.ID = af.ItemID
where it.TemplateID = '<%# ((TemplateInSet) Container.DataItem).TemplateId.Trim(new char[] { '{', '}' }) %>' and af.Version > 0
go
    </ItemTemplate>
</asp:Repeater>

<asp:Repeater runat="server" DataSource="<%# Settings.AllContributingTemplatesInSets %>">
    <ItemTemplate>
-- <%# ((ContributingTemplateInSet) Container.DataItem).TemplateName %> (<%# ((ContributingTemplateInSet)Container.DataItem).TemplateId%>)
create view DomainModelViews.<%# ((ContributingTemplateInSet)Container.DataItem).ClassName%>
as
select it.ID as ItemID, it.Name as ItemName, it.Created as ItemCreated, it.Updated as ItemUpdated, af.Language as ItemLanguage, af.Version as ItemVersion, af.IsLatestVersion<asp:Repeater runat="server" DataSource="<%# ((ContributingTemplateInSet) Container.DataItem).AllFields %>">
        <ItemTemplate>,
        (select fl.Value from DomainModelViews.AllFields fl where fl.FieldId = '<%# ((TemplateField)Container.DataItem).FieldId.Trim(new char[] { '{', '}' })%>' and fl.ItemId = it.ID and fl.Language = af.Language and fl.Version = af.Version) as '<%# ((TemplateField)Container.DataItem).FieldName%>'</ItemTemplate>
    </asp:Repeater>
    from Items it
    inner join (select allF.ItemID, allF.Language, allF.Version, allF.IsLatestVersion 
					 from DomainModelViews.AllFields allF
					 where allF.Language != ''
					 group by allF.ItemID, allF.Language, allF.Version, allF.IsLatestVersion) as af on it.ID = af.ItemID
where it.TemplateID = '<%# ((ContributingTemplateInSet)Container.DataItem).TemplateId.Trim(new char[] { '{', '}' })%>' and af.Version > 0
go
    </ItemTemplate>
</asp:Repeater>
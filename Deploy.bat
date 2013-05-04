@echo off
xcopy "CompiledDomainModel\CompiledDomainModel\sitecore modules\*.*" "c:\Demo\SitecoreDemo\Website\sitecore modules\" /s /e /y
xcopy "CompiledDomainModel\CompiledDomainModel\bin\CompiledDomainModel.*" "c:\Demo\SitecoreDemo\Website\bin\" /s /e /y
@echo off
xcopy "CompiledDomainModel\CompiledDomainModel\sitecore modules\*.*" "CompiledDomainModel\CompiledDomainModel.NuGet\wwwroot\sitecore modules\" /s /e /y
xcopy "CompiledDomainModel\CompiledDomainModel\bin\CompiledDomainModel.*" "CompiledDomainModel\CompiledDomainModel.NuGet\wwwroot\bin\" /s /e /y

xcopy "c:\Demo\SitecoreDemo\Data\packages\CompiledDomainModel-1.0.0.0.zip" "CompiledDomainModel\CompiledDomainModel.NuGet\content\" /s /e /y

rem Ensure that the latest version is serialized before running this script

xcopy "c:\Demo\SitecoreDemo\Data\serialization\master\sitecore\templates\CompiledDomainModel\*.*" "CompiledDomainModel\CompiledDomainModel.NuGet\serialization\master\sitecore\templates\CompiledDomainModel\" /s /e /y
xcopy "c:\Demo\SitecoreDemo\Data\serialization\master\sitecore\templates\CompiledDomainModel.item" "CompiledDomainModel\CompiledDomainModel.NuGet\serialization\master\sitecore\templates\" /y

xcopy "c:\Demo\SitecoreDemo\Data\serialization\master\sitecore\system\Modules\CompiledDomainModel.item" "CompiledDomainModel\CompiledDomainModel.NuGet\serialization\master\sitecore\system\Modules\" /y
xcopy "c:\Demo\SitecoreDemo\Data\serialization\master\sitecore\system\Modules\CompiledDomainModel\Settings.item" "CompiledDomainModel\CompiledDomainModel.NuGet\serialization\master\sitecore\system\Modules\CompiledDomainModel\" /y

xcopy "c:\Demo\SitecoreDemo\Data\serialization\core\sitecore\content\Applications\CompiledDomainModel\*.*" "CompiledDomainModel\CompiledDomainModel.NuGet\serialization\core\sitecore\content\Applications\CompiledDomainModel\" /s /e /y
xcopy "c:\Demo\SitecoreDemo\Data\serialization\core\sitecore\content\Applications\CompiledDomainModel.item" "CompiledDomainModel\CompiledDomainModel.NuGet\serialization\core\sitecore\content\Applications\" /y

xcopy "c:\Demo\SitecoreDemo\Data\serialization\core\sitecore\content\Documents and settings\All users\Start menu\Right\Development Tools\CompiledDomainModel Validator.item" "CompiledDomainModel\CompiledDomainModel.NuGet\serialization\core\sitecore\content\Documents and settings\All users\Start menu\Right\Development Tools\" /y
xcopy "c:\Demo\SitecoreDemo\Data\serialization\core\sitecore\content\Documents and settings\All users\Start menu\Right\Development Tools\CompiledDomainModel Code Generator.item" "CompiledDomainModel\CompiledDomainModel.NuGet\serialization\core\sitecore\content\Documents and settings\All users\Start menu\Right\Development Tools\" /y

xcopy "c:\Demo\SitecoreDemo\Data\serialization\core\sitecore\layout\Layouts\CompiledDomainModel\*.*" "CompiledDomainModel\CompiledDomainModel.NuGet\serialization\core\sitecore\layout\Layouts\CompiledDomainModel\" /s /e /y
xcopy "c:\Demo\SitecoreDemo\Data\serialization\core\sitecore\layout\Layouts\CompiledDomainModel.item" "CompiledDomainModel\CompiledDomainModel.NuGet\serialization\core\sitecore\layout\Layouts\" /y

xcopy "c:\Demo\SitecoreDemo\Data\serialization\core\sitecore\content\Applications\Content Editor\Gutters\CDM Configuration.item" "CompiledDomainModel\CompiledDomainModel.NuGet\serialization\core\sitecore\content\Applications\Content Editor\Gutters\" /y
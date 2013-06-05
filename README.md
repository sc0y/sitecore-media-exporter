sitecore-media-exporter
=======================

A Sitecore module to export Media Library items.
------------------------------------------------

1. Install the package.

2. The SharedSource.MediaExporter.config file in the App_Config/Include folder allows you to choose where exported 
zip files will be stored on the file system relative to the Data folder.

3. There will now be an additional button when viewing the contents of a folder in the Media Library to export all 
media items in that folder.

** Note the current version of this module only supports Sitecore installations running on the .NET 4.0 framework. 
This module does not currently support exporting a specific language version of a media item.  Only the current 
version of the default language will be exported.

This module assumes items are stored in the database vs the file system.

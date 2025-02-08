## Getting started

* Install the NuGet package 'DavidHome.Optimizely.MultiSite'
* If you are using Startup.cs, please make sure you are injecting an instance of ``IWebHostEnvironment`` in your constructor. 
* On an instance of type ``IServiceCollection``, call ``AddDavidHomeMultiSiteRouting()``.
* On an instance of type ``IWebHostEnvironment``, call ``AddDavidHomeMultiSiteFileProvider()``.

### Optional

* Install the NuGet package DavidHome.Optimizely.MultiSite.Ui
* Adds category list selection capabilities scoped only for the currently edited site. More information bellow.

## Effects

This library adds a couple of mechanismes to ease the task for developers to more easily isolate the development of features per site. On the bright side, it also allows to share components when needed. Typically under a Optimizely CMS website, every components are entirely shared with each others. One of the biggest challenge is to be able to have a clear separation between two different websites, especially when they are completely different. 

### Frontend assets

You are now able to physically drop your assets under wwwroot within a subfolder. The subfolder **must** be the name of the website configured under Optimizely CMS. Say by example that your site name is "MyExampleSite", well then the structure of the folder will be the following: ``wwwroot/myexamplesite``. 

Assuming you have a file ``myscript.js`` in that folder; if you render a page on your site "MyExampleSite" and include this script in your page, simply use the following: 
```html
<script src="/myscript.js" asp-append-version="true"></script>
```

The use of ``asp-append-version`` is optional, but generally recommended so that cache can be automatically invalidated after a deployment. 

As you can see, we're not including the site name in the provided src attribute, as this is handled in the backend based on the site that is currently being rendered with Optimizely CMS. The file provider will also try to read the same file name in the "actual" file path if the first cannot be read. Ultimately if it doesn't exist in both paths, it will return a classic http 404 error. 

### Backend development

Backend developers are now able to create a completely separated C# library project and develop features for a specific website only in it, including razor views. 

#### Effects on view locations

The following location pattern will be added to your site: 

* 

#### C# Library setup

Make sure the node "Project" has the correct SDK: 
```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
```

Add the following instructions in your .csproj file: 
```xml
<PropertyGroup>
    <AddRazorSupportForMvc>true</AddRazorSupportForMvc>
</PropertyGroup>

<ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
</ItemGroup>
```
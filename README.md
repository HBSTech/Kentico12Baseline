# Kentico12Baseline
Our Kentico 12 Baseline Site, the perfect starting point for your Kentico 12 MVC Site to get up and running right away.

***

# Installation
Install a normal Kentico MVC Site, and hotfix it up to the current Hotfix of this project (12.0.71)
## Install NuGet Packages
On the Kentico Admin (WebApp/Mother) solution, install the following NuGet Packages
1. [DynamicRouting.Kentico](https://www.nuget.org/packages/DynamicRouting.Kentico)
2. [Meeg.Kentico.ContentComponents.Cms.Admin](https://www.nuget.org/packages/Meeg.Kentico.ContentComponents.Cms.Admin/)
3. [RelationshiposExtended](https://www.nuget.org/packages/RelationshipsExtended/)
4. [PageBuilderContainers.Kentico](https://www.nuget.org/packages/PageBuilderContainers.Kentico/)

Optionally install
1. [HBS_CSVImport](https://www.nuget.org/packages/HBS_CSVImport/)
2. [HBS.AutomaticGeneratedUserRoles.Kentico.Mother](https://www.nuget.org/packages/HBS.AutomaticGeneratedUserRoles.Kentico.Mother/)

## Install Site Objects
In your Kentico Admin instance, go to `Sites` - `Import Site or Object` and upload the [Baseline Site](https://github.com/HBSTech/Kentico12Baseline/blob/master/Baseline-Site-Import.zip).  You can import these objects into an existing site or create the site from it.

## Enable Webfarm

Kentico uses Webfarm to sync media file changes, event triggers, and more importantly, Cache dependency touches.  Please go to `Settings - Versioning & Synchronization - Web Farm` and set to `Automatic` (you can also set it to Manual if you wish).  By default, the Web farm names will be the server name (for your admin site) and the Server Name + "`_AutoExternalWeb`" for your MVC site. 

## Replace MVC Solution with Baseline
1. Remove the default MVC site folder and replace it with this repository
2. Copy `AppSettings.template` and rename it `AppSettings.config`, update the empty fields with what matches in your Admin web.config
3. Copy `ConnectionStrings.template` and rename it `ConnectionStrings.config` and update the connection string to point to your database
4. Copy `SessionState.template` and rename it `SessionState.config`
5. Open the MVC Solution
6. Restore Nuget Packages (May have to run `Update-Package -reinstall` in nuget command prompt)
7. Rebuild solution

***


Baseline Items
======================================================================

# Features
1. Bootstrap4 and jQuery included (with Bootstrap Layout Tool)
2. Header, Footer Configurations
3. Main Navigation with SubNav, MegaMenu, and Dynamic Menu capabilities
4. SEO Title/Thumbnail with OG Tag generation
5. Breadcrumb and Side Navigation systems
6. User Management
7. Sitemap.xml Generation
8. Securit Headers
9. Generic Pages
10. Rich Text Editor, Image Widget, Static HTML Widgets, and a couple more
11. Widget Provider to configure what users see what widgets
12. AutoFac Dependency Injection
13. MVCCaching for automatic Caching of IRepository and OutputCacheDependency injection
14. Dynamic Routing for, well dynamic routing.
15. AutoMapper
16. production.GitIgnore for your projects

***
# Tool Explenations
Below will not go through all the systems in the Baseline and explain how they operate.

### [Global.asax.cs](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Global.asax.cs)

Your Global.asax.cs contains the starting point of your application, and it is where most of the core systems are activated. 

### [ApplicationConfig.cs](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/App_Start/ApplicationConfig.cs)

Where various features for Kentico are enabled. 

### [RouteConfig.cs](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/App_Start/RouteConfig.cs)

The Routing priority contains Kentico's Route handler, MVC Route attribute priority, Sitemap and Home Routes, Dynamic Routing, 404, Default Route, and a catch all for the not found.

### [BundleConfig.cs](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/App_Start/Bundles.cs)

Bundles have been split and placed in the _Layout, the ones you will mainly configure are the `RegisterFooterJSBundle`, `RegisterHeaderJSBundle`, and `RegisterHeaderStyleBundle`.

### Dependency Injection (AutoFac)

Dependency Injection may be foreign to some readers as it was to me a while back.  Dependency Injection is when the system automatically "Injects" some dependent code you need.  An example, Is your [UserController](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Controllers/Administrative/AccountController.cs) may need a helper that can get the [current IUserInfo](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Repositories/Interfaces/IUserRepository.cs), [register a user](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Services/Interfaces/IUserService.cs), or do other similar operations.  The [UserController](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Controllers/Administrative/AccountController.cs) requests these classes (in the form of an Interface), and the Dependency Injector finds the current class that implements the interface (like [this class](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Repositories/Implementations/KenticoUserRepository.cs) and [this one](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Services/Implementation/KenticoUserService.cs)) and injects it in.

In our project, we use `AutoFac` for our Dependency Injection, and you can see it registering `IRepository, IService, IOutputCache, IDynamicRouteHelper, and cultureName / lastVersionEnabled` [in this file](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Infrastructure/Caching/Startup/DependencyResolverConfig.cs).

You can also see it registering `AutoMapper`.

### [Views/web.config](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/web.config)

For some of the Kentico features to show properly on the Views, we need to include some default namespaces.  You could do this individually on the view files `@using` statements, but I prefer to put them in the view's [web.config](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/web.config).  You can see mine, please note that mine will have other namespaces of items that we will be adding in later on in this tutorial, you may need to comment out some of these as you go.

### Optional: Security Headers in [Web.config](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Web.config)

It is recommended that many of the Headers that get sent to the browser in a request get modified or stripped out, and security headers be added in.  You can look at the `httpProtocol` element in the web.config on how to perform these to increase your site’s security.

The Master Template ([\_Layout.cshtml](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Shared/_layout.cshtml))
----------------------------------------------------------------------------------------------------------------------------------------------

In Portal Engine (or those familiar with Web Forms), there was the concept of the Master Page.  In MVC, this is instead the Layout of a particular view.  The Layout will contain the wrapper around your page, normally the Header and Footer.  This section we will dive into the Layout and its elements.

### Styles/Javascript

If you take a look at our [\_layout.cshtml](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Shared/_layout.cshtml), you're going to see a lot of things going on to add in the Header/Footer Custom Elements, Styles, and Javascript, as well as Page Builder functionality.  
Going from top to bottom...

*   `@Scripts.Render("~/bundles/jquery")` - Since jQuery is often needs to be available right away, I also add this to the header.
*   `@Scripts.Render("~/bundles/HeaderJS")` - Any additional header JavaScript, keep in mind you should keep header JavaScript to a minimum as it blocks load.
*   `@Scripts.Render("~/bundles/HeaderStyles")` - Any Header CSS style sheets
*   `@RenderSection("head", required: false)` - This is where each view that uses this shared layout can add header elements, such as the meta tags and OG:Tags, or schema.org json+lt scripts
*   `PageBuilderEnabled` section - If Page Builder is enabled, this loads the needed styles, as well as your Edit mode styles (more on this later)
*   `IsEditMode` - Page Builder means that the current request has a DocumentID assigned to it and Page Builder widgets are added, however if you are in Edit Mode, you will need more than just the widget styles.  You will need the styles that build out the interface. 
*   `@Scripts.Render("~/bundles/jqueryval")` - jQuery Validator for MVC
*   `@Scripts.Render("~/bundles/jquery-unobtrusive-ajax")` - Ajax validator
*   `if PageBuilderEnabled` section - Renders Page Builder Scripts, which include scripts needed for the Form Widget
*   `if PageBuilder().EditMode` section - Renders the Froala Rich Text Editor custom configuration.  Otherwise it will just load in the two scripts needed to render the Form widget.
*   `@Scripts.Render("~/bundles/FooterJS")` - Any Footer Javascript files
*   `@RenderSection("bottom", required: false)` - This is where each view that uses this shared layout can add footer elements, such as additional JavaScript libraries needed.

### Header and Footer Content

When it comes to the Header and Footer, you have two options available.  You can hard code the content, since it often does not change frequently, however any updates to the will have to be done by you in the future, and you lose some of the Page Builder personalization features. 

The other option is to give the editors a header and/or footer page with Page Builder zones, and then pull that content into your normal \_layout using the [Partial Widget Page](https://github.com/KenticoDevTrev/PartialWidgetPage).  This is how you leverage the Partial Widget Page:

#### 1\. Generic.Header and Generic.Footer

The first piece is the page types themselves.  We have a special `[Generic.Header](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Models/PageTypes/Header.cs)` and `[Generic.Footer](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Models/PageTypes/Footer.cs)` page type that will give the users the Page Builder items on the content tree to edit the content.

#### 2\. Headerless and Footerless \_Layouts

The next piece is a clone of the normal [\_layout.cshtml](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Shared/_layout.cshtml) is required for the Edit mode of these page types.  We have a [\_layout\_NoHeader.cshtml](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Shared/_layout_NoHeader.cshtml) and [\_layout\_NoFooter.cshtml](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Shared/_layout_NoFooter.cshtml), and as the names indicate, they are just the normal \_layout.cshtml except they do not render the header and footer (and the \_layout\_NoHeader.cshtml's  `RenderBody()` is in the header section).  The reason we do this is because while editing, we still want all of our CSS and Javascript to render, but we don't want the header/footer to render while we are trying to edit it, otherwise you end up with an infinite loop as it tries to render the header with the header.

#### 3\. Special Views

For each page type, we have a view to render it, and what is key is in the layout declaration, we are using the Html Helper `LayoutIfEditMode` method, which means that the content around it will only render if it's being edited by the CMS.  Otherwise, it will only render the view itself (which is just the PageBuilder content).  In this way, when we pull the content into the header/footer areas, it is just that area's HTML.  Here’s the [Header](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Header/Render.cshtml) and Here’s the [Footer](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Shared/PageTypes/Footer.cshtml).

#### 4\. Dynamic Route

Since the rendering of these sections are very simple, we are going to just add two [Dynamic Routes](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Controllers/DynamicRoutes.cs) for them.  Keep in mind you can also have a full Controller/Action and render out the view, to do this you would just put the `/{Controller}/{Action}` for the `@Html.PartialWidgetPage` command and set `PathIsNodeAliasPath` to false.  But for our case, I am simply going to pull in the content by NodeAliasPath.

    @Html.PartialWidgetPage("/Masterpage/Header", PathIsNodeAliasPath: true, stripSession: false)
    @Html.PartialWidgetPage("/Masterpage/Footer", PathIsNodeAliasPath: true, stripSession: false)

#### 5\. Manual Output Cache

Since the ParitalWidgetPage makes a web request to get its content, it is not aware of any Output Caching on the elements it is pulling in, meaning our Header and Footer content will not be automatically added to the output cache.  So, we have a special helper function to manually add the output cache for the header and footer.  This is not ideal of course; I am open to any ideas how to do this otherwise though.

    @{OutputCacheHelper.AddCacheDependency($"node|{OutputCacheHelper.CurrentSiteName()}|/Masterpage/Footer"); }
    @{OutputCacheHelper.AddCacheDependency($"nodes|{OutputCacheHelper.CurrentSiteName()}|Generic.Header|all"); }

***

### Navigation

Navigation systems usually fall into one of two categories: Manual or based on the Content Tree.

Manual navigation tends to be more flexible because many clients want specific control over what items show up on the navigation, especially considering how most navigation bars can only fit so many items before wrapping. 

Content Tree navigation was easier to maintain, you add a page, and instantly it was on the menu, and with the "Show in Menu" property that existed prior to Kentico MVC, it was easy to hide elements from the tree.  However, this field is now hidden and may not come back, so we should not rely on it.

My navigation system is a hybrid.  It is based on Navigation elements on the content tree, that can be either Manual or Automatic, and contain Manual or Dynamic children, or can be a Mega Menu, all based on the configuration.

Having this pre-made for you should take a lot of the development time off your shoulders, so let me explain this system for you.

#### Navigation Page Type

The Navigation Page Type is your manual navigation item.  It has multiple configurations, but the largest of which is if you wish the item itself to be `Manual` (you fill in the link, title, etc.), or `Automatic` (select a page and pull in its `PageMetaData` / Document Name and relative link automatically). 

The next item is in the Additional Configuration if it is a `Mega Menu`.  If this is true, then whatever content you place in the Page Builder (Page tab), it will pull it into the transformation. 

The last section, the `Dynamic` Section, allows the children to be Dynamically generated, this is like a repeater so you should be familiar with selecting your `Path`, `Page Types`, and other settings.  `NodeLevel, NodeOrder` for the `Order by` will mimic the tree structure.

Lastly, if you add `Node Categories` to the navigation items, you can filter what Navigation Elements are pulled in by the category code name (see the [INavigationRepository.GetNavItems()](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Repositories/Interfaces/INavigationRepository.cs)).  This can be useful if you have some elements that you wish to display on the Mobile navigation only vs. the Desktop, you can assign different categories to them and display.

#### [INavigationRepository](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Repositories/Interfaces/INavigationRepository.cs)

The `[INavigationRepository](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Repositories/Interfaces/INavigationRepository.cs)` is the interface that does all the heavy lifting, this is used in the `[HeaderController](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Controllers/Structure/HeaderController.cs)` to render the navigation but be aware you can use this for other things.  It has methods to convert any list of Nodes into a `[HierarchyTreeNode](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Models/Generic/HierarchyTreeNode.cs)`, and has logic for getting Side Navigations (more on those later).

#### Header Views

The last piece is the actual rendering of the navigation.  Included are the basic building blocks that you can adapt to your template. 

*   [RenderNavigation.cshtml](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Header/RenderNavigation.cshtml) - The container for your navigation
*   [RenderNavigationItem.cshtml](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Header/RenderNavigationItem.cshtml) - Rendering for each type of navigation item, be it single, drop down, or mega menu.  This is the "top level" navigation
*   [RenderNavigationDropdownItem.cshtml](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Header/RenderNavigationDropdownItem.cshtml) - 2nd and beyond level rendering.

#### Navigation-less Layout, Navigation View, Dynamic Route Tag for Mega Menus

Just like the Header / Footer Content, the Mega Menu system uses the Partial Widget Page to allow the user to create WYSIWYG content on the Page tab of the navigation item and pull that into the mega menu itself.   It uses a [\_Layout\_Navigation.cshtml](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Shared/_layout_Navigation.cshtml), a [Navigation.cshtml](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Shared/PageTypes/Navigation.cshtml) view, and a [Dynamic Route Attribute](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Controllers/DynamicRoutes.cs).  There is no need to include manual OutputCache calls in the [RenderNavigationItem.cshtml](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Header/RenderNavigation.cshtml) as all the Navigation elements are cached through the `[INavigationRepository](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Repositories/Interfaces/INavigationRepository.cs)` ([in the Implementation of it](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Repositories/Implementations/KenticoNavigationRepository.cs)) system.

#### Custom Cache Clear Event Hook

Lastly since Navigation items may be generated dynamically, a page a navigation item is pointing may update and not trigger the navigation cache to clear.  There is a [custom event hook](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Library/Generic_LoaderModule.cs) that we do both on the Documents, their children if dynamic, and page categories.

***

### MetaData and Title

Using the `ViewBag` is fine for the page title, as it is pretty common practice, and it is what I do in my site.  But when it comes to passing a `MetaData` model (or models), while you can use the `ViewBag`, I would not recommend it.  Instead it is better to have a `MetaData` model, and pass that to a `ParitalView` to render it.  This way you have a strongly typed class, and you can possibly send different types of models to render different types of meta data (such as `JSON+ld` tags).

The hard part about this is then you usually are stuck generating this model for each page type, which can be very tedious (even WITH an `AutoMapper`).  To remedy this, I use [Meeg's ContentComponent](https://github.com/CMeeg/kentico-contrib/tree/master/src/Meeg.Kentico.ContentComponents) system, which allows you to store a serialized model (Page type) in a field of your page.  I have a Page Type called `Page MetaData` which contains the items I need to create my Page Title, OG tags, etc.  I can retrieve this strongly typed model by calling `GetPageTypeComponent<T>("FieldName")` on any `TreeNode` object.  I have a naming convention that the Field must be the Component's ClassName (with period replaced with underscore) for all classes.  If the field does not exist, it returns null which my [partial view handles by itself by](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Shared/Components/PageMetaData.cshtml).  You can see this in action in my [GenericPage.cshtml](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Shared/PageTypes/GenericPage.cshtml) and my [Homepage.cshtml](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Home/Index.cshtml).

If you wish to have other components, simply create a  `Page type` that matches your model for your component, then add a long text field to the Page Types you wish to have this component, using the `Page Type Component` form control to set it.  Then on the MVC side of things, retrieve it and pass it to your Partial View.

***

### Page Content

Lastly, the actual Page Content by the `@RenderBody()` tag in the various Layouts.

Home Page
---------

The Home page is one of the few content pages that requires a little extra configuration.  First is in the [RouteConfig.cs](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/App_Start/RouteConfig.cs), there is a specific route `Home` that maps the base url to the `Home Controller`, that is because the Dynamic Route maps on the path, and empty url maps to the `CMS.Root` (root node) on the content tree, not the Home Page.  This Route also sets the `isHomeRoute = true` which is a trigger for special logic on the `HomeController`.  The `[HomeController](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Controllers/PageTypes/HomeController.cs)` receives this value (default false) which tells the controller "Don’t use the Dynamic Route, just grab the home page" in this case.

Generic Pages
-------------

At this point the only last thing to discuss on Page structure is adding new Pages / Page Types.  You can use Dynamic Routing and new Page Types to link things, along with Kentico’s [PageTemplates](https://docs.kentico.com/k12sp/developing-websites/page-builder-development/developing-page-templates-in-mvc) (Dynamic Routing honors page template usage). The baseline includes the "No Template" template that tells the Dynamic Router to route the page based on it's page type.

***

Secondary Navigation
--------------------

As a sample, on the [GenericPage.cshtml](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Shared/PageTypes/GenericPage.cshtml), I demonstrate showing a Secondary navigation.  This is through the aforementioned `[INavigationRepository](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Repositories/Interfaces/INavigationRepository.cs)`. You can add your own `ActionResults` to the `[NavigationController](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Controllers/Structure/NavigationController.cs)` to produce different SideNavigations.  The `GetSecondaryNavItems` takes a path and some other configurations, you can also use the `GetAncestorPath` method to show a side navigation from an ancestor of the current page.  The rendering views ([here](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Navigation/SecondaryNavigation.cshtml)  and [here](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Navigation/SecondaryNavigationDropdownItem.cshtml)) operate the same as the main navigation.

### Breadcrumbs

You also have a `Breadcrumb` action method that is demonstrated on the same [GenericPage.cshtml](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Shared/PageTypes/GenericPage.cshtml).  This one is much simpler as really a breadcrumb should be the current page, onwards up.  You just call the `Breadcrumbs` Action on the `[NavigationController](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Controllers/Structure/NavigationController.cs)`.  There is also a `DefaultBreadcrumb` you can configure (default uses two localization strings), and you can configure what page types should be included.  The rendering of the View is found [here](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Navigation/Breadcrumbs.cshtml).

There is also a `Breadcrumb JSON+LD` rendering that you can include in the header for extra SEO, again demonstrated in the [GenericPage.cshtml](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Shared/PageTypes/GenericPage.cshtml)

***

Contact Us
----------

For the Contact Us page, I just used the normal Generic Page and the Form Page Builder Widget that comes with Kentico…so nothing special to show there!

***

[Sitemap](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Controllers/Administrative/SiteMapController.cs)
---------------------------------------------------------------------------------------------------------------------------

To generate the sitemap, we need one more nuget package which was not included in the first article, and that is the `[SimpleMvcSitemap](https://www.nuget.org/packages/SimpleMvcSitemap/)` package.  We are only using this for the model it provides.  While at first, I fully used the `SimpleMvCSitemap` system, using it somehow disabled output cache, so I had to do things a little more manually.

The Sitemap system is relatively simple, you use the `ISiteMapRepository` to get `SitemapNodes` from your page content, or manually create them, and then once you have a list of `SitemapNodes`, you call the `GetSitemapXml` and it will do create the markup and set the proper output and encoding.  The [RouteConfig.cs](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/App_Start/RouteConfig.cs) has two mappings that that allow `sitemap.xml` or `googlesitemap.xml` to render, this depends also on a web.config settings on the system.webServer modules node to runAllManagedModulesForAllRequests

    <modules runAllManagedModulesForAllRequests="true">

[Robots.txt](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/robots.txt)
-----------------------------------------------------------------------------------------

For the `Robots.txt` file, you can just create one, there is no need for really any MVC logic unless you, for some reason, want the `Robots.txt` to be dynamic.  Then you can follow a similar pattern to the Sitemap.

***

User Management
---------------

One final piece many sites need is User Management.  I have included an `[AccountController](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Controllers/Administrative/AccountController.cs)` and related views that allow for `[Sign In](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Account/SignIn.cshtml)`, `Sign Out`, `[Register](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Account/Register.cshtml)` (with [Email Verification](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Account/CheckYourEmail.cshtml)), `[Change Password](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Account/ResetPassword.cshtml)`, and `[Reset Forgotten Password](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Views/Account/ForgottenPasswordReset.cshtml)` (Email password reset link).

***

Code Systems
============

Now to briefly touch on various Code Systems this Baseline must help optimize and speed up development.

Dependency Injection (AutoFac)
------------------------------

This was mentioned prior in this article.  Any class in your assembly that inherits the `IRepository` or `IService` will automatically be enabled for Dependency Injection, and any constructor that has the variables `string cultureName` and `bool lastVersionEnabled` will have those values set automatically.  Also, `IMapper` and `IDynamicRouteHelper` are included in the Automatic factoring. 

MVC Caching with IRepository
----------------------------

For Interfaces that retrieve data and inherit `IRepository`, you should follow this pattern when creating your interfaces:

1.  Any logic that retrieves data should be an Interface that inherits `IRepository`, any logic that simply manipulates or performs some action should be an Interface that inherits `IService`
2.  Then create your Implementation of these `Interfaces`, for Repositories, leverage the `MVCCaching` attributes.  Any call made through dependency injection that have `MVCCaching` will automatically cache with the proper dependencies, and the dependency keys will be added to the output cache.
3.  This may require a `Helper` interface so your calls within the implementation are also done through dependency injection.  For example, [KenticoRoleRepository](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Repositories/Implementations/KenticoRoleRepository.cs) (which implements [IRoleRepository](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Repositories/Interfaces/IRoleRepository.cs)) has a helper [IKenticoRoleRepositoryHelper](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Repositories/Interfaces/IKenticoRoleRepositoryHelper.cs) for some of its calls, so it can call it through an `IRepository` interface.

This will ensure that your code is automatically cached.  Definitely recommend reading the [MVCCaching Documentation](https://github.com/KenticoDevTrev/MVCCaching/blob/master/Documentation.md) for all of it’s nuances.  
 

Output Caching, both manual and Automatic
-----------------------------------------

Output caching is automatically done on any `IRepository` public method.  The current page is also added to the Output Cache automatically when you call the `IDynamicRouteHelper.GetPage` method.  You can use Dependency Injection on `[IOutputCacheDependencies](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Infrastructure/Caching/Interfaces/IOutputCacheDependencies.cs)` to manually add dependencies.

AutoMapper
----------

`AutoMapper` is a common tool to Map one model to another.  This is often used when mapping Data models to View Models.  You can include the `IMapper` interface in your constructor, and then use its `Map<TDestination>(SourceObject)`.  You can configure this and add your own mappings in the [AutoMapperMaps.cs](https://github.com/HBSTech/Kentico12Baseline/blob/master/MVC/MVC/Infrastructure/AutoMapper/AutoMapperMaps.cs).

Take it from here
==========================================

At this point you have the starting site structure.  Just take your website HTML / CSS / Javascript and start implementing it, adjust the views to match your navigation, add in your media files, etc.  There may be some specific widgets you will want to create, just follow [Kentico’s Documentation](https://docs.kentico.com/k12sp/developing-websites/page-builder-development/developing-widgets-in-mvc), or specific [Page Templates with properties](https://docs.kentico.com/k12sp/developing-websites/page-builder-development/developing-page-templates-in-mvc).  That part we will cover in more detail in part 3, which gets into building out specific systems and module integration examples.


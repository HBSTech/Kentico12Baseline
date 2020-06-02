using System.Web.Mvc;
using System.Web.Routing;
using KMVCHelper;
using Kentico.Web.Mvc;
using DynamicRouting.Kentico.MVC;

public class RouteConfig
{
    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

        // Maps routes to Kentico HTTP handlers and features enabled in ApplicationConfig.cs
        // Always map the Kentico routes before adding other routes. Issues may occur if Kentico URLs are matched by a general route, for example images might not be displayed on pages
        routes.Kentico().MapRoutes();

        // MVC Routes
        routes.MapMvcAttributeRoutes();

        // Redirect to administration site if the path is "admin"
        routes.MapRoute(
            name: "Admin",
            url: "admin",
            defaults: new { controller = "AdminRedirect", action = "Index" }
        );

        //Site map
        routes.MapRoute(
        name: "MySiteMap",
        url: "sitemap.xml",
        defaults: new { controller = "Sitemap", action = "Index" }
        );

        routes.MapRoute(
        name: "MySiteMap_Google",
        url: "googlesitemap.xml",
        defaults: new { controller = "Sitemap", action = "Index" }
        );

        // Home route is special since normally "/" maps to the CMS.Root node
        routes.MapRoute(
            name: "Home",
            url: "",
            defaults: new { controller = "Home", action = "Index", isHomeRoute = true }
        );

        // If the Page is found, will handle the routing dynamically
        var route = routes.MapRoute(
            name: "DynamicRouting",
            url: "{*url}",
            defaults: new { defaultcontroller = "HttpErrors", defaultaction = "Index" },
            constraints: new { PageFound = new DynamicRouteConstraint() }
        );
        route.RouteHandler = new DynamicRouteHandler();

        // Specific 404 handler
        routes.MapRoute(
            name: "404-PageNotFound",
            url: "404",
            defaults: new { controller = "HttpErrors", action = "Index" }
            );

        // This will again look for matching routes or node alias paths, this time it won't care if the route is priority or not.
        routes.MapRoute(
             name: "Default",
             url: "{controller}/{action}/{id}",
             defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
        );

        // Finally, any unfound should also go to the 404
        routes.MapRoute(
            name: "PageNotFound",
            url: "{*url}",
            defaults: new { controller = "HttpErrors", action = "Index" }
            );
    }
}
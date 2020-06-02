using KMVCHelper;
using System.Web.Optimization;

public class Bundles
{
    /// <summary>
    /// Registers MVC Bundles.
    /// </summary>
    /// <param name="bundles"></param>
    public static void RegisterBundles(BundleCollection bundles)
    {
        RegisterJqueryBundle(bundles);
        RegisterJqueryUnobtrusiveAjaxBundle(bundles);
        RegisterJqueryValidationBundle(bundles);
        RegisterFooterJSBundle(bundles);
        RegisterHeaderJSBundle(bundles);
        RegisterHeaderStyleBundle(bundles);
    }

    private static void RegisterJqueryBundle(BundleCollection bundles)
    {
        bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
             "~/Kentico/Scripts/jquery-3.3.1.js"));
    }

    private static void RegisterJqueryValidationBundle(BundleCollection bundles)
    {
        bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                    "~/Content/js/jquery.validate/jquery.validate-vsdoc.js",
                    "~/Content/js/jquery.validate/jquery.validate.js",
                    "~/Content/js/jquery.validate/jquery.validate.unobtrusive.js"));
    }


    private static void RegisterJqueryUnobtrusiveAjaxBundle(BundleCollection bundles)
    {
        var bundle = new ScriptBundle("~/bundles/jquery-unobtrusive-ajax")
            .Include("~/Kentico/Scripts/jquery.unobtrusive-ajax.js");

        bundles.Add(bundle);
    }

    private static void RegisterFooterJSBundle(BundleCollection bundles)
    {
        var bundle = new ScriptBundle("~/bundles/FooterJS").Include(
                    //"~/Content/js/Custom.js",
                    );
        bundle.Orderer = new NonOrderingBundleOrderer();
        bundles.Add(bundle);
    }
    private static void RegisterHeaderJSBundle(BundleCollection bundles)
    {
        var bundle = new ScriptBundle("~/bundles/HeaderJS").Include(
            "~/Content/js/bootstrap/popper.min.js",
            "~/Content/js/bootstrap/bootstrap.min.js"
        );
        bundle.Orderer = new NonOrderingBundleOrderer();
        bundles.Add(bundle);
    }

    private static void RegisterHeaderStyleBundle(BundleCollection bundles)
    {
        var bundle = new StyleBundle("~/bundles/HeaderStyles").Include(
                    "~/Content/css/bootstrap/bootstrap.min.css",
                    "~/Content/css/Custom.min.css"
                    );
        bundle.Orderer = new NonOrderingBundleOrderer();
        bundles.Add(bundle);
    }
}
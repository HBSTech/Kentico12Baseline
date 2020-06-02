using System.Linq;
using System.Web.Mvc;
using Generic.Repositories.Interfaces;
using DynamicRouting.Interfaces;

namespace Generic.Controllers.PageTypes
{
    public class HeaderController : Controller
    {
        private INavigationRepository NavRepo { get; set; }
        readonly IDynamicRouteHelper _DynamicRouteHelper;

        public HeaderController(INavigationRepository NavigationRepo, IDynamicRouteHelper dynamicRouteHelper)
        {
            NavRepo = NavigationRepo;
            _DynamicRouteHelper = dynamicRouteHelper;
        }

        /// <summary>
        /// Renders the Navigation
        /// </summary>
        public ActionResult RenderNavigation()
        {
            var Page = _DynamicRouteHelper.GetPage(Columns: new string[] { "NodeAliasPath" });
            string PagePath = Page?.NodeAliasPath;
            // Special Case for "root" to be the Home page
            if (Page != null && Page.NodeAliasPath == "/")
            {
                PagePath = "/Home";
            }
            // Use ViewBag to set current Path
            ViewBag.NavigationCurrentPagePath = PagePath;
            var model = NavRepo.GetNavItems("/MasterPage/Navigation").ToList();

            return View(model);
        }

    }
}
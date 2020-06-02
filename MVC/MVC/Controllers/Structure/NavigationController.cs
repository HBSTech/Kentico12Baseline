using DynamicRouting.Interfaces;
using Generic.Models;
using Generic.Repositories.Interfaces;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class NavigationController : Controller
    {
        private INavigationRepository _NavigationRepo { get; set; }
        readonly IDynamicRouteHelper _DynamicRouteHelper;
        readonly IBreadcrumbRepository _BreadcrumbRepo;

        public NavigationController(INavigationRepository NavigationRepo, IDynamicRouteHelper dynamicRouteHelper, IBreadcrumbRepository BreadCrumbRepo)
        {
            _NavigationRepo = NavigationRepo;
            _DynamicRouteHelper = dynamicRouteHelper;
            _BreadcrumbRepo = BreadCrumbRepo;
        }

        /// <summary>
        /// Example of a Secondary Navigation rendering
        /// </summary>
        public ActionResult SecondaryNavigation()
        {
            var Page = _DynamicRouteHelper.GetPage(Columns: new string[] { "NodeAliasPath" });
            var model = _NavigationRepo.GetSecondaryNavItems(_NavigationRepo.GetAncestorPath(Page.NodeAliasPath, 2, false), Enums.PathSelectionEnum.ParentAndChildren);
            return View(model);
        }

        /// <summary>
        /// Breadcrumb Rendering using the current page
        /// </summary>
        public ActionResult Breadcrumbs()
        {
            var Page = _DynamicRouteHelper.GetPage(Columns: new string[] { "NodeID" });
            return View(_BreadcrumbRepo.GetBreadcrumbs(Page.NodeID, true));
        }

        /// <summary>
        /// Breadcrumb JsonLD rendering, use this in Head section
        /// </summary>
        public ActionResult BreadcrumbJsonLD()
        {
            var Page = _DynamicRouteHelper.GetPage(Columns: new string[] { "NodeID" });
            var Model = new BreadcrumbJsonLD(_BreadcrumbRepo.GetBreadcrumbs(Page.NodeID, false));
            Model.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(Model);
            return View(Model);
        }
    }
}
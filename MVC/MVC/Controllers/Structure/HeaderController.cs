using System.Linq;
using System.Web.Mvc;
using CMS.DocumentEngine;
using DynamicRouting.Kentico.MVC;
using Kentico.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Generic.Repositories.Interfaces;
using CMS.DocumentEngine.Types.Generic;
using Generic.ViewModels;
using Generic.Controllers.PageTypes;
using DynamicRouting.Interfaces;

[assembly: DynamicRouting(typeof(HeaderController), new string[] { Header.CLASS_NAME }, nameof(HeaderController.Render))]
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
        /// Renders the Header area
        /// </summary>
        /// <returns></returns>
        public ActionResult Render()
        {
            var Page = _DynamicRouteHelper.GetPage<Header>();
            if (Page == null)
            {
                Page = DocumentHelper.GetDocuments<Header>().FirstOrDefault();
            }

            if (Page == null)
            {
                return HttpNotFound("No header page found.  Site must contain at least 1 header object");
            }

            HttpContext.Kentico().PageBuilder().Initialize(Page.DocumentID);


            HeaderViewModel model = new HeaderViewModel()
            {
                HeaderPage = Page
            };

            return View(model);
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
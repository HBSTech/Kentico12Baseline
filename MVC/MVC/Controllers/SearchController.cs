using CMS.Helpers;
using CMS.Membership;
using CMS.Search;
using CMS.WebAnalytics;
using Generic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class SearchController : Controller
    {
        private readonly IPagesActivityLogger mPagesActivityLogger;

        public SearchController(IPagesActivityLogger pagesActivityLogger)
        {
            mPagesActivityLogger = pagesActivityLogger;
        }
        // GET: Search
        public ActionResult Index(string SearchValue = null)
        {
            SearchValue = ValidationHelper.GetString(SearchValue, "");
            SearchViewModel Model = new SearchViewModel()
            {
                SearchValue = SearchValue
            };
            if (!string.IsNullOrWhiteSpace(SearchValue))
            {
                var searchParameters = SearchParameters.PrepareForPages(SearchValue, new[] { "SiteSearch" }, 1, 100, MembershipContext.AuthenticatedUser);
                var Search = SearchHelper.Search(searchParameters);
                Model.SearchItems = Search.Items;
                mPagesActivityLogger.LogInternalSearch(SearchValue);
            }
            return View(Model);
        }

    }
}
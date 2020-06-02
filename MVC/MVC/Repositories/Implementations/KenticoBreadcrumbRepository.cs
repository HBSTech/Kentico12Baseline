using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using Generic.Models;
using Generic.Repositories.Helpers.Interfaces;
using Generic.Repositories.Interfaces;
using MVCCaching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Generic.Repositories.Implementations
{
    public class KenticoBreadcrumbRepository : IBreadcrumbRepository, IRepository
    {
        public string cultureName { get; set; }
        public bool latestVersionEnabled { get; set; }
        private IKenticoBreadcrumbRepositoryHelper _Helper;
        public KenticoBreadcrumbRepository(string cultureName, bool latestVersionEnabled, IKenticoBreadcrumbRepositoryHelper Helper)
        {
            this.cultureName = cultureName;
            this.latestVersionEnabled = latestVersionEnabled;
            _Helper = Helper;
        }

        /// <summary>
        /// Gets the Breadcrumbs of the given page.
        /// </summary>
        /// <param name="PageIdentifier"></param>
        /// <param name="TopLevelBreadcrumb"></param>
        /// <returns></returns>
        [CacheDependency("node|##SITENAME##|/|childnodes")]
        public List<Breadcrumb> GetBreadcrumbs(int PageIdentifier, bool IncludeDefaultBreadcrumb = true)
        {
            string[] ValidClassNames = SettingsKeyInfoProvider.GetValue(new SettingsKeyName("BreadcrumbPageTypes", new SiteInfoIdentifier(SiteContext.CurrentSiteID))).ToLower().Split(";,|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            List<Breadcrumb> Breadcrumbs = new List<Breadcrumb>();

            // Get the current Page and then loop through ancestors
            var Page = _Helper.GetBreadcrumbNode(PageIdentifier);
            bool IsCurrentPage = true;
            while (Page != null && !Page.ClassName.Equals("CMS.Root", StringComparison.InvariantCultureIgnoreCase))
            {
                if (ValidClassNames.Length == 0 || ValidClassNames.Contains(Page.ClassName.ToLower()))
                {
                    Breadcrumbs.Add(_Helper.PageToBreadcrumb(Page, IsCurrentPage));
                }
                Page = _Helper.GetBreadcrumbNode(Page.NodeParentID);
                IsCurrentPage = false;
            }

            // Add given Top Level Breadcrumb if provided
            if (IncludeDefaultBreadcrumb)
            {
                Breadcrumbs.Add(DependencyResolver.Current.GetService<IBreadcrumbRepository>().GetDefaultBreadcrumb());
            }
            // Reverse breadcrumb order
            Breadcrumbs.Reverse();
            return Breadcrumbs;
        }

        [CacheDependency("cms.resourcestring|byname|generic.default.breadcrumbtext")]
        [CacheDependency("cms.resourcestring|byname|generic.default.breadcrumburl")]
        public Breadcrumb GetDefaultBreadcrumb()
        {
            return new Breadcrumb()
            {
                LinkText = ResHelper.LocalizeExpression("generic.default.breadcrumbtext"),
                LinkUrl = ResHelper.LocalizeExpression("generic.default.breadcrumburl"),
            };
        }
    }
}
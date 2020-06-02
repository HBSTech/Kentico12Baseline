using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using DynamicRouting.Interfaces;
using Generic.Models;
using Generic.Repositories.Helpers.Interfaces;
using Generic.Repositories.Interfaces;
using MVCCaching;
using SimpleMvcSitemap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Generic.Repositories.Implementations
{
    public class KenticoSiteMapRepository : ISiteMapRepository
    {
        public string cultureName { get; set; }
        public bool latestVersionEnabled { get; set; }
        readonly IDynamicRouteHelper _DynamicRouteHelper;
        private readonly IKenticoSiteMapRepositoryHelper _Helper;

        public KenticoSiteMapRepository(string cultureName, bool latestVersionEnabled, IDynamicRouteHelper dynamicRouteHelper, IKenticoSiteMapRepositoryHelper Helper)
        {
            this.cultureName = cultureName;
            this.latestVersionEnabled = latestVersionEnabled;
            _DynamicRouteHelper = dynamicRouteHelper;
            _Helper = Helper;
        }

        [DoNotCache]
        public IEnumerable<SitemapNode> GetSiteMapUrlSet(SiteMapOptions Options)
        {
            // Clean up
            Options.Path = DataHelper.GetNotEmpty(Options.Path, "/").Replace("%", "");
            Options.SiteName = DataHelper.GetNotEmpty(Options.SiteName, SiteContext.CurrentSiteName);
            List<SitemapNode> Nodes = new List<SitemapNode>();

            if (Options.ClassNames != null && Options.ClassNames.Count() > 0)
            {
                foreach (string ClassName in Options.ClassNames)
                {
                    if (string.IsNullOrWhiteSpace(Options.UrlColumnName))
                    {
                        Nodes.AddRange(_Helper.GetSiteMapUrlSetForClass(Options.Path, ClassName, Options.SiteName, Options));
                    }
                    else
                    {
                        // Since it's not the specific node, but the page found at that url that we need, we will first get the urls, then cache on getting those items.
                        Nodes.AddRange(GetSiteMapUrlSetForClassWithUrlColumn(Options.Path, ClassName, Options.SiteName, Options));
                    }
                }
            }
            else
            {
                Nodes.AddRange(_Helper.GetSiteMapUrlSetForAllClass(Options.Path, Options.SiteName, Options));
            }

            // Clean up, remove any that are not a URL
            Nodes.RemoveAll(x => !Uri.IsWellFormedUriString(x.Url, UriKind.Absolute));
            return Nodes;
        }

        /// <summary>
        /// Gets the SitemapNodes, looking up the page they point to automatically to get the accurate Document last modified.
        /// </summary>
        /// <param name="Path">The parent Nodealiaspath</param>
        /// <param name="ClassName">The Class Name to query</param>
        /// <param name="SiteName">The SiteName</param>
        /// <param name="Options">The Options</param>
        /// <returns>The SitemapNodes</returns>
        private IEnumerable<SitemapNode> GetSiteMapUrlSetForClassWithUrlColumn(string Path, string ClassName, string SiteName, SiteMapOptions Options)
        {
            var DocumentQuery = _Helper.GetDocumentQuery(Path, Options, ClassName);
            List<SitemapNode> SiteMapItems = new List<SitemapNode>();
            foreach (string RelativeUrl in _Helper.GetRelativeUrls(Path, ClassName, SiteName, Options))
            {
                // Get the page, Dynamic Routing already has it's own internal Caching and adds to the output cache
                var Page = (TreeNode)_DynamicRouteHelper.GetPage(RelativeUrl, Options.CultureCode, SiteName, new string[] { "DocumentModifiedWhen", "NodeID", "DocumentCulture" }, true);
                if (Page != null)
                {
                    SiteMapItems.Add(ConvertToSiteMapUrl(Page.RelativeURL, SiteName, Page.DocumentModifiedWhen));
                }
                else
                {
                    SiteMapItems.Add(ConvertToSiteMapUrl(RelativeUrl, SiteName, null));
                }
            }
            return SiteMapItems;
        }

        /// <summary>
        /// Converts the realtive Url and possible Datetime into an SitemapNode with an absolute Url
        /// </summary>
        /// <param name="RelativeURL">The Relative Url</param>
        /// <param name="SiteName">The SiteName</param>
        /// <param name="ModifiedLast">The last modified date</param>
        /// <returns>The SitemapNode</returns>
        private SitemapNode ConvertToSiteMapUrl(string RelativeURL, string SiteName, DateTime? ModifiedLast)
        {
            string Url = URLHelper.GetAbsoluteUrl(RelativeURL, DocumentURLProvider.EnsureDomainPrefix(RequestContext.CurrentDomain, SiteName));
            SitemapNode SiteMapItem = new SitemapNode(Url);
            if (ModifiedLast.HasValue)
            {
                SiteMapItem.LastModificationDate = ModifiedLast;
            }
            return SiteMapItem;
        }
    }
}
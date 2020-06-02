using AutoMapper;
using CMS.DocumentEngine;
using Generic.Models;
using Generic.Repositories.Helpers.Interfaces;
using MVCCaching;
using System.Linq;

namespace Generic.Repositories.Helpers.Implementations
{
    public class KenticoBreadcrumbRepositoryHelper : IKenticoBreadcrumbRepositoryHelper
    {
        public string cultureName { get; set; }
        public bool latestVersionEnabled { get; set; }

        private IMapper _Mapper;

        public KenticoBreadcrumbRepositoryHelper(string cultureName, bool latestVersionEnabled, IMapper Mapper)
        {
            this.cultureName = cultureName;
            this.latestVersionEnabled = latestVersionEnabled;
            _Mapper = Mapper;
        }

        [CacheDependency("Nodeid|{0}")]
        public TreeNode GetBreadcrumbNode(int NodeID)
        {
            return DocumentHelper.GetDocuments()
                .WhereEquals("NodeID", NodeID)
                .Culture(cultureName)
                .CombineWithDefaultCulture()
                .CombineWithAnyCulture()
                .Published(!latestVersionEnabled)
                .LatestVersion(latestVersionEnabled)
                .Columns("NodeID", "DocumentCulture", "DocumentName", "NodeParentID", "ClassName") // These are needed for Dynamic Route URL Slug lookup for Relative Url, and breadcrumb determination
                .FirstOrDefault();
        }

        /// <summary>
        /// Converts the TreeNode into a breadcrumb.
        /// </summary>
        /// <param name="Page">The Page</param>
        /// <param name="IsCurrentPage">If the page is the current page</param>
        /// <returns></returns>
        public Breadcrumb PageToBreadcrumb(TreeNode Page, bool IsCurrentPage)
        {
            Breadcrumb breadcrumb = _Mapper.Map<Breadcrumb>(Page);
            breadcrumb.IsCurrentPage = IsCurrentPage;
            return breadcrumb;
        }
    }
}
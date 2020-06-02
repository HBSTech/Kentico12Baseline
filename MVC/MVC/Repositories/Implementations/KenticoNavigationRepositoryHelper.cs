using AutoMapper;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.Generic;
using Generic.Enums;
using Generic.Models;
using Generic.Repositories.Helpers.Interfaces;
using MVCCaching;
using RelationshipsExtended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Generic.Repositories.Helpers.Implementations
{
    public class KenticoNavigationRepositoryHelper : IKenticoNavigationRepositoryHelper
    {
        public string cultureName { get; set; }
        public bool latestVersionEnabled { get; set; }

        private IMapper _Mapper;

        public KenticoNavigationRepositoryHelper(string cultureName, bool latestVersionEnabled, IMapper Mapper)
        {
            this.cultureName = cultureName;
            this.latestVersionEnabled = latestVersionEnabled;
            _Mapper = Mapper;
        }

        public NavigationItem GetTreeNodeToNavigationItem(HierarchyTreeNode HierarchyNavTreeNode)
        {
            IKenticoNavigationRepositoryHelper _CachableSelfHelper = DependencyResolver.Current.GetService<IKenticoNavigationRepositoryHelper>();
            NavigationItem NavItem = new NavigationItem();
            if (HierarchyNavTreeNode.Page is Navigation)
            {
                Navigation NavTreeNode = (Navigation)HierarchyNavTreeNode.Page;
                switch ((NavigationTypeEnum)(NavTreeNode.NavigationType))
                {
                    case NavigationTypeEnum.Automatic:
                        if(NavTreeNode.NavigationPageNodeGuid != Guid.Empty) { 
                            var TempNavItem = _CachableSelfHelper.GetTreeNodeToNav(NavTreeNode.NavigationPageNodeGuid);
                            // Convert to a new navigation item so it's not linked to the cached memory object, specifically the Children List
                            NavItem = _Mapper.Map<NavigationItem>(TempNavItem);
                        } else
                        {
                            NavItem.LinkText = NavTreeNode.NavigationLinkText;
                            NavItem.LinkTarget = NavTreeNode.NavigationLinkTarget;
                            NavItem.LinkHref = NavTreeNode.NavigationLinkUrl;
                        }
                        break;
                    case NavigationTypeEnum.Manual:
                    default:
                        NavItem.LinkText = NavTreeNode.NavigationLinkText;
                        NavItem.LinkTarget = NavTreeNode.NavigationLinkTarget;
                        NavItem.LinkHref = NavTreeNode.NavigationLinkUrl;
                        break;
                }
                // Add additional items
                NavItem.IsMegaMenu = NavTreeNode.NavigationIsMegaMenu;
                NavItem.LinkCSSClass = NavTreeNode.NavigationLinkCSS;
                NavItem.LinkOnClick = NavTreeNode.NavigationLinkOnClick;
                NavItem.LinkAlt = NavTreeNode.NavigationLinkAlt;
                NavItem.LinkPagePath = NavTreeNode.NodeAliasPath;

               
            } else
            {
                // Create navigation item from page manually
                NavItem = _Mapper.Map<NavigationItem>(HierarchyNavTreeNode.Page);
            }

            // Add children
            foreach (var HierarchyChild in HierarchyNavTreeNode.Children)
            {
                NavItem.Children.Add(GetTreeNodeToNavigationItem(HierarchyChild));
            }
            return NavItem;
        }

        [CacheDependency("node|##SITENAME##|{0}")]
        public NavigationItem GetTreeNodeToNav(Guid linkPageIdentifier)
        {
            var DocQuery = DocumentHelper.GetDocuments()
                        .WhereEquals("NodeGuid", linkPageIdentifier)
                        .Culture(cultureName)
                        .CombineWithDefaultCulture()
                        .CombineWithAnyCulture()
                        .OnCurrentSite()
                        .Columns("DocumentName", "ClassName", "DocumentCulture", "NodeID", "NodeGuid", "NodeAliasPath");
            if (latestVersionEnabled)
            {
                DocQuery.Published();
            }
            else
            {
                DocQuery.LatestVersion(true);
                DocQuery.Published(false);
            }
            var TreeItem = DocQuery.FirstOrDefault();
            if (TreeItem == null)
            {
                return null;
            }

            return _Mapper.Map<NavigationItem>(TreeItem);
        }

        [CacheDependency("nodes|##SITENAME##|generic.navigation|all")]
        // This cache key is manually triggered upon Navigation Category changes or Page updates to pages the navigation points to
        [CacheDependency("CustomNavigationClearKey")]
        public IEnumerable<Navigation> GetNavigationItems(string NavPath, string[] NavTypes)
        {
            var NavigationItems = DocumentHelper.GetDocuments<Navigation>()
           .OrderBy("NodeLevel, NodeOrder")
           .Culture(cultureName)
           .CombineWithDefaultCulture()
           .CombineWithAnyCulture()
           .Published(!latestVersionEnabled)
           .LatestVersion(latestVersionEnabled)
           .Columns(new string[] {
                "NodeParentID", "NodeID", "NavigationType", "NavigationPageNodeGuid", "NavigationLinkText", "NavigationLinkTarget", "NavigationLinkUrl",
                "NavigationIsMegaMenu", "NavigationLinkCSS", "NavigationLinkOnClick", "NavigationLinkAlt", "NodeAliasPath",
                "IsDynamic", "Path", "PageTypes", "OrderBy", "WhereCondition", "MaxLevel", "TopNumber"
           });

            if (!string.IsNullOrWhiteSpace(NavPath))
            {
                NavigationItems.Path(NavPath.Trim('%'), PathTypeEnum.Section);
            }

            // Handle Nav Type with Categories found
            if (NavTypes != null && NavTypes.Length > 0)
            {
                NavigationItems.Where(RelHelper.GetNodeCategoryWhere(NavTypes));
            }
            return NavigationItems.TypedResult;
        }
    }
}
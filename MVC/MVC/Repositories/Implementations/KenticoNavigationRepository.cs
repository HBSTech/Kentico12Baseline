using CMS.Base;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.Generic;
using Generic.Enums;
using Generic.Models;
using Generic.Repositories.Helpers.Interfaces;
using Generic.Repositories.Interfaces;
using MVCCaching;
using MVCCaching.Kentico;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Generic.Repositories.Implementations
{
    public class KenticoNavigationRepository : INavigationRepository
    {
        public string cultureName { get; set; }
        public bool latestVersionEnabled { get; set; }
        public IOutputCacheDependencies cacheDependencies { get; set; }

        private IKenticoNavigationRepositoryHelper _helper;
        private IGeneralDocumentRepository _GeneralDocumentRepo;

        public KenticoNavigationRepository(string cultureName, bool latestVersionEnabled, IOutputCacheDependencies cacheDependencies, IGeneralDocumentRepository GeneralDocumentRepository, IKenticoNavigationRepositoryHelper Helper)
        {
            this.cultureName = cultureName;
            this.latestVersionEnabled = latestVersionEnabled;
            this.cacheDependencies = cacheDependencies;
            _GeneralDocumentRepo = GeneralDocumentRepository;
            _helper = Helper;
        }

        /// <summary>
        /// Converts a list of TreeNodes into a HierarchyTreeNode, putting any children in the Children Property
        /// </summary>
        /// <param name="Nodes">The list of TreeNodes</param>
        /// <returns>List of HierarchyTreeNodes</returns>
        public List<HierarchyTreeNode> NodeListToHierarchyTreeNodes(IEnumerable<ITreeNode> Nodes)
        {
            List<HierarchyTreeNode> HierarchyNodes = new List<HierarchyTreeNode>();

            Dictionary<int, HierarchyTreeNode> NodeIDToHierarchyTreeNode = new Dictionary<int, HierarchyTreeNode>();
            List<TreeNode> NewNodeList = new List<TreeNode>();

            // populate ParentNodeIDToTreeNode
            foreach (TreeNode Node in Nodes)
            {
                NodeIDToHierarchyTreeNode.Add(Node.NodeID, new HierarchyTreeNode(Node));
                NewNodeList.Add(Node);

                // Special Handling of Navigation Items
                if (Node is Navigation)
                {
                    // Get dynamic items below this, and add them to the NodeIDToHierarchyTreeNode
                    Navigation NavItem = (Navigation)Node;

                    if (NavItem.IsDynamic)
                    {
                        // Build NodeIDtoHierarchyTreeNode from the underneath
                        var DynamicNodes = _GeneralDocumentRepo.GetDocuments(
                            "/" + NavItem.Path.Trim('%').Trim('/'),
                            PathSelectionEnum.ChildrenOnly,
                            NavItem.PageTypes.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                            NavItem.OrderBy,
                            NavItem.WhereCondition,
                            NavItem.GetValue("MaxLevel") == null ? -1 : NavItem.MaxLevel,
                            NavItem.GetValue("TopNumber") == null ? -1 : NavItem.TopNumber,
                            new string[] { "DocumentName", "ClassName", "DocumentCulture", "NodeID", "NodeParentID", "NodeLevel", "NodeGuid", "NodeAliasPath" }
                            )
                            .Select(x => (TreeNode)x);

                        if (DynamicNodes.Count() > 0)
                        {
                            int MinNodeLevel = DynamicNodes.Min(x => x.NodeLevel);
                            foreach (TreeNode DynamicNode in DynamicNodes)
                            {
                                // Change the "Parent" to be this navigation node
                                if (DynamicNode.NodeLevel == MinNodeLevel)
                                {
                                    DynamicNode.NodeParentID = NavItem.NodeID;
                                }
                                NodeIDToHierarchyTreeNode.Add(DynamicNode.NodeID, new HierarchyTreeNode(DynamicNode));
                                NewNodeList.Add(DynamicNode);
                            }
                        }
                    }
                }

            }

            // Populate the Children of the TypedResults
            foreach (TreeNode Node in NewNodeList)
            {
                // If no parent exists, add to top level
                if (!NodeIDToHierarchyTreeNode.ContainsKey(Node.NodeParentID))
                {
                    HierarchyNodes.Add(NodeIDToHierarchyTreeNode[Node.NodeID]);
                }
                else
                {
                    // Otherwise, add to the parent element.
                    NodeIDToHierarchyTreeNode[Node.NodeParentID].Children.Add(NodeIDToHierarchyTreeNode[Node.NodeID]);
                }
            }
            return HierarchyNodes;
        }

        [DoNotCache]
        public IEnumerable<NavigationItem> GetNavItems(string NavPath, string[] NavTypes = null)
        {
            var NavigationItems = _helper.GetNavigationItems(NavPath, NavTypes);

            // Convert to a Hierarchy listing
            var HierarchyItems = NodeListToHierarchyTreeNodes(NavigationItems);

            // Convert to Model
            List<NavigationItem> Items = new List<NavigationItem>();
            foreach (var HierarchyNavTreeNode in HierarchyItems)
            {
                // Call the check to set the Ancestor is current
                Items.Add(_helper.GetTreeNodeToNavigationItem(HierarchyNavTreeNode));
            }
            return Items;
        }

        [CacheDependency("node|##SITENAME##|{0}")]
        public string GetNavMegaMenuContent(string NodeAliasPath)
        {
            return PartialWidgetPageExtensions.PartialWidgetPage(null, NodeAliasPath, PathIsNodeAliasPath: true, stripSession: false).ToHtmlString();
        }

        [DoNotCache]
        public IEnumerable<NavigationItem> GetSecondaryNavItems(string StartingPath, PathSelectionEnum PathType = PathSelectionEnum.ChildrenOnly, string[] PageTypes = null, string OrderBy = null, string WhereCondition = null, int MaxLevel = -1, int TopNumber = -1)
        {
            List<HierarchyTreeNode> HierarchyNodes = new List<HierarchyTreeNode>();

            Dictionary<int, HierarchyTreeNode> NodeIDToHierarchyTreeNode = new Dictionary<int, HierarchyTreeNode>();
            List<TreeNode> NewNodeList = new List<TreeNode>();
            IEnumerable<TreeNode> Nodes = _GeneralDocumentRepo.GetDocuments(
                            TreePathUtils.EnsureSingleNodePath(StartingPath),
                            PathType,
                            PageTypes,
                            OrderBy,
                            WhereCondition,
                            MaxLevel,
                            TopNumber,
                            new string[] { "DocumentName", "ClassName", "DocumentCulture", "NodeID", "NodeParentID", "NodeLevel", "NodeGuid", "NodeAliasPath" }
                            )
                            .Select(x => (TreeNode)x);

            // populate ParentNodeIDToTreeNode
            foreach (TreeNode Node in Nodes)
            {
                NodeIDToHierarchyTreeNode.Add(Node.NodeID, new HierarchyTreeNode(Node));
                NewNodeList.Add(Node);

            }

            // Populate the Children of the TypedResults
            foreach (TreeNode Node in NewNodeList)
            {
                // If no parent exists, add to top level
                if (!NodeIDToHierarchyTreeNode.ContainsKey(Node.NodeParentID))
                {
                    HierarchyNodes.Add(NodeIDToHierarchyTreeNode[Node.NodeID]);
                }
                else
                {
                    // Otherwise, add to the parent element.
                    NodeIDToHierarchyTreeNode[Node.NodeParentID].Children.Add(NodeIDToHierarchyTreeNode[Node.NodeID]);
                }
            }

            // Convert to Model
            List<NavigationItem> Items = new List<NavigationItem>();
            foreach (var HierarchyNavTreeNode in HierarchyNodes)
            {
                // Call the check to set the Ancestor is current
                Items.Add(_helper.GetTreeNodeToNavigationItem(HierarchyNavTreeNode));
            }
            return Items;
        }

        [DoNotCache]
        public string GetAncestorPath(string Path, int Levels, bool LevelIsRelative = true, int MinAbsoluteLevel = 2)
        {
            string[] PathParts = TreePathUtils.EnsureSingleNodePath(Path).Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (LevelIsRelative)
            {
                // Handle minimum absolute level
                if((PathParts.Length - Levels+1) < MinAbsoluteLevel)
                {
                    Levels -= MinAbsoluteLevel - (PathParts.Length - Levels+1);
                }
                return TreePathUtils.GetParentPath(TreePathUtils.EnsureSingleNodePath(Path), Levels);
            }

            // Since first 'item' in path is actually level 2, reduce level by 1 to match counts
            Levels--;
            if (PathParts.Count() > Levels)
            {
                return "/" + string.Join("/", PathParts.Take(Levels));
            }
            else
            {
                return TreePathUtils.EnsureSingleNodePath(Path);
            }
        }

        [DoNotCache]
        public string GetAncestorPath(Guid NodeGuid, int Levels, bool LevelIsRelative = true, int MinAbsoluteLevel = 2)
        {
            return GetAncestorPath(_GeneralDocumentRepo.GetDocument(NodeGuid, Columns: new string[] { "NodeAliasPath" }).NodeAliasPath, Levels, LevelIsRelative);
        }

        [DoNotCache]
        public string GetAncestorPath(int NodeID, int Levels, bool LevelIsRelative = true, int MinAbsoluteLevel = 2)
        {
            return GetAncestorPath(_GeneralDocumentRepo.GetDocument(NodeID, Columns: new string[] { "NodeAliasPath" }).NodeAliasPath, Levels, LevelIsRelative);
        }
    }
}
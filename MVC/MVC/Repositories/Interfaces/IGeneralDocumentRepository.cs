using CMS.Base;
using Generic.Enums;
using MVCCaching;
using System;
using System.Collections.Generic;

namespace Generic.Repositories.Interfaces
{
    public interface IGeneralDocumentRepository : IRepository
    {
        /// <summary>
        /// Gets Tree Nodes based on the settings
        /// </summary>
        /// <param name="SinglePath">The Page or Parent Path, should not contain wildcards such as %</param>
        /// <param name="PathType">The Path Type</param>
        /// <param name="PageTypes">The Page Types (Class Names)</param>
        /// <param name="OrderBy">Order by, for Kentico NodeLevel, NodeOrder will follow the tree structure</param>
        /// <param name="WhereCondition">The Where Condition, note that if are you selecting multiple page types, you should limit the where condition to only fields shared by them.</param>
        /// <param name="MaxLevel">Max nesting level of the pages you wish to select</param>
        /// <param name="TopNumber">The Top number of items that you wish to select</param>
        /// <param name="Columns">The Columns you wish to retrieve</param>
        /// <param name="IncludeCoupledColumns">If you wish to return extra column data that isn't shared, this should be set to true for multiple page types retrieved if you need those columns.</param>
        /// <returns>The ITreeNodes</returns>
        IEnumerable<ITreeNode> GetDocuments(string SinglePath, PathSelectionEnum PathType, string[] PageTypes = null, string OrderBy = null, string WhereCondition = null, int MaxLevel = -1, int TopNumber = -1, string[] Columns = null, bool IncludeCoupledColumns = false);

        /// <summary>
        /// Gets the given Node based on it's Guid
        /// </summary>
        /// <param name="NodeGuid">The Node Guid</param>
        /// <param name="PageType">The Classes Page type</param>
        /// <param name="Columns">Columns you wish to retrieve</param>
        /// <returns>The Node</returns>
        ITreeNode GetDocument(Guid NodeGuid, string PageType = null, string[] Columns = null);

        /// <summary>
        /// Gets the given Node based on it's ID
        /// </summary>
        /// <param name="NodeGuid">The Node ID</param>
        /// <param name="PageType">The Classes Page type</param>
        /// <param name="Columns">Columns you wish to retrieve</param>
        /// <returns>The Node</returns>
        ITreeNode GetDocument(int NodeID, string PageType = null, string[] Columns = null);
    }
}
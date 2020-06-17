using CMS.Base;
using CMS.DocumentEngine;
using Generic.Enums;
using Generic.Repositories.Interfaces;
using MVCCaching;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Generic.Repositories.Implementations
{
    public class KenticoGeneralDocumentRepository : IGeneralDocumentRepository
    {
        private ISiteRepository _SiteRepo;

        public string cultureName { get; set; }
        public bool latestVersionEnabled { get; set; }

        public KenticoGeneralDocumentRepository(string cultureName, bool latestVersionEnabled, ISiteRepository SiteRepo)
        {
            this.cultureName = cultureName;
            this.latestVersionEnabled = latestVersionEnabled;
            _SiteRepo = SiteRepo;
        }

        [CacheDependency("node|##SITENAME##|{0}|childnodes")]
        [CacheDependency("node|##SITENAME##|{0}")]
        public IEnumerable<ITreeNode> GetDocuments(string SinglePath, PathSelectionEnum PathType, string[] PageTypes = null, string OrderBy = null, string WhereCondition = null, int MaxLevel = -1, int TopNumber = -1, string[] Columns = null, bool IncludeCoupledColumns = false)
        {
            using (MultiDocumentQuery Query = new MultiDocumentQuery())
            {
                if (PageTypes != null && PageTypes.Length > 0)
                {
                    if (PageTypes.Length == 1)
                    {
                        Query.Type(PageTypes[0]);
                    }
                    else
                    {
                        Query.Types(PageTypes);
                    }
                }
                if (IncludeCoupledColumns)
                {
                    Query.ExpandColumns();
                }

                // Handle culture and versioning and site
                Query.Culture(cultureName)
                    .CombineWithDefaultCulture()
                    .CombineWithAnyCulture()
                    .Published(!latestVersionEnabled)
                    .LatestVersion(latestVersionEnabled)
                    .OnSite(_SiteRepo.CurrentSiteName());

                PathTypeEnum KenticoPathType = PathTypeEnum.Explicit;
                switch (PathType)
                {
                    case PathSelectionEnum.ChildrenOnly:
                        KenticoPathType = PathTypeEnum.Children;
                        break;
                    case PathSelectionEnum.ParentAndChildren:
                        KenticoPathType = PathTypeEnum.Section;
                        break;
                    case PathSelectionEnum.ParentOnly:
                        KenticoPathType = PathTypeEnum.Single;
                        break;
                }
                Query.Path(SinglePath, KenticoPathType);

                if (!string.IsNullOrWhiteSpace(OrderBy))
                {
                    Query.OrderBy(OrderBy);
                }
                if (!string.IsNullOrWhiteSpace(WhereCondition))
                {
                    Query.Where(WhereCondition);
                }
                if (Columns != null && Columns.Length > 0)
                {
                    Query.Columns(Columns);
                }
                if (MaxLevel >= 0)
                {
                    Query.NestingLevel(MaxLevel);
                }
                if (TopNumber >= 0)
                {
                    Query.TopN(TopNumber);
                }
                return Query.TypedResult;
            }
        }

        [CacheDependency("nodeguid|##SITENAME##|{0}")]
        public ITreeNode GetDocument(Guid NodeGuid, string PageType = null, string[] Columns = null)
        {
            DocumentQuery Query = (!string.IsNullOrWhiteSpace(PageType) ? new DocumentQuery(PageType) : new DocumentQuery());
            Query.Culture(cultureName)
                .CombineWithDefaultCulture()
                .CombineWithAnyCulture()
                .Published(!latestVersionEnabled)
                .LatestVersion(latestVersionEnabled)
                .OnSite(_SiteRepo.CurrentSiteName());
            if (Columns != null && Columns.Length > 0)
            {
                Query.Columns(Columns);
            }
            Query.WhereEquals("NodeGuid", NodeGuid);
            return Query.FirstOrDefault();
        }

        [CacheDependency("nodeid|{0}")]
        public ITreeNode GetDocument(int NodeID, string PageType = null, string[] Columns = null)
        {
            DocumentQuery Query = (!string.IsNullOrWhiteSpace(PageType) ? new DocumentQuery(PageType) : new DocumentQuery());
            Query.Culture(cultureName)
                .CombineWithDefaultCulture()
                .CombineWithAnyCulture()
                .Published(!latestVersionEnabled)
                .LatestVersion(latestVersionEnabled)
                .OnSite(_SiteRepo.CurrentSiteName());
            if (Columns != null && Columns.Length > 0)
            {
                Query.Columns(Columns);
            }
            Query.WhereEquals("NodeID", NodeID);
            return Query.FirstOrDefault();
        }
    }
}
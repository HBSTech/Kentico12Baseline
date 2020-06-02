using Generic.Models;
using MVCCaching;
using System.Collections.Generic;

namespace Generic.Repositories.Interfaces
{
    public interface IBreadcrumbRepository : IRepository
    {
        /// <summary>
        /// Gets a list of Breadcrumbs
        /// </summary>
        /// <param name="PageIdentifier">The Page Identifier (NodeID)</param>
        /// <returns></returns>
        List<Breadcrumb> GetBreadcrumbs(int PageIdentifier, bool IncludeDefaultBreadcrumb = true);

        /// <summary>
        /// Gets the Default Breadcrumb (built from Resource Strings)
        /// </summary>
        /// <returns></returns>
        Breadcrumb GetDefaultBreadcrumb();
    }
}
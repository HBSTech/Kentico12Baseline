using MVCCaching;

namespace Generic.Repositories.Interfaces
{
    public interface ISiteRepository : IRepository
    {
        /// <summary>
        /// Gets the SiteID for the given SiteName
        /// </summary>
        /// <param name="SiteName">The Site Name</param>
        /// <returns>The SiteID</returns>
        int GetSiteID(string SiteName);

        /// <summary>
        /// Gets the current Site Name
        /// </summary>
        /// <returns>The Site Name</returns>
        string CurrentSiteName();
    }
}
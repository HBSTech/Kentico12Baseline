using CMS.SiteProvider;
using MVCCaching;
using Generic.Repositories.Interfaces;

namespace Generic.Repositories.Implementations
{
    public class KenticoSiteRepository : ISiteRepository
    {
        [DoNotCache]
        public string CurrentSiteName()
        {
            return SiteContext.CurrentSiteName;
        }

        [CacheDependency("cms.site|byname|{0}")]
        public int GetSiteID(string SiteName)
        {
            return SiteInfoProvider.GetSiteID(SiteName);
        }
    }
}
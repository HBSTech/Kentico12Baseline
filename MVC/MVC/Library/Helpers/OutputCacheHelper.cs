using CMS.Helpers;
using CMS.SiteProvider;

namespace Generic
{
    public class OutputCacheHelper
    {
        // Sadly a little stuck with this one, need to call from Shared layout View so can't use AutoFac

        /// <summary>
        /// Adds the cache dependency key to the output cache
        /// </summary>
        /// <param name="Dependency">The Dependency Key</param>
        public static void AddCacheDependency(string Dependency)
        {
            CacheHelper.EnsureDummyKey(Dependency);
            CacheHelper.AddOutputCacheDependencies(new string[] { Dependency });
        }

        /// <summary>
        /// Adds the cache dependency keys to the output cache
        /// </summary>
        /// <param name="Dependencies">The Dependencies</param>
        public static void AddCacheDependencies(string[] Dependencies)
        {
            foreach(string Dependency in Dependencies) { 
                CacheHelper.EnsureDummyKey(Dependency);
            }
            CacheHelper.AddOutputCacheDependencies(Dependencies);
        }

        /// <summary>
        /// Gets the current site name
        /// </summary>
        /// <returns>The current site name</returns>
        public static string CurrentSiteName()
        {
            return SiteContext.CurrentSiteName;
        }


    }
}
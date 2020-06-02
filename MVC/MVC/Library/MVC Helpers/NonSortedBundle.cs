using System.Collections.Generic;
using System.Web.Optimization;

namespace KMVCHelper
{
    /// <summary>
    /// Ensures the Bundle is outputted in the order it's provided, vs. alphabetical, as JS and CSS often it is important what order things go in.
    /// </summary>
    public class NonOrderingBundleOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }
}
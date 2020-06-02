using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KMVCHelper
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Allows you to render the given path with Query String parameters, automatically handles joining the list of query string values from the given path with the dictionary provided.
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="path">The Url (relative or absolute), can include existing query strings</param>
        /// <param name="dict">Dictionary of Key-value pairs to append to the Url</param>
        /// <returns>The Url</returns>
        public static string RenderPath(this UrlHelper urlHelper, string path, IDictionary<string, object> dict)
        {
            var list = new List<string>();
            var listKeys = new List<string>();
            foreach (var item in dict)
            {
                list.Add(item.Key + "=" + item.Value);
                listKeys.Add(item.Key);
            }
            if (path.Contains('?'))
            {
                string[] ExistingQueryAndValues = path.Split('?')[1].Split('#')[0].Split('&');
                foreach (string ExistingQueryAndValue in ExistingQueryAndValues)
                {
                    string Key = "";
                    string Value = "";
                    if (ExistingQueryAndValue.Contains("="))
                    {
                        Key = ExistingQueryAndValue.Split('=')[0];
                        Value = ExistingQueryAndValue.Split('=')[1];
                    }
                    else
                    {
                        Key = ExistingQueryAndValue;
                    }

                    if (!listKeys.Contains(Key, StringComparer.InvariantCultureIgnoreCase))
                    {
                        list.Add(Key + (!string.IsNullOrWhiteSpace(Value) ? "=" + Value : ""));
                    }
                }
            }

            string Anchor = (path.Contains("#") ? path.Split('#')[1] : "");
            Anchor = (!string.IsNullOrWhiteSpace(Anchor) ? $"#{Anchor}" : "");
            string QueryList = string.Join("&", list);

            return $"{ path.Split('?')[0].Split('#')[0]}?{QueryList}{Anchor}";

        }

        /// <summary>
        /// Gets the absolute URL of the given path.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="path">The Relative Url</param>
        /// <param name="forceHttps">If https should be forced</param>
        /// <returns>The absolute URL</returns>
        public static MvcHtmlString GenerateAbsoluteUrl(this UrlHelper helper, string path, bool forceHttps = false)
        {
            const string HTTPS = "https";
            var uri = helper.RequestContext.HttpContext.Request.Url;
            var scheme = forceHttps ? HTTPS : uri.Scheme;
            var host = uri.Host;
            var port = (forceHttps || uri.Scheme == HTTPS) ? string.Empty : (uri.Port == 80 ? string.Empty : ":" + uri.Port);

            return new MvcHtmlString(string.Format("{0}://{1}{2}/{3}", scheme, host, port, string.IsNullOrEmpty(path) ? string.Empty : path.TrimStart("~/".ToCharArray())));
        }
    }
}
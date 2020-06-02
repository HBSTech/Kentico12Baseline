using CMS.Base;
using Generic.Widgets;
using HBS.StaticTextContainerizedWidget.Kentico.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Helpers
{
    /// <summary>
    /// This provides a means to control what widgets users see based on their permissions.  For example, the Static Text / Partial Page widgets should probably only be usable by the Global Admins or those who know HTML.
    /// </summary>
    public class UserWidgetProvider
    {

        public const string KenticoFormWidget_IDENTIFIER = "Kentico.FormWidget";
        public const string KenticoRichTextWidget_IDENTIFIER = "Kentico.Widget.RichText";
        /// <summary>
        /// Listing of all widgets considered "Generic" and as a baseline for what should be allowed
        /// </summary>
        /// <returns></returns>
        private static string[] GetGenericWidgets()
        {
            return new string[]
            {
                KenticoFormWidget_IDENTIFIER,
                KenticoRichTextWidget_IDENTIFIER,
                RichTextWidgetProperties.IDENTIFIER,
                StaticTextContainerizedWidgetProperties.IDENTIFIER,
                ImageWidgetController.IDENTIFIER
            };
        }

        /// <summary>
        /// Gets the current user's allowed widgets
        /// </summary>
        /// <param name="ZoneWidgets">If provided, only widgets in this list will be allowed of all allowable.</param>
        /// <param name="AddZoneWidgets">If true, then the zone widgets will be added to the user's default widgets.  False by default means only the ZoneWidgets the user has access to will be allowed</param>
        /// <returns></returns>
        public static string[] GetUserAllowedWidgets(string[] ZoneWidgets = null, bool AddZoneWidgets = false)
        {
            var User = EnvironmentHelper.AuthenticatedUser(HttpContext.Current.User);
            List<string> Widgets = new List<string>();
            if (ZoneWidgets == null || ZoneWidgets.Length == 0 || AddZoneWidgets)
            {
                Widgets.AddRange(GetGenericWidgets());
            }

            // Add or remove widgets here based on permission
            if (User != null)
            {
                switch (User.SiteIndependentPrivilegeLevel)
                {
                    case UserPrivilegeLevelEnum.GlobalAdmin:
                    case UserPrivilegeLevelEnum.Admin:
                        Widgets.Add("PartialWidgetPage.PartialWidget");
                        break;
                    case UserPrivilegeLevelEnum.Editor:
                        break;
                }
            }

            // If zone widgets provided
            if (ZoneWidgets != null && ZoneWidgets.Length > 0)
            {
                if (AddZoneWidgets)
                {
                    return Widgets.Union(ZoneWidgets).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();
                }
                else
                {
                    return ZoneWidgets.Intersect(Widgets, StringComparer.InvariantCultureIgnoreCase).ToArray();
                }
            }
            else
            {
                return Widgets.ToArray();
            }
        }

    }
}
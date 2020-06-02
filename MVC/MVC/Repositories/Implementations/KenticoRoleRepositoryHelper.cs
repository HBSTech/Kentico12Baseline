using CMS.Membership;
using Generic.Library.Helpers;
using Generic.Repositories.Helpers.Interfaces;
using MVCCaching;
using System.Linq;

namespace Generic.Repositories.Helpers.Implementations
{
    public class KenticoRoleRepositoryHelper : IKenticoRoleRepositoryHelper
    {
        [CacheDependency("cms.user|byid|{0}")]
        public UserInfo GetUser(int UserID)
        {
            return UserInfoProvider.GetUserInfo(UserID);
        }

        [CacheDependency("cms.role|byname|{0}")]
        public RoleInfo GetRole(string RoleName, string SiteName, string[] Columns = null)
        {
            return RoleInfoProvider.GetRoles()
                .OnSite(new CMS.DataEngine.SiteInfoIdentifier(SiteName))
                .ColumnsNullHandled(Columns)
                .FirstOrDefault();
        }
    }
}
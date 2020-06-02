using CMS.EventLog;
using CMS.Membership;
using MVCCaching;
using Generic.Repositories.Interfaces;
using Generic.Repositories.Helpers.Interfaces;

namespace Generic.Repositories.Implementations
{
    public class KenticoRoleRepository : IRoleRepository
    {
        private IKenticoRoleRepositoryHelper _Helper;

        public KenticoRoleRepository(IKenticoRoleRepositoryHelper Helper)
        {
            _Helper = Helper;
        }

        [DoNotCache]
        public RoleInfo GetRole(string RoleName, string SiteName, string[] Columns = null)
        {
            return _Helper.GetRole(RoleName, SiteName, Columns);
        }

        [DoNotCache]
        public void SetUserRole(int UserID, string RoleName, string SiteName, bool RoleToggle)
        {
            var Role = _Helper.GetRole(RoleName, SiteName, new string[] { "RoleID" });
            if (RoleToggle)
            {
                if (UserRoleInfoProvider.GetUserRoleInfo(UserID, Role.RoleID) == null)
                {
                    UserRoleInfoProvider.AddUserToRole(UserID, Role.RoleID);
                }
            }
            else
            {
                var ExistingUserRole = UserRoleInfoProvider.GetUserRoleInfo(UserID, Role.RoleID);
                if (ExistingUserRole != null)
                {
                    ExistingUserRole.Delete();
                }
            }
        }

        [CacheDependency("cms.userrole|all")]
        public bool UserInRole(int UserID, string RoleName, string SiteName)
        {
            var Role = _Helper.GetRole(RoleName, SiteName, new string[] { "RoleID" });
            if (Role == null)
            {
                EventLogProvider.LogEvent("E", "KenticoUserRepository", "NoAdministratorRole", "No Administrator Role found! Please add!");
            }
            return UserRoleInfoProvider.IsUserInRole(UserID, Role.RoleID);
        }

        [CacheDependency("cms.user|byid|{0}")]
        [CacheDependency("cms.userrole|all")]
        [CacheDependency("cms.permission|all")]
        [CacheDependency("cms.rolepermission|all")]
        public bool UserHasPermission(int UserID, string ResourceName, string PermissionName, string SiteName)
        {
            return UserSecurityHelper.IsAuthorizedPerResource(ResourceName, PermissionName, SiteName, _Helper.GetUser(UserID));
        }

        
    }
}
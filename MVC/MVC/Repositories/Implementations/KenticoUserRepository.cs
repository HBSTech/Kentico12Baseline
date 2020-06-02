using CMS.Membership;
using Kentico.Membership;
using Microsoft.AspNet.Identity.Owin;
using MVCCaching;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Generic.Repositories.Interfaces;
using CMS.Base;
using Generic.Library.Helpers;

namespace Generic.Repositories.Implementations
{
    public class KenticoUserRepository : IUserRepository
    {
        /// <summary>
        /// Provides access to the Kentico.Membership.UserManager instance.
        /// </summary>
        public UserManager UserManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Get<UserManager>();
            }
        }

        public KenticoUserRepository()
        {
        }

        [CacheDependency("cms.user|byname|{0}")]
        public IUserInfo GetUserByUsername(string Username, string[] Columns = null)
        {
            return UserInfoProvider.GetUsers()
                .WhereEquals("Username", Username)
                .ColumnsNullHandled(Columns)
                .FirstOrDefault();
        }

        [CacheDependency("cms.user|byname|{0}")]
        public IUserInfo GetUserByEmail(string Email, string[] Columns = null)
        {
            return UserInfoProvider.GetUsers()
                .WhereEquals("Email", Email)
                .ColumnsNullHandled(Columns)
                .FirstOrDefault();
        }

        [CacheDependency("cms.user|byid|{0}")]
        public IUserInfo GetUserByID(int UserID, string[] Columns = null)
        {
            return UserInfoProvider.GetUsers()
                .WhereEquals("UserID", UserID)
                .ColumnsNullHandled(Columns)
                .FirstOrDefault();
        }

        [CacheDependency("cms.user|byguid|{0}")]
        public int? UserGuidToID(Guid UserGuid)
        {
            return UserInfoProvider.GetUsers()
                .WhereEquals("UserGuid", UserGuid)
                .Columns("UserID")
                .FirstOrDefault()
                ?.UserID;
        }

    }
}
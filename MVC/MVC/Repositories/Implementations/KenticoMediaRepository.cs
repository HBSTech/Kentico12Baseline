using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.MediaLibrary;
using Generic.Repositories.Interfaces;
using MVCCaching;
using System;
using System.Linq;

namespace Generic.Repositories.Implementations
{
    public class KenticoMediaRepository : IMediaRepository
    {
        private ISiteRepository _SiteRepo;

        public KenticoMediaRepository(ISiteRepository SiteRepo)
        {
            _SiteRepo = SiteRepo;
        }

        [CacheDependency("attachment|{0}")]
        public string GetAttachmentImage(TreeNode Page, Guid ImageGuid)
        {
            return Page?.AllAttachments.FirstOrDefault(x => x.AttachmentGUID == ImageGuid).AttachmentUrl;
        }

        [CacheDependency("mediafile|{0}")]
        public string GetMediaFileUrl(Guid FileGuid)
        {
            try
            {
                return MediaLibraryHelper.GetDirectUrl(MediaFileInfoProvider.GetMediaFileInfo(FileGuid, _SiteRepo.CurrentSiteName()));
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("KenticoMediaRepository", "MediaFileMissing", ex, additionalMessage: "For media file with Guid " + FileGuid.ToString());
                return "";
            }
        }
    }
}
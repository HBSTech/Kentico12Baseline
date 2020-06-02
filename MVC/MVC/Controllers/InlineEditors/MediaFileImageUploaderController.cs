using System;
using System.Web;
using System.Web.Mvc;

using CMS.MediaLibrary;
using CMS.Membership;
using CMS.SiteProvider;

using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;

namespace Generic.Controllers.InlineEditors
{
    public class MediaFileImageUploaderController : Controller
    {
        [HttpPost]
        public JsonResult Upload(string libraryName)
        {
            if (!HttpContext.Kentico().PageBuilder().EditMode)
            {
                throw new HttpException(403, "It is allowed to upload an image only when the page builder is in the edit mode.");
            }

            var library = MediaLibraryInfoProvider.GetMediaLibraryInfo(libraryName, SiteContext.CurrentSiteName);
            if (library == null)
            {
                throw new InvalidOperationException($"The '{libraryName}' media library doesn't exist.");
            }

            if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(library, "FileCreate", MembershipContext.AuthenticatedUser))
            {
                throw new HttpException(403, "You are not authorized to upload an image to the media library.");
            }

            var imageGuid = Guid.Empty;

            foreach (string requestFileName in Request.Files)
            {
                imageGuid = AddMediaFile(requestFileName, library);
            }

            return Json(new { guid = imageGuid });
        }


        private Guid AddMediaFile(string requestFileName, MediaLibraryInfo library)
        {
            if (!(Request.Files[requestFileName] is HttpPostedFileWrapper file))
            {
                return Guid.Empty;
            }

            return ImageUploaderHelper.Upload(file, path =>
            {
                var mediaFile = new MediaFileInfo(path, library.LibraryID);
                MediaFileInfoProvider.SetMediaFileInfo(mediaFile);

                return mediaFile.FileGUID;
            });
        }
    }
}
using System.Web.Mvc;
using DynamicRouting;
using DynamicRouting.Kentico.MVC;
using Generic.Controllers;
using CMS.DocumentEngine;
using System.IO;
using File = CMS.DocumentEngine.Types.CMS.File;
using CMS.SiteProvider;
using DynamicRouting.Interfaces;

[assembly: DynamicRouting(typeof(FileController), new string[] { File.CLASS_NAME }, nameof(FileController.GetFile))]
namespace Generic.Controllers
{
    /// <summary>
    /// This allows you to have Generic.File page types that when you visit the URL it renders the file itself.
    /// </summary>
    public class FileController : Controller
    {
        readonly IDynamicRouteHelper _DynamicRouteHelper;
        public FileController(IDynamicRouteHelper dynamicRouteHelper)
        {
            _DynamicRouteHelper = dynamicRouteHelper;
        }

        public ActionResult GetFile()
        {
            File Page = _DynamicRouteHelper.GetPage<File>();
            var Attachment = AttachmentInfoProvider.GetAttachmentInfo(Page.FileAttachment, SiteContext.CurrentSiteName);
            if (Attachment != null)
            {
                return new FileStreamResult(new MemoryStream(Attachment.AttachmentBinary), Attachment.AttachmentMimeType)
                {
                    FileDownloadName = Attachment.AttachmentName
                };
            }
            else
            {
                return HttpNotFound();
            }
        }
    }
}
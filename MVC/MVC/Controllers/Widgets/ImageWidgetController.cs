using Generic.Widgets;
using Generic.Models;
using Generic.Repositories.Interfaces;
using Generic.ViewModels;
using Kentico.PageBuilder.Web.Mvc;
using System.Web.Mvc;
using Generic.Models.InlineEditors;

[assembly: RegisterWidget(ImageWidgetController.IDENTIFIER, typeof(ImageWidgetController), "Image", Description = "Places an image on the page", IconClass = "icon-picture")]

namespace Generic.Widgets
{
    public class ImageWidgetController : WidgetController<ImageWidgetProperties>
    {
        public const string IDENTIFIER = "Generic.ImageWidget";
        readonly IMediaRepository _MediaRepo;

        public ImageWidgetController(IMediaRepository MediaRepository) {

            _MediaRepo = MediaRepository;
        }

        public ActionResult Index()
        {
            var Properties = GetProperties();
            ImageWidgetViewModel model = new ImageWidgetViewModel()
            {
                ImageUrl = (Properties.UseAttachment ? _MediaRepo.GetAttachmentImage(GetPage(), Properties.ImageGuid) : (Properties.ImageUrl != null && Properties.ImageUrl.Count > 0 ? _MediaRepo.GetMediaFileUrl(Properties.ImageUrl[0].FileGuid) : "")),
                Alt = Properties.Alt,
                CssClass = Properties.CssClass,
                ImageType = Properties.UseAttachment ? ImageTypeEnum.Attachment : ImageTypeEnum.MediaFile
            };

            return View(model);
        }
    }
}
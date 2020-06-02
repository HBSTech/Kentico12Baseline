using Generic.Widgets;
using Generic.Models;
using Generic.Repositories.Interfaces;
using Generic.ViewModels;
using Kentico.PageBuilder.Web.Mvc;
using System.Web.Mvc;

[assembly: RegisterWidget(ImageWidgetController.IDENTIFIER, typeof(ImageWidgetController), "Image", Description = "Places an image on the page", IconClass = "icon-picture")]

namespace Generic.Widgets
{
    public class ImageWidgetController : WidgetController<ImageWidgetProperties>
    {
        public const string IDENTIFIER = "Generic.ImageWidget";
        readonly IMediaRepository MediaRepo;

        public ImageWidgetController(IMediaRepository MediaRepository) {

            MediaRepo = MediaRepository;
        }

        public ActionResult Index()
        {
            var Properties = GetProperties();
            ImageWidgetViewModel model = new ImageWidgetViewModel()
            {
                ImageUrl = (Properties.UseAttachment ? MediaRepo.GetAttachmentImage(GetPage(), Properties.ImageGuid) : Properties.ImageUrl),
                Alt = Properties.Alt,
                CssClass = Properties.CssClass
            };

            return View(model);
        }
    }
}
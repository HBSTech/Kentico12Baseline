using Generic.Models.InlineEditors;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using System.Web;
using System.Web.Mvc;


namespace Models.InlineEditors
{
    /// <summary>
    /// View model for Image uploader editor.
    /// </summary>
    public class ImageUploaderEditorViewModel : InlineEditorViewModel
    {
        private const string MEDIA_LIBRARY_NAME = "Graphics";


        /// <summary>
        /// Indicates if image is present.
        /// </summary>
        public bool HasImage { get; set; }


        /// <summary>
        /// Indicates if the message should be positioned absolutely for empty image.
        /// </summary>
        public bool UseAbsolutePosition { get; set; }


        /// <summary>
        /// Position of the message in case of absolute position.
        /// </summary>
        public PanelPositionEnum MessagePosition { get; set; } = PanelPositionEnum.Center;


        /// <summary>
        /// Type of the uploaded image.
        /// </summary>
        public ImageTypeEnum ImageType { get; set; } = ImageTypeEnum.Attachment;


        /// <summary>
        /// Gets URL for uploading the data.
        /// </summary>
        public string GetDataUrl()
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

            if (ImageType == ImageTypeEnum.Attachment)
            {
                return urlHelper.Kentico().AuthenticateUrl(urlHelper.Action("Upload", "AttachmentImageUploader", new
                {
                    pageId = HttpContext.Current.Kentico().PageBuilder().PageIdentifier
                }), false).ToString();
            }

            if (ImageType == ImageTypeEnum.MediaFile)
            {
                return urlHelper.Kentico().AuthenticateUrl(urlHelper.Action("Upload", "MediaFileImageUploader", new
                {
                    libraryName = MEDIA_LIBRARY_NAME
                }), false).ToString();
            }

            return null;
        }
    }
}
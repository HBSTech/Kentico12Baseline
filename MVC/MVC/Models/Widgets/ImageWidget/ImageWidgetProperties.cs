using System;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

namespace Generic.Models
{
    /// <summary>
    /// Properties of Image widget.
    /// </summary>
    public class ImageWidgetProperties : IWidgetProperties
    {
        /// <summary>
        /// Guid of an image to be displayed.
        /// </summary>
        public Guid ImageGuid { get; set; }

        [EditingComponent(CheckBoxComponent.IDENTIFIER, Label ="Use Attachment", Tooltip ="Uncheck if you wish to use the below media library path.", Order = 1)]
        public bool UseAttachment { get; set; } = true;

        [EditingComponent(TextInputComponent.IDENTIFIER, Label ="Media relative link", Order = 2)]
        public string ImageUrl { get; set; }

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "Image Alt", Order = 3)]
        public string Alt { get; set; }

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "CSS Class", Order = 4)]
        public string CssClass { get; set; }
    }
}
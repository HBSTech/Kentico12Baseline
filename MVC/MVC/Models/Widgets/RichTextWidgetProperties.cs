using Generic.Models.FormComponents;
using HBS.TinyMCE_Wysiwyg;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using System;

[assembly: RegisterWidget(Generic.Widgets.RichTextWidgetProperties.IDENTIFIER, "Rich Text (Container)", typeof(Generic.Widgets.RichTextWidgetProperties), Description = "Text area where the content will be rendered as is as HTML through the widget dialog", IconClass = "icon-l-lightbox")]
namespace Generic.Widgets
{
    public class RichTextWidgetProperties : PageBuilderContainers.PageBuilderWithHtmlBeforeAfterWidgetProperties
    {
        public const string IDENTIFIER = "RichTextEditor";

        [EditingComponent(TinyMCEInputComponent.IDENTIFIER, Label = "Html Content")]
        public string Html { get; set; }

        [EditingComponent(GuidInputComponent.IDENTIFIER, Label = "Guid test")]
        public Guid guidTest { get; set; }

        public RichTextWidgetProperties()
        {

        }
    }
}
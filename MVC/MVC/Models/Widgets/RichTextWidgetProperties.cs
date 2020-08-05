﻿using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

[assembly: RegisterWidget(Generic.Widgets.RichTextWidgetProperties.IDENTIFIER, "Rich Text (Container)", typeof(Generic.Widgets.RichTextWidgetProperties), Description = "Text area where the content will be rendered as is as HTML through the widget dialog", IconClass = "icon-l-lightbox")]
namespace Generic.Widgets
{
    public class RichTextWidgetProperties : PageBuilderContainers.PageBuilderWithHtmlBeforeAfterWidgetProperties
    {
        public const string IDENTIFIER = "RichTextEditor";

        [EditingComponent(RichText.IDENTIFIER, Label = "Html Content")]
        public string Html { get; set; }

        public RichTextWidgetProperties()
        {

        }
    }
}
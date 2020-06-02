using Kentico.PageBuilder.Web.Mvc.PageTemplates;

// This is used as a General "Widget" Page.
[assembly: RegisterPageTemplate("Blank.Widget", "Blank Widget Page", customViewName: "PageTemplates/_BlankWidgetTemplate", Description = "Blank page with a widget zone")]
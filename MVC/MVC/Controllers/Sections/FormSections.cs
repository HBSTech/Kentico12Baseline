using Kentico.Forms.Web.Mvc;
using System.Web.Mvc;

[assembly: RegisterFormSection("Bootstrap.TwoColumn", typeof(BootstrapTwoColumnFormSectionController), "Two columns 50-50", Description = "Organizes fields into two equal-width columns.", IconClass = "icon-l-cols-2")]
[assembly: RegisterFormSection("Bootstrap.TwoColumn2575", typeof(BootstrapTwoColumn2575FormSectionController), "Two columns 25-75", Description = "Organizes fields into two columns, 25% and 75% widget.", IconClass = "icon-l-cols-20-80")]
[assembly: RegisterFormSection("Bootstrap.TwoColumn7525", typeof(BootstrapTwoColumn7525FormSectionController), "Two columns 75-25", Description = "Organizes fields into two columns, 75% and 25% widget.", IconClass = "icon-l-cols-80-20")]
[assembly: RegisterFormSection("Bootstrap.TwoColumn3366", typeof(BootstrapTwoColumn3366FormSectionController), "Two columns 33-66", Description = "Organizes fields into two columns, 33% and 66% widget.", IconClass = "icon-l-cols-30-70")]
[assembly: RegisterFormSection("Bootstrap.TwoColumn6633", typeof(BootstrapTwoColumn6633FormSectionController), "Two columns 66-33", Description = "Organizes fields into two columns, 66% and 33% widget.", IconClass = "icon-l-cols-70-30")]
[assembly: RegisterFormSection("Bootstrap.ThreeColumn", typeof(BootstrapThreeColumnFormSectionController), "Three Columns", Description = "Organizes fields into three equal-width columns.", IconClass = "icon-l-cols-3")]
[assembly: RegisterFormSection("Bootstrap.FourColumn", typeof(BootstrapFourColumnFormSectionController), "Four Columns", Description = "Organizes fields into four equal-width columns.", IconClass = "icon-l-cols-4")]
[assembly: RegisterFormSection("Bootstrap.FiveColumn", typeof(BootstrapFiveColumnFormSectionController), "Five Columns", Description = "Organizes fields into five equal-width columns.", IconClass = "icon-l-cols-4")]
[assembly: RegisterFormSection("Bootstrap.SixColumn", typeof(BootstrapSixColumnFormSectionController), "Six Columns", Description = "Organizes fields into six equal-width columns.", IconClass = "icon-l-cols-4")]

public class BootstrapTwoColumnFormSectionController : Controller
{
    // Action used to retrieve the section markup
    public ActionResult Index()
    {
        return PartialView("FormSections/_BootstrapTwoColumnFormSection");
    }
}
public class BootstrapTwoColumn2575FormSectionController : Controller
{
    // Action used to retrieve the section markup
    public ActionResult Index()
    {
        return PartialView("FormSections/_BootstrapTwoColumn2575FormSection");
    }
}
public class BootstrapTwoColumn7525FormSectionController : Controller
{
    // Action used to retrieve the section markup
    public ActionResult Index()
    {
        return PartialView("FormSections/_BootstrapTwoColumn7525FormSection");
    }
}
public class BootstrapTwoColumn3366FormSectionController : Controller
{
    // Action used to retrieve the section markup
    public ActionResult Index()
    {
        return PartialView("FormSections/_BootstrapTwoColumn3366FormSection");
    }
}
public class BootstrapTwoColumn6633FormSectionController : Controller
{
    // Action used to retrieve the section markup
    public ActionResult Index()
    {
        return PartialView("FormSections/_BootstrapTwoColumn6633FormSection");
    }
}
public class BootstrapThreeColumnFormSectionController : Controller
{
    // Action used to retrieve the section markup
    public ActionResult Index()
    {
        return PartialView("FormSections/_BootstrapThreeColumnFormSection");
    }
}
public class BootstrapFourColumnFormSectionController : Controller
{
    // Action used to retrieve the section markup
    public ActionResult Index()
    {
        return PartialView("FormSections/_BootstrapFourColumnFormSection");
    }
}
public class BootstrapFiveColumnFormSectionController : Controller
{
    // Action used to retrieve the section markup
    public ActionResult Index()
    {
        return PartialView("FormSections/_BootstrapFiveColumnFormSection");
    }
}
public class BootstrapSixColumnFormSectionController : Controller
{
    // Action used to retrieve the section markup
    public ActionResult Index()
    {
        return PartialView("FormSections/_BootstrapSixColumnFormSection");
    }
}
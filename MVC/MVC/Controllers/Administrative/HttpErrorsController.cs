using System.Web.Mvc;

namespace Controllers
{
    public class HttpErrorsController : Controller
    {
        public ActionResult Index()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;

            return View();
        }
    }
}
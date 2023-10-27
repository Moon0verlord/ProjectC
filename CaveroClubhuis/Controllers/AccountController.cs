using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace CaveroClubhuis.Controllers
{
   
    public class AccountController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {

            // Access the content root path
                string contentRootPath = _webHostEnvironment.ContentRootPath;

                // Access the web root path
                string webRootPath = _webHostEnvironment.WebRootPath;

               // Access the current environment name (Development, Staging, Production, etc.)
               string environmentName = _webHostEnvironment.EnvironmentName;

             
         
            return View();
        }

        //[HttpPost]
        //public IActionResult Index(string URL)
        //{
        //    // Access the content root path
        //    string contentRootPath = _webHostEnvironment.ContentRootPath;

        //    // Access the web root path
        //    string webRootPath = _webHostEnvironment.WebRootPath;

        //    // Access the current environment name (Development, Staging, Production, etc.)
        //    string environmentName = _webHostEnvironment.EnvironmentName;

        //    // Do something with the environment information, for example, returning it to the view
        //    ViewData["ContentRootPath"] = contentRootPath;
        //    ViewData["WebRootPath"] = webRootPath;
        //    ViewData["EnvironmentName"] = environmentName;
        //    return View();
        //}
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Tucson.Palermo.Models;

namespace Tucson.Palermo.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}

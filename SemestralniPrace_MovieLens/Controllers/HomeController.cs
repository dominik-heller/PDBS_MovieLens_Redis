using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SemestralniPrace_MovieLens.DAL;
using SemestralniPrace_MovieLens.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SemestralniPrace_MovieLens.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDataAccess _store;

        public HomeController(ILogger<HomeController> logger, IDataAccess store)
        {
            _logger = logger;
            _store = store;

        }

        public IActionResult Index()
        {
            Movie m = _store.GetMovie("Movies/2");
            return View("Privacy",m);
        }

        public IActionResult Privacy()
        {
            Movie m = _store.GetMovie("Movies/1");
            return View(m);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

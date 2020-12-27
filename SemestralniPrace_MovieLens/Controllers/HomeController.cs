using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SemestralniPrace_MovieLens.DAL;
using SemestralniPrace_MovieLens.Models;
using SemestralniPrace_MovieLens.ViewModels;
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

        public IActionResult AllMovies(string option, int page)
        {
            //gets a page of size 10 of movies
            IList<Movie> movies = _store.GetAllMovies(option, page);
            return View(movies);
        }

        public IActionResult MoviesByTitleSearch(string selection)
        {
            IList<Movie> movies = _store.GetMovieByTitleSearch(selection);
            return View("AllMovies", movies);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
          // string id = "";
          //  _store.GetSimilarUsers(id);
            return View();
        }

        public IActionResult SimilarUsers(string UserId)
        {
            string id = UserId;
            TempData["SelectedUserId"] = id;
            List<SimilarUser> l =_store.GetSimilarUsers(id);
            return View(l);
        }

        public IActionResult GetTags(string movieid, int page)
        {
            IList<Tags> taglist = _store.GetTagsByMovieId(movieid, page);
            return PartialView(taglist);
        }

        public IActionResult MovieDetail(string movie_id)
        {
            Movie m = _store.GetMovie(movie_id);
            return View(m);
        }

        public IActionResult TopByGenre(string genre)
        {
            IList<Movie> movies_by_genre = _store.GetMovieByGenre(genre);
            return View("AllMovies", movies_by_genre);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

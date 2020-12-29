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
        private readonly IDataAccess _store;

        public HomeController(IDataAccess store)
        {
            _store = store;
        }

        public IActionResult Index()
        {
            return View();
        }

        //gets a page of size 10
        public IActionResult AllMovies(string option, int page)
        {
            IList<Movie> movies = _store.GetAllMovies(option, page);
            return View(movies);
        }

        public IActionResult MoviesByTitleSearch(string selection)
        {
            IList<Movie> movies = _store.GetMovieByTitleSearch(selection);
            return View("AllMovies", movies);
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

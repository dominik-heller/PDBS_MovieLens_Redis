using SemestralniPrace_MovieLens.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemestralniPrace_MovieLens.DAL
{
    public interface IDataAccess
    {
        public Movie GetMovie(string id);
        IList<Movie> GetAllMovies(string option, int page);
        IList<Movie> GetMovieByTitleSearch(string selection);
    }
}

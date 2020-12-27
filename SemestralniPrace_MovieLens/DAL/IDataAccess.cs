using SemestralniPrace_MovieLens.Models;
using SemestralniPrace_MovieLens.ViewModels;
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
        IList<Movie> GetMovieByGenre(string genre);
        IList<Tags> GetTagsByMovieId(string movieid, int page);
        List<SimilarUser> GetSimilarUsers(string id);
    }
}

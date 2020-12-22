using Raven.Client.Documents;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using SemestralniPrace_MovieLens.DAL.Indexes;
using SemestralniPrace_MovieLens.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemestralniPrace_MovieLens.DAL
{
    public class DataAccess : IDataAccess
    {
        private readonly IDocumentStore _store;
        public DataAccess(IDocumentStore store)
        {
            _store = store;
            _store.Initialize();
            new Movie_ByTitle().Execute(_store);
            new Movie_TitleSearch().Execute(_store);
            new Rating_AverageForMovie().Execute(_store);
            new Movie_GenreAndRatingSearch().Execute(_store);
        }

        public IList<Movie> GetAllMovies(string option, int page)
        {
            using (IDocumentSession session = _store.OpenSession())
            {
                if (option.Equals("rating")) return session.Query<Movie, Movie_GenreAndRatingSearch>().OrderByDescending(x => x.AverageRating).Skip((page - 1) * 10).Take(10).OfType<Movie>().ToList();
                if (option.Equals("title")) return session.Query<Movie, Movie_ByTitle>().OrderBy(x=>x.Title).Skip((page-1)*10).Take(10).OfType<Movie>().ToList();
                return session.Query<Movie>().Skip((page - 1) * 10).Take(10).ToList();
            }
        }

        public IList<Movie> GetByGenre(string genre)
        {

            using (IDocumentSession session = _store.OpenSession())
            {
                IList<Movie> movies_by_genre = session.Advanced.DocumentQuery<Movie, Movie_GenreAndRatingSearch>().Search("Query", genre).OrderByDescending(x => x.AverageRating).Take(100).ToList();
                return movies_by_genre;
            }
        }

        public Movie GetMovie(string id)
        {
            using (IDocumentSession session = _store.OpenSession())
            {
                //   session.Advanced.Patch<Movie, string>(id, x => x.Rating, "prd");
                //   session.SaveChanges();
                // ne všechny movies mají rating (59047)
                /*
                IList<Rating_AverageForMovie.Result> results1 = session.Query<Rating_AverageForMovie.Result, Rating_AverageForMovie>().ToList();
                Console.WriteLine(results1.Count());
                foreach(Rating_AverageForMovie.Result i in results1)
                {
                    session.Advanced.Patch<Movie, double>(id, x => x.AverageRating, rating_average);
                session.SaveChanges();
                }
                // IList<Rating_AverageForMovie.Result> results = session.Query<Rating_AverageForMovie.Result, Rating_AverageForMovie>().Where(x => x.MovieId.Equals(id)).ToList();
                // double rating = results.FirstOrDefault().Rating_Average;
                // new MovieAndRating() { Movie = t, AverageRating = rating };
                */
                Movie t = session.Load<Movie>(id);
                return t;
            }
        }


        public IList<Movie> GetMovieByTitleSearch(string selection)
        {
            using (IDocumentSession session = _store.OpenSession())
            {
                IList<Movie> movies = session.Advanced.DocumentQuery<Movie, Movie_TitleSearch>().Search("Query", $"{selection}", @operator: SearchOperator.And).Statistics(out QueryStatistics stats).ToList();
                int totalResults = stats.TotalResults;
                return movies;
            }
        }
    }
}

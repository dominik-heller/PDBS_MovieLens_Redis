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
        }

        public IList<Movie> GetAllMovies(string option, int page)
        {
            using (IDocumentSession session = _store.OpenSession())
            {
               if (option.Equals("title")) return session.Query<Movie, Movie_ByTitle>().OrderBy(x=>x.Title).Skip((page-1)*10).Take(10).OfType<Movie>().ToList();
               return session.Query<Movie>().Skip((page-1)*10).Take(10).ToList();
            }
        }

        public Movie GetMovie(string id)
        {
            using (IDocumentSession session = _store.OpenSession())
            {
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

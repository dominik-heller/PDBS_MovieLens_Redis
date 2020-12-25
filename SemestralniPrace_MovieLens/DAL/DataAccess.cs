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
            new Tags_ByMovieId().Execute(_store);
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

        public IList<Movie> GetMovieByGenre(string genre)
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

        public IList<Tags> GetTagsByMovieId(string movieid, int page)
        {
            using (IDocumentSession session = _store.OpenSession())
            {
                IList <Tags>  l = session.Query<Tags, Tags_ByMovieId>().Where(x=>x.MovieId==movieid).Skip((page) * 10).Take(10).ToList();
                return l;
            }
        }

        #region Event_subscription
        //Tato část pouze ukazuje vyvolání "triggeru" v situaci, kdy dojde k přidání nového/odebrání stávajícího ratingu k filmu (to v semestrální práci samostatně neřešeno, ale případně by to asi byla vhodná funkcionalita)
        //Map-reduce index vypočítavající AvarageRating se přidání/odebrání ratingu k filmu ihned aktualizuje a novou hodnotu poté uložíme k příslušnému filmu
        // => tím by výsledné hodnocení filmu zůstalo vždy aktuální
        //Níže ukázáno na uložení nového ratingu.
        private void StoreRating()
        {
            _store.OnAfterSaveChanges += this.OnAfterSaveChanges;
            using (IDocumentSession session = _store.OpenSession())
            {
                Ratings r = new Ratings() { Id = "test_rating", MovieId = "Movies/100001", Rating = 5.0, Timestamp = DateTime.Now.ToString(), UserId = "myUser" };
                session.Store(r);
                session.SaveChanges();
            }
        }

        private void OnAfterSaveChanges(object sender, AfterSaveChangesEventArgs args)
        {
            if(args.Entity is Ratings)
            {
                var rating = args.Entity as Ratings;
                using (IDocumentSession session = _store.OpenSession())
                {
                    double average = session.Query<Rating_AverageForMovie.Result, Rating_AverageForMovie>().Where(x => x.MovieId.Equals(rating.MovieId)).FirstOrDefault().Rating_Average;
                    session.Advanced.Patch<Movie, double>(rating.MovieId, x => x.AverageRating, average);
                    session.SaveChanges();
                }
            }

            
        }

        #endregion
    }
}

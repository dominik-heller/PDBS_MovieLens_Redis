﻿using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using SemestralniPrace_MovieLens.DAL.Indexes;
using SemestralniPrace_MovieLens.Models;
using SemestralniPrace_MovieLens.ViewModels;
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
            this.InitializeDb(_store);
        }

        private void InitializeDb(IDocumentStore store)
        {
            try
            {
                _store.Initialize();
                new Movie_ByTitle().Execute(_store);
                new Movie_TitleSearch().Execute(_store);
                new Rating_AverageForMovie().Execute(_store);
                new Movie_GenreAndRatingSearch().Execute(_store);
                new Tags_ByMovieId().Execute(_store);
                new Ratings_ByMovieIdAndUserId().Execute(_store);
            }
            catch (Raven.Client.Exceptions.RavenException e)
            {
                throw new Raven.Client.Exceptions.RavenException("Unable to connect to DB => check the connection setting in: 'SemestralniPrace\\SemestralniPrace_MovieLens\\appsettings.json'", e);
            }
        }

        public List<SimilarUser> GetSimilarUsers(string id)
        {
            List<Ratings> user_ratings = new List<Ratings>();
            if (user_ratings != null || user_ratings.Count() > 10)
            {
                //ziskani vsech hodnoceni zvoleneho uzivatele
                using (IDocumentSession session = _store.OpenSession())
                {
                    user_ratings = session.Query<Ratings, Ratings_ByMovieIdAndUserId>().Where(x => x.UserId == id).ToList();
                }
                //selekce vsech movieId z hodnoceni zvoleneho uzivatele
                List<string> user_ratedmovies = user_ratings.Select(x => x.MovieId).ToList();
                double sumofuserratings = user_ratings.Sum(x => x.Rating);
                int countofuserratings = user_ratedmovies.Count();
                double finalaverage = sumofuserratings / countofuserratings;
                //ziskani vsech hodnoceni u vsech filmu ktere zvoleny uzivatel hodnotil (asynchronně)
                List<Ratings> ratings_foruseratedmovies = DoWork(user_ratedmovies); //sync:  session.Query<Ratings, Ratings_ByMovieIdAndUserId>().Where(x => x.MovieId.In(user_ratedmovies)).ToList();
                //prumerne hodnoceni kazdeho uzivatele napric vsemi ziskanymi filmy
                var results = from p in ratings_foruseratedmovies
                              group p by p.UserId into g
                              let sumor = g.Sum(x => x.Rating)
                              let countor = g.Count()
                              select new SimilarUser
                              {
                                  UserId = g.Key,
                                  // SumOfRatings = sumor,
                                  CountOfRatings = countor,
                                  AverageRating = sumor / countor
                              };
                //selekce pouze tech uzivatelu, kteri hodnotili alespon polovinu filmu co zvoleny uzivatel
                var results1 = from p in results
                               where p.CountOfRatings > countofuserratings / 2 
                               select p;
                //vyber top 10 nejpodobněji hodnotích uživatelů (11=>1 is selected user)
                var l = results1.OrderBy(item => Math.Abs(finalaverage - item.AverageRating)).Take(11).ToList();
                //l.Add(new SimilarUser { UserId = id, CountOfRatings = countofuserratings, AverageRating = finalaverage }); //vybraný user
                return l.ToList();
            }
            else
            {
                List<SimilarUser> l = new List<SimilarUser>();
                l.Add(new SimilarUser { UserId = null, AverageRating = 0, CountOfRatings = 0 });
                return l;
            }
        }

        private List<Ratings> DoWork(List<string> user_ratedmovies)
        {
            async Task<List<Ratings>> GetDocument(int skip, int take)
            {
                using (IAsyncDocumentSession asyncSession = _store.OpenAsyncSession())
                {
                    {
                        using (var session = _store.OpenAsyncSession())
                        {
                            return await session.Query<Ratings, Ratings_ByMovieIdAndUserId>().Where(x => x.MovieId.In(user_ratedmovies.Skip(skip).Take(take))).ToListAsync();
                        };
                    }
                }
            }
            int no_of_tasks = 30;
            int rounds = (user_ratedmovies.Count() / no_of_tasks) + 1;
            List<Task<List<Ratings>>> tasks = new List<Task<List<Ratings>>>();
            for (int i = 0; i < no_of_tasks; i++)
            {
                tasks.Add(GetDocument(rounds * i, rounds));
                if (rounds * i > user_ratedmovies.Count())
                { break; }
            }
            Task.WaitAll(tasks.ToArray());
            List<Ratings> final = new List<Ratings>();
            foreach (var i in tasks)
            {
                List<Ratings> l = i.Result;
                final.AddRange(l);
            }
            return final;
        }

        public IList<Movie> GetAllMovies(string option, int page)
        {
            using (IDocumentSession session = _store.OpenSession())
            {
                if (option.Equals("rating")) return session.Query<Movie, Movie_GenreAndRatingSearch>().OrderByDescending(x => x.AverageRating).Skip((page - 1) * 10).Take(10).OfType<Movie>().ToList();
                if (option.Equals("title")) return session.Query<Movie, Movie_ByTitle>().OrderBy(x => x.Title).Skip((page - 1) * 10).Take(10).OfType<Movie>().ToList();
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
                return session.Load<Movie>(id);
            }
        }


        public IList<Movie> GetMovieByTitleSearch(string selection)
        {
            using (IDocumentSession session = _store.OpenSession())
            {
                IList<Movie> movies = session.Advanced.DocumentQuery<Movie, Movie_TitleSearch>().Search(x => x.Title, selection, @operator: SearchOperator.And).Statistics(out QueryStatistics stats).ToList();
                // int totalResults = stats.TotalResults;
                return movies;
            }
        }

        public IList<Tags> GetTagsByMovieId(string movieid, int page)
        {
            using (IDocumentSession session = _store.OpenSession())
            {
                IList<Tags> l = session.Query<Tags, Tags_ByMovieId>().Where(x => x.MovieId == movieid).Skip((page) * 10).Take(10).ToList();
                return l;
            }
        }

        #region Event_subscription
        //Tato část pouze ukazuje vyvolání "triggeru" v situaci, kdy dojde k přidání nového/odebrání stávajícího ratingu k filmu (to v semestrální práci samostatně neřešeno, ale případně by to byla vhodná klíčová funkcionalita)
        //Map-reduce index vypočítavající AvarageRating se přidáním/odebráním ratingu k filmu ihned aktualizuje a novou hodnotu poté díký navázání na událost OnAfterSaveChanges() uložíme k příslušnému filmu
        // => tím by výsledné hodnocení filmu zůstalo vždy aktuální
        //Níže ukázáno na uložení nového ratingu.
        private void StoreRating()
        {
            _store.OnAfterSaveChanges += this.OnAfterSaveChanges;
            using (IDocumentSession session = _store.OpenSession())
            {
                Ratings r = new Ratings() { Id = "test_rating", MovieId = "Movies/100001", Rating = 5.0, Timestamp = DateTime.Now.ToString(), UserId = "myUser" };
                session.Store(r);
                session.SaveChanges(); //vyvolání události
            }
        }

        private void OnAfterSaveChanges(object sender, AfterSaveChangesEventArgs args)
        {
            if (args.Entity is Ratings)
            {
                var rating = args.Entity as Ratings;
                using (IDocumentSession session = _store.OpenSession())
                {
                    //získání hodnoty z indexu
                    double average = session.Query<Rating_AverageForMovie.Result, Rating_AverageForMovie>().Where(x => x.MovieId.Equals(rating.MovieId)).FirstOrDefault().Rating_Average;
                    //aktualizace ratingu u filmu
                    session.Advanced.Patch<Movie, double>(rating.MovieId, x => x.AverageRating, average);
                    session.SaveChanges();
                }
            }
        }
        #endregion
    }
}

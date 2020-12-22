using Raven.Client.Documents.Indexes;
using SemestralniPrace_MovieLens.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemestralniPrace_MovieLens.DAL.Indexes
{
    public class Movie_GenreAndRatingSearch : AbstractIndexCreationTask<Movie, Movie_GenreAndRatingSearch.Result>
    {
        public class Result
        {
            public object Query { get; set; }
            public double AverageRating { get; set; }
        }

        public Movie_GenreAndRatingSearch()
        {
            Map = movies => from movie in movies
                            select new Result
                            {
                                AverageRating = movie.AverageRating,
                                Query = new object[]
                                   {
                                       movie.Genres
                                   }
                            };

            Index("Query", FieldIndexing.Search);
        }
    }
    
}

using Raven.Client.Documents.Indexes;
using SemestralniPrace_MovieLens.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemestralniPrace_MovieLens.DAL.Indexes
{
    public class Ratings_ByMovieIdAndUserId : AbstractIndexCreationTask<Ratings, Ratings_ByMovieIdAndUserId.Result>
    {
        public class Result
        {
            public string UserId { get; set; }
            public string MovieId { get; set; }
        }

        public Ratings_ByMovieIdAndUserId()
        {
            Map = ratings => from rating in ratings
                             select new Result
                             {
                                 UserId = rating.UserId,
                                 MovieId = rating.MovieId,
                             };
        }

    }
}

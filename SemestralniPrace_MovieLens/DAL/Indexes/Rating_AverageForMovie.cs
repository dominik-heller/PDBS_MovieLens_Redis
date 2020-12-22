using Raven.Client.Documents.Indexes;
using SemestralniPrace_MovieLens.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemestralniPrace_MovieLens.DAL.Indexes
{
    public class Rating_AverageForMovie : AbstractIndexCreationTask<Ratings, Rating_AverageForMovie.Result>
    {
        public class Result
        {
            public string MovieId { get; set; }
            public int Rating_Count { get; set; }
            public double Rating_Sum { get; set; }
            public double Rating_Average { get; set; }
        }

        public Rating_AverageForMovie()
        {
            Map = ratings => from rating in ratings
                             select new Result
                             {
                                 MovieId = rating.MovieId,
                                 Rating_Sum = rating.Rating,
                                 Rating_Count = 1,
                                 Rating_Average = 0
                             };
            Reduce = results => from result in results
                                group result by result.MovieId into g
                                let ratingCount = g.Sum(x=>x.Rating_Count) 
                                let ratingSum = g.Sum(x=>x.Rating_Sum) 
                                select new Result
                                {
                                   MovieId = g.Key,
                                   Rating_Sum =ratingSum,
                                   Rating_Count=ratingCount,
                                   Rating_Average=ratingSum / ratingCount
                                };

        }
    }
}

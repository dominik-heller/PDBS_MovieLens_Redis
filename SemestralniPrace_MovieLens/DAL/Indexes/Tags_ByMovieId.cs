using Raven.Client.Documents.Indexes;
using SemestralniPrace_MovieLens.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemestralniPrace_MovieLens.DAL.Indexes
{
    public class Tags_ByMovieId : AbstractIndexCreationTask<Tags>
    {
        public class Result
        {
            public string MovieId { get; set; }
        }

        public Tags_ByMovieId()
        {
            Map = Tags => from tag in Tags
                             select new Result
                             {
                                 MovieId = tag.MovieId
                             };
        }
    }
}

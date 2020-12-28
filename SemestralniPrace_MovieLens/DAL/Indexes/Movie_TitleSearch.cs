using Raven.Client.Documents.Indexes;
using SemestralniPrace_MovieLens.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemestralniPrace_MovieLens.DAL.Indexes
{
    public class Movie_TitleSearch : AbstractIndexCreationTask<Movie, Movie_TitleSearch.Result>
    {
        public class Result
        {
            public string Title { get; set; }
        }

        public Movie_TitleSearch()
        {
            Map = movies => from movie in movies
                             select new Result
                             {
                                    Title=movie.Title
                             };

            Index(x=>x.Title, FieldIndexing.Search);
        }
    }
}

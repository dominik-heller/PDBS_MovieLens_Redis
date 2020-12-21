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
            public object Query { get; set; }
        }

        public Movie_TitleSearch()
        {
            Map = movies => from movie in movies
                             select new Result
                             {
                                 Query = new object[]
                                    {
                                    movie.Title
                                    //možno přidat více parametrů podle čeho vyhledávat
                                    }
                             };

            Index("Query", FieldIndexing.Search);
        }
    }
}

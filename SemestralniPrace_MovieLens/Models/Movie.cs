using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemestralniPrace_MovieLens.Models
{
    public class Movie
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Genres { get; set; }
        public IList<Link> Links { get; set; }
    }
}

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
        private IList<Link> links;
        public IList<Link> Links
        {
            get { if (links == null) { return links = new List<Link>(); } return links; }
            set { links = value; }
        }
        public double AverageRating { get; set; }
    }
}

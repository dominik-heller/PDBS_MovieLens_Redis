using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemestralniPrace_MovieLens.ViewModels
{

    public class SimilarUser
    {
        public string UserId { get; set; }
        public int CountOfRatings { get; set; }
        public double AverageRating { get; set; }
    }


}

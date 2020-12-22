using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemestralniPrace_MovieLens.Models
{
    //musí se to jmenovat ratings => protože property Rating má stejné jméno (a jiný název třídy než Rating/Ratings se nenamapuje na ravendb kolekci Rating)
    public class Ratings
    {
        public string Id { get; set; }
        public double Rating { get; set; }
        public string Timestamp { get; set; }
        public string MovieId { get; set; }
        public string UserId { get; set; }
    }
}

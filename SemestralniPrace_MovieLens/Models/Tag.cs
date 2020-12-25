using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemestralniPrace_MovieLens.Models
{
    public class Tags
    {
        public string Tag { get; set; }
        public string Timestamp { get; set; }
        public string MovieId { get; set; }
        public string UserId { get; set; }
    }
}

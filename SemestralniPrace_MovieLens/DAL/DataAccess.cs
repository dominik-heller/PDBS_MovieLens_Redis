using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using SemestralniPrace_MovieLens.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemestralniPrace_MovieLens.DAL
{
    public class DataAccess : IDataAccess
    {
        private readonly IDocumentStore _store;
        public DataAccess(IDocumentStore store)
        {
            _store = store;
            _store.Initialize();
        }

        public Movie GetMovie(string id)
        {
            using (IDocumentSession session = _store.OpenSession())
            {
                Movie t = session.Load<Movie>(id);
                return t;
            }
        }
    }
}

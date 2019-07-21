using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TextPrediction.Models
{
    public class SearchContext: DbContext
    {
        public SearchContext(DbContextOptions<SearchContext> options) : base(options)
        { }

        public DbSet<Search> Searches { get; set; }


    }

    public class Search
    {
        public int SearchId { get; set; }

        public string Query { get; set; }

        public string Author { get; set; }
    }

}

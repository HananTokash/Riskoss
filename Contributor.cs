using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitHubAPISandbox
{
    public class Contributor
    {
        public Author author { get; set; }
        public int total { get; set; }
        public List<Week> week { get; set; }

        public Contributor(Author author, int total, List<Week> week)
        {
            this.author = author;
            this.total = total;
            this.week = week;
        }

    }
}

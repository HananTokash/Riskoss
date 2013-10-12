using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.IO;
using ServiceStack.Text;

namespace GitHubAPISandbox
{
    class Program
    {
        static void Main(string[] args)
        {

            QueryStatistics query = new QueryStatistics();

            query.QueryContributor("xwiki", "xwiki-platform");
            query.GetListCommitsOnARepository("xwiki", "xwiki-platform");
            query.GetLinesAddedDeleted();

        }

        
    }
}

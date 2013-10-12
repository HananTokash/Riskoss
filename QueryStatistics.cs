using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.IO;
using ServiceStack.Text;

namespace GitHubAPISandbox
{
    public class QueryStatistics
    {
        public readonly HttpBasicAuthenticator Auth = new HttpBasicAuthenticator("tokash", "tokash30");

        public Contributor QueryContributor(string user, string repository)
        {
            var client = new RestClient("https://api.github.com");
            var request = new RestRequest();
            int counter = 0;
            bool isCompleted = false;
            List<Contributor> contributors = new List<Contributor>();

            //request.Resource = String.Format("repos/{0}/{1}/stats/contributors", user, repository);
            //request.RequestFormat = DataFormat.Json;

            //IRestResponse response = client.Execute(request);

            //if (response.Headers[0].Value.ToString() == "{Status=202 Accepted}")
            //{
            //    while (response.Headers[0].Value.ToString() == "{Status=202 Accepted}")
            //    {
            //        System.Threading.Thread.Sleep(500);
            //        counter++;

            //        //if no results were received after 2.5 seconds abort the query
            //        if (counter == 5)
            //        {
            //            isCompleted = false;
            //            break;
            //        }

            //        response = client.Execute(request);
            //    }
            //}
            //else
            //{
            //    isCompleted = true;
            //}

            ////checking if github returned actual results, if so, need to deserialize
            //if (isCompleted == true)
            //{
            //    var jsonObj = JsonSerializer.DeserializeFromString<List<ServiceStack.Text.JsonObject>>(response.Content);

            //    foreach (var item in jsonObj)
            //    {
            //        var author = JsonSerializer.DeserializeFromString<List<ServiceStack.Text.JsonObject>>(item.Child("author"));

            //        List<Author> authors = author.ConvertAll(x => new Author
            //        {
            //            login = x.Get("login"),
            //            id = int.Parse(x.Get("id")),
            //            avater_url = x.Get("avatar_id"),
            //            gravater_id = x.Get("gravatar_id"),
            //            url = x.Get("url"),
            //        });

            //        int total = int.Parse(item.Child("total"));

            //        var jsonWeek = JsonSerializer.DeserializeFromString<List<ServiceStack.Text.JsonObject>>(item.Child("weeks"));
            //        List<Week> week = jsonWeek.ConvertAll(x => new Week
            //        {
            //            sTicks = x.Get("w"),
            //            ticks = long.Parse(x.Get("w")),
            //            weekBinary = DateTime.FromBinary(long.Parse(x.Get("w"))),
            //            weekFileTime = DateTime.FromBinary(long.Parse(x.Get("w"))),
            //            weekFileTimeUTC = DateTime.FromBinary(long.Parse(x.Get("w"))),
            //            weekOADate = DateTime.FromBinary(long.Parse(x.Get("w"))),

            //            //week = new DateTime(ticks),
            //            additions = int.Parse(x.Get("a")),
            //            deletions = int.Parse(x.Get("d")),
            //            commits = int.Parse(x.Get("c")),
            //        });

            //        contributors.Add(new Contributor(authors.ElementAt(0), total, week));
            //        var YearlyCommitActivity = GetYearlyCommitActivity(user, repository);
            //        //int total = total.ConvertAll( x => int.Parse(x.Get("total")));
            //    }
            //    //var total;
            //    //var week;
            //}



            return null;
        }

        public void GetListCommitsOnARepository(string user, string repository)
        {
            var client = new RestClient("https://api.github.com");
            var request = new RestRequest();
            List<String> commitSha = null;
            DateTime since = new DateTime(2012, 8, 1);
            DateTime until = new DateTime(2013, 8, 1);

            commitSha = QueryCommits(user, repository, since, until, Auth);

            using (StreamWriter csvRequiredCommitDetails = new StreamWriter("CommitsExtractedData.csv", true))
            {
                string csv = "Sha ,";
                csv += "Additions ,";
                csv += "Deletions ,";
                csv += "Modifications ,";
                csv += "Date ,";
                csv += "Hour ,";
                csv += "WeekDay ,";
                csv += "MonthDay ,";
                csv += "Month, ";
                csv += "Total Commits During 1.8.2012 to 26.9.2013";

                csvRequiredCommitDetails.WriteLine(csv);
            }

            foreach (var sha in commitSha)
            {
                var Commit = GetSingleCommit(user, repository, sha, Auth);
                

                var stats = JsonSerializer.DeserializeFromString<List<ServiceStack.Text.JsonObject>>(Commit[0].Child("stats"));
                var CommitDetails = JsonSerializer.DeserializeFromString<List<ServiceStack.Text.JsonObject>>(Commit[0].Child("commit"));

                    
                var Author = JsonSerializer.DeserializeFromString<List<ServiceStack.Text.JsonObject>>(CommitDetails[0].Child("author"));

                List<CommitUser> author = Author.ConvertAll(x => new CommitUser
                {
                    date = System.DateTime.Parse(x.Get("date")),
                    email = x.Get("email"),
                    name = x.Get("name"),
                });

                List<CommitDetails> RequiredCommitDetails = stats.ConvertAll(x => new CommitDetails
                {
                    Sha = sha.ToString(),
                    Additions = int.Parse(x.Get("additions")),
                    Deletions = int.Parse(x.Get("deletions")),
                    Modifications = int.Parse(x.Get("total")),
                    Date = author[0].date.ToString(),
                    Hour = author[0].date.TimeOfDay,
                    WeekDay = author[0].date.DayOfWeek,
                    MonthDay = author[0].date.Day,
                    Month = author[0].date.Month,
                });

                using (StreamWriter csvRequiredCommitDetails = new StreamWriter("CommitsExtractedData.csv", true))
                {
                    string csv = RequiredCommitDetails[0].Sha + ",";
                    csv += RequiredCommitDetails[0].Additions.ToString() + ",";
                    csv += RequiredCommitDetails[0].Deletions.ToString() + ",";
                    csv += RequiredCommitDetails[0].Modifications.ToString() + ",";
                    csv += RequiredCommitDetails[0].Date + ",";
                    csv += RequiredCommitDetails[0].Hour.ToString() + ",";
                    csv += RequiredCommitDetails[0].WeekDay.ToString() + ",";
                    csv += RequiredCommitDetails[0].MonthDay.ToString() + ",";
                    csv += RequiredCommitDetails[0].Month.ToString();
                    csvRequiredCommitDetails.WriteLine(csv);
                }
            }
            using (StreamWriter csvRequiredCommitDetails = new StreamWriter("CommitsExtractedData.csv", true))
            {
                string csv = " , , , , , , , , , " + commitSha.Count.ToString();
                csvRequiredCommitDetails.WriteLine(csv);
            }
        }

        public List<ServiceStack.Text.JsonObject> GetSingleCommit(string user, string repository, string sha, HttpBasicAuthenticator Auth)
        {
            var client = new RestClient("https://api.github.com");
            var request = new RestRequest();
            List<String> commitSha = new List<string>();

            request.Resource = String.Format("/repos/{0}/{1}/commits/{2}", user, repository, sha);
            request.RequestFormat = DataFormat.Json;
            client.Authenticator = Auth;

            IRestResponse response = client.Execute(request);

            var commit = JsonSerializer.DeserializeFromString<List<ServiceStack.Text.JsonObject>>(response.Content);

            return commit;
        }

        public List<ServiceStack.Text.JsonObject> GetYearlyCommitActivity(string user, string repository)
        {
            var client = new RestClient("https://api.github.com");
            var request = new RestRequest();
            List<String> commitSha = new List<string>();

            request.Resource = String.Format("/repos/{0}/{1}/stats/commit_activity", user, repository);
            request.RequestFormat = DataFormat.Json;

            IRestResponse response = client.Execute(request);

            var commitActivity = JsonSerializer.DeserializeFromString<List<ServiceStack.Text.JsonObject>>(response.Content);

            return commitActivity;
        }

        List<string> QueryCommits(string user, string repository, DateTime since, DateTime until, HttpBasicAuthenticator Auth)
        {
            List<string> commitSha = new List<string>();
            var client = new RestClient("https://api.github.com");
            var request = new RestRequest();

            request.Resource = String.Format("/repos/{0}/{1}/commits?since={2}", user, repository, since.ToString("yyyy-MM-dd HH':'mm':'ss"));
            request.RequestFormat = DataFormat.Json;

            client.Authenticator = Auth;

            IRestResponse response = client.Execute(request);
            var jsonObj = JsonSerializer.DeserializeFromString<List<ServiceStack.Text.JsonObject>>(response.Content);

            string Link = GetLink(response);
            bool isNext = GetRel(response);


            while (isNext)
            {
                foreach (var item in jsonObj)
                {
                    commitSha.Add(item.Get("sha"));
                }

                request.Resource = Link;
                request.RequestFormat = DataFormat.Json;

                response = client.Execute(request);

                jsonObj = JsonSerializer.DeserializeFromString<List<ServiceStack.Text.JsonObject>>(response.Content);
                Link = GetLink(response);
                isNext = GetRel(response);
            }

            return commitSha;
        }

        string GetLink(IRestResponse response)
        {
            string Link = string.Empty;

            if (response.Headers.Count > 7)
            {
                string[] LinkDetails = response.Headers[7].ToString().Split(';');
                Link = LinkDetails[0];
                Link = Link.Remove(0, 28);
                Link = Link.Remove(Link.Length - 1, 1);
            }
            else
            {
                Console.WriteLine("Error");
            }

            return Link; 
        }

        bool GetRel(IRestResponse response)
        {
            string[] LinkDetails = response.Headers[7].ToString().Split(';');
            string[] ParsedRel = LinkDetails[1].Split(',');
            string Rel = ParsedRel[0];

            return Rel.Contains("next");
        }
    
    }
}

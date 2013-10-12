using RestSharp;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GitHubAPISandbox
{
    public class QueryStatistics
    {
        //username and password --> Username: "tokash", Password: "tokash30"

        //public readonly HttpBasicAuthenticator Auth = new HttpBasicAuthenticator("**EnterUsername**", "**EnterPassword**");

        public readonly HttpBasicAuthenticator Auth = new HttpBasicAuthenticator("tokash", "tokash30");

        public Contributor QueryContributor(string user, string repository)
        {
            var client = new RestClient("https://api.github.com");
            var request = new RestRequest();
            //int counter = 0;
            //bool isCompleted = false;
            List<Contributor> contributors = new List<Contributor>();

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

        public void GetLinesAddedDeleted()
        {
            string WorkingDirectory = @"C:\Users\חנן טוקש\Documents\GitHub\xwiki-platform";

            string path = Environment.GetEnvironmentVariable("GIT", EnvironmentVariableTarget.User);
            Executable exec = new Executable();

            string batFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\LinesAddedDeletedPerCommit.bat";//@"d:\gitcommand.bat";

            CreateBatchFile(path);

            WriteStatsToBat(batFilePath, WorkingDirectory, exec);   

            ParseStatsToCsv(exec);  //Parse Lines of txt file to get number of added/deleted lines per commit  
        } //Get number of lines added/deleted per commit and write to csv

        public void CreateBatchFile(string FilePath)
        {
            if (!File.Exists(FilePath))
            {
                using (FileStream fs = File.Create(FilePath))
                {
                    fs.Close();
                }
            }
        }

        public void ParseStatsToCsv(Executable exec)
        {
            using (TextReader tr = new StreamReader(exec.StandardOutputFileName))
            {
                List<string> textFileLines = new List<string>();
                string[] nonParsedSha = null;
                string[] nonParsedChanges = null;
                string[] ParsedChanges = null;
                string[] ParsedInsertions = null;
                string[] ParsedDeletions = null;
                string FilesChanged = string.Empty;
                string LinesAdded = string.Empty;
                string LinesDeleted = string.Empty;
                string line = string.Empty;
                string sha = string.Empty;
                int ExceptionalCases = 0;
                int TotalLinesChanged = 0;

                int StdShaLength = 40;

                while ((line = tr.ReadLine()) != null) { textFileLines.Add(line); }

                while ((line = tr.ReadLine()) != null) { textFileLines.Add(line); }

                using (StreamWriter csvRequiredCommitDetails = new StreamWriter("CommitsChanges.csv", true))
                {
                    string csv = "Sha ,";
                    csv += "LinesAdded ,";
                    csv += "LinesDeleted ,";
                    csv += "TotalLinesChanged ,";
                    csv += "FilesChanged ,";
                    csv += "Exceptional Cases ,";
                    csvRequiredCommitDetails.WriteLine(csv);
                }

                foreach (string item in textFileLines)
                {

                    if (item.Contains("commit"))
                    {
                        nonParsedSha = item.Split(' ');
                        sha = nonParsedSha[1];
                    }

                    if (item.Contains("files changed"))
                    {
                        if (sha.Length == StdShaLength)
                        {

                            nonParsedChanges = item.Split(',');

                            ParsedChanges = nonParsedChanges[0].Split(' ');
                            FilesChanged = ParsedChanges[1];

                            if (item.Contains("insertions") || item.Contains("insertion"))
                            {
                                ParsedInsertions = nonParsedChanges[1].Split(' ');
                                LinesAdded = ParsedInsertions[1];
                            }
                            else
                            {
                                LinesAdded = "0";
                            }

                            if (item.Contains("deletions") || item.Contains("deletion"))
                            {
                                if (!(item.Contains("insertions") || item.Contains("insertion")))
                                {
                                    ParsedDeletions = nonParsedChanges[1].Split(' ');
                                    LinesDeleted = ParsedDeletions[1];
                                }
                                else
                                {
                                    ParsedDeletions = nonParsedChanges[2].Split(' ');
                                    LinesDeleted = ParsedDeletions[1];
                                }
                            }
                            else
                            {
                                LinesDeleted = "0";
                            }

                            TotalLinesChanged = Convert.ToInt32(LinesAdded) + Convert.ToInt32(LinesDeleted);

                            using (StreamWriter csvRequiredCommitDetails = new StreamWriter("CommitsChanges.csv", true))
                            {
                                string csv = sha + ",";
                                csv += LinesAdded + ",";
                                csv += LinesDeleted + ",";
                                csv += TotalLinesChanged + ",";
                                csv += FilesChanged + ",";
                                csvRequiredCommitDetails.WriteLine(csv);
                            }
                        }
                        else
                        {
                            ExceptionalCases++;
                        }
                    }
                }


                using (StreamWriter csvRequiredCommitDetails = new StreamWriter("CommitsChanges.csv", true))
                {
                    string csv = " , , , , , " + ExceptionalCases;
                    csvRequiredCommitDetails.WriteLine(csv);
                }

                //Console.WriteLine(string.Format("Exceptional Cases: {0}", ExceptionalCases)); 
            }
        }

        public void WriteStatsToBat(string FilePath, string WorkingDirectory, Executable exec)
        {
            using (StreamWriter sw = new StreamWriter(FilePath))
            {
                //sw.WriteLine(string.Format(@"""{0}"" log --pretty=tformat: --numstat", path));
                sw.WriteLine(string.Format(@"""{0}"" log --stat", FilePath));
                //sw.WriteLine(string.Format(@"""{0}"" log --author=""Marius Dumitru"" --pretty=tformat: --numstat | gawk '\{ add += $1 ; subs += $2 ; loc += $1 - $2 \} END \{ printf ""added lines: %s removed lines : %s total lines: %s\n"",add,subs,loc \}'", path));
                //sw.WriteLine("SET PATH= %PATH%;\"C:\\Program Files\\Git\\bin\\gawk.exe\"");
                //string s =  "\"" + path + "\"" + " log --author=\"Marius Dumitru\" --pretty=tformat: --numstat | gawk \"{ add += $1 ; subs += $2 ; loc += $1 - $2 } END { printf \"added lines: %s removed lines : %s total lines: %s\n,add,subs,loc }\"";
                //sw.WriteLine(s);
                sw.WriteLine("@echo done");
            }

            exec.ProgramFileName = FilePath;
            exec.WorkingDirectory = WorkingDirectory;
            exec.StandardOutputFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + Path.GetFileNameWithoutExtension(FilePath) + "output.txt";
            exec.Run();

            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Text;
using RestSharp;

namespace GitHubAPISandbox
{
    public class CommitParser
    {
        public int GetTotalCommits(string user, string repo)
        {
            var client = new RestClient("https://api.github.com");
            var request = new RestRequest();

            request.Resource = String.Format("repos/{0}/{1}/commits", user, repo);
            request.RequestFormat = DataFormat.Json;

            IRestResponse response = client.Execute(request);
            var jsonObj = JsonSerializer.DeserializeFromString<List<ServiceStack.Text.JsonObject>>(response.Content);

            return jsonObj.Count;
        }
    }
}

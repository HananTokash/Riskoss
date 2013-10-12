using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitHubAPISandbox
{
    public class Repository
    {
        public bool fork { get; set; }

        public bool hasDownloads { get; set; }

        public bool hasIssues { get; set; }

        public bool hasWiki { get; set; }

        public bool isPrivate { get; set; }

        public DateTime createdAt { get; set; }

        public DateTime pushedAt { get; set; }

        public DateTime updatedAt { get; set; }

        public int forks { get; set; }

        public long id { get; set; }

        public int openIssues { get; set; }

        public int size { get; set; }

        public int watchers { get; set; }

        public Repository parent { get; set; }

        public Repository source { get; set; }

        public String cloneUrl { get; set; }

        public String description { get; set; }

        public String homepage { get; set; }

        public String gitUrl { get; set; }

        public String htmlUrl { get; set; }

        public String language { get; set; }

        public String masterBranch { get; set; }

        public String mirrorUrl { get; set; }

        public String name { get; set; }

        public String sshUrl { get; set; }

        public String svnUrl { get; set; }

        public String url { get; set; }

        //private User owner;
    }
}

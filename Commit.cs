using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitHubAPISandbox
{
    public class Commit
    {
        public CommitUser author { get; set; }

        public CommitUser committer { get; set; }

        public int commentCount { get; set; }

        public List<Commit> parents { get; set; }

        public String message { get; set; }

        public String sha { get; set; }

        public String url { get; set; }

        public Tree tree { get; set; }

        public CommitDetails RequiredCommitDetails { get; set; }

    }

    public class Tree
    {
        public List<TreeEntry> tree { get; set; }

        public String sha { get; set; }

        public String url { get; set; }
    }

    public class TreeEntry
    {

        public long size { get; set; }

        public String mode { get; set; }

        public String path { get; set; }

        public String sha { get; set; }

        public String type { get; set; }

        public String url { get; set; }
    }

    public class CommitUser 
    {

	    public DateTime date;

        public String email;

        public String name;
    }

    public class CommitDetails
    {
        public string Sha;

        public int Additions;

        public int Deletions;

        public int Modifications;

        public string Date;

        public System.TimeSpan Hour;

        public System.DayOfWeek WeekDay;

        public int MonthDay;

        public int Month;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Reflection;


namespace MailingListReader
{
    class Program
    {
        //private static readonly string path = @"C:\Users\Tokashyo\Documents\Visual Studio 2010\Projects\MailingListReader\2013-september.txt";
        private static readonly string csvColumns = "Day, Messages";
        private static readonly string gzipExtractLocation = @"D:\Temp";
        private static readonly string txtExtension = @".txt";
        private static readonly string gzipExtension = @".gz";


        static void Main(string[] args)
        {

            //List<String> zippedFiles = WebCommon.GetLinksFromHtml(@"http://lists.xwiki.org/pipermail/devs/", "2012", gzipExtension);
            List<String> zippedFiles = WebCommon.GetLinksFromHtml(@"http://lists.xwiki.org/pipermail/devs/", "2013", gzipExtension);

            foreach (string file in zippedFiles)
            {
                if (!File.Exists(file))
                {
                    WebCommon.DownloadWebFile(file, Path.GetFileName(file));
                }
            }

            string currentRunningDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var files = Directory.EnumerateFiles(currentRunningDir, "*.gz", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(gzipExtension));

            foreach (var item in files)
            {
                GZIPUtililty.Decompress(new FileInfo(item), gzipExtractLocation);
            }

            List<String> rawMailingList;
            List<String> mailingList;
            List<MailMessage> mailMessages;
            List<String> csvLines = new List<string>();
            string thread = string.Empty;
            bool isFirst = true;
            MailingListParser mailingListParser = new MailingListParser();

            var mailingListFiles = Directory.EnumerateFiles(gzipExtractLocation, "*.txt", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(txtExtension));

            foreach (string file in mailingListFiles)
            {
                rawMailingList = TextFilesCommon.ReadTextFile(file);

                //TextFilesCommon.WriteTextFile(rawMailingList, Path.GetFileNameWithoutExtension(path));

                mailingList = MailingListParser.GetMessagesFromList(rawMailingList);

                mailMessages = mailingListParser.ParseMailMessage(mailingList);

                int[] mailCountPerDay = MailingListParser.GetMailCountPerDay(mailMessages);
                //List<MailMessage> SortedList = mailMessages.OrderBy(o => o.Date).ToList();

                for (int i = 0; i < mailCountPerDay.Length; i++)
                {
                    string s = (i+1) + "," + mailCountPerDay[i];
                    csvLines.Add(s);
                }

                TextFilesCommon.WriteCSVFile(Path.GetFileNameWithoutExtension(file) + ".csv", csvLines, csvColumns);
                csvLines.Clear();
            }

        }
    }
}

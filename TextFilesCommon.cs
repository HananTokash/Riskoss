using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MailingListReader
{
    public class TextFilesCommon
    {
        public static List<String> ReadTextFile(string path)
        {
            List<String> list = new List<string>();

            //read whole file to buffer
            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    list.Add(line + "\n");
                }
            }

            return list;
        }

        public static void WriteTextFile(List<String> i_lines, string o_OutputFilePath)
        {
            using (StreamWriter writer = new StreamWriter(o_OutputFilePath, true))
            {
                foreach (string item in i_lines)
                {
                    writer.WriteLine(item);
                }
            }
        }

        public static void WriteCSVFile(string o_CSVFilepath, List<String> i_Lines, string i_Columns)
        {
            using (StreamWriter csvRequiredCommitDetails = new StreamWriter(o_CSVFilepath, true))
            {
                csvRequiredCommitDetails.WriteLine(i_Columns);

                foreach (string line in i_Lines)
                {
                    csvRequiredCommitDetails.WriteLine(line);
                }
            }
        }
    }
}

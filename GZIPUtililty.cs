﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace MailingListReader
{
    class GZIPUtililty
    {
        public static void Compress(FileInfo fi)
        {
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                // Prevent compressing hidden and 
                // already compressed files.
                if ((File.GetAttributes(fi.FullName)
                    & FileAttributes.Hidden)
                    != FileAttributes.Hidden & fi.Extension != ".gz")
                {
                    // Create the compressed file.
                    using (FileStream outFile =
                                File.Create(fi.FullName + ".gz"))
                    {
                        using (GZipStream Compress = new GZipStream(outFile, CompressionMode.Compress))
                        {
                            // Copy the source file into 
                            // the compression stream.
                            inFile.CopyTo(Compress);

                            Console.WriteLine("Compressed {0} from {1} to {2} bytes.",
                                fi.Name, fi.Length.ToString(), outFile.Length.ToString());
                        }
                    }
                }
            }
        }

        public static void Decompress(FileInfo fi, string o_UnzipLocation)
        {
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                // Get original file extension, for example
                // "doc" from report.doc.gz.
                string curFile = fi.FullName;
                string origName = curFile.Remove(curFile.Length -
                        fi.Extension.Length);

                string unzipLocationFullPath = String.Format(@"{0}\{1}", o_UnzipLocation, Path.GetFileName(origName));

                //Create the decompressed file.
                if (Directory.Exists(Path.GetDirectoryName(unzipLocationFullPath)))
                {
                    if (!File.Exists(unzipLocationFullPath))
                    {
                        using (FileStream outFile = File.Create(unzipLocationFullPath))
                        {
                            using (GZipStream Decompress = new GZipStream(inFile,
                                    CompressionMode.Decompress))
                            {
                                // Copy the decompression stream 
                                // into the output file.
                                Decompress.CopyTo(outFile);

                                Console.WriteLine("Decompressed: {0}", fi.Name);
                            }
                        }
                    } 
                }
            }
        }
    }
}

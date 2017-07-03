using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;

namespace BS.API.Models
{
    public class FileWriter
    {
        public static void WriteFile(Byte[] bytes, string fullpath)
        {
            //FileStream fs = File.Create(fullpath, 2048);
            //BinaryWriter bw = new BinaryWriter(fs);
            //bw.Write(bytes);
            if (IsGZipHeader(bytes))
            {
                // Unzip
                string path = System.IO.Path.GetDirectoryName(fullpath);
                bytes = Decompress(bytes);
                //Console.WriteLine("NOT GZPI");
            }
            if (!File.Exists(fullpath))
            {
                TryWrite(bytes, fullpath);
            }
        }

        private static void TryWrite(byte[] bytes, string fullpath)
        {
            try
            {
                File.WriteAllBytes(fullpath, bytes);
            }
            catch (Exception ex)
            {
                // File in use, do nothing.
                //Console.WriteLine("FileExists: " + ExistsCount++);
            }
        }

        public static string ExtractPathFrom(string fullpath)
        {
            return fullpath.Substring(0, fullpath.LastIndexOf("\\"));
        }

        public static bool IsPDF(string fullpath)
        {
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(fullpath);
            line = file.ReadLine();
            if (line.Contains("PDF")) { file.Close(); return true; }
            file.Close();
            return false;
        }

        public static bool IsGZipHeader(byte[] arr)
        {
            return arr.Length >= 2 && arr[0] == 31 && arr[1] == 139;
        }

        public static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip),
                CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        static public void CompressDirectory(string directory, string output)
        {
            ZipFile.CreateFromDirectory(directory, output, CompressionLevel.Fastest, true);
        }

        public static string CreateOutputDirectory(string directoryRoot)
        {
            String directoryName = directoryRoot + "\\" + "CE" + DateTime.Now.ToFileTime();
            System.IO.Directory.CreateDirectory(directoryName);
            return directoryName;
        }

        public static void CompressFolderAndRemove(string fileName, string directoryName)
        {
            try
            {
                if (System.IO.File.Exists(fileName))
                {
                    fileName = DateTime.Now.ToFileTime() + ".zip";
                    Console.WriteLine("File exists, tempfile will be placed in CWD: " + fileName);
                }
                FileWriter.CompressDirectory(directoryName, fileName);
                //Success, remove directory.
                Directory.Delete(directoryName, true);
            }

            catch (Exception)
            {
                throw;
            }
        }
    }
}
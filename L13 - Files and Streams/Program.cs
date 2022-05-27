using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Lesson13
{
    public static class ProgramL13
    {
        public static void Main(string[] args)
        {
            string rootPathDir1 = @"D:\L13DIR\DIR1\";
            string rootPathDir2 = @"D:\L13DIR\DIR2\";

            List<string> filesDir1 = new List<string>(Directory.GetFiles(rootPathDir1, "*.*", SearchOption.AllDirectories));
            List<string> filesDir2 = new List<string>(Directory.GetFiles(rootPathDir2, "*.*", SearchOption.AllDirectories));

            List<Hash> filesHashDir1 = new List<Hash>();
            List<Hash> filesHashDir2 = new List<Hash>();


            void HashingFiles(List<string> filesDir, List<Hash> filesHashDir)
            {
                foreach (var file in filesDir)
                {
                    var fileHash = HashFile.HashingFiles(file);
                    filesHashDir.Add(new Hash() { filePath = file, hash = fileHash });
                }
            }
            HashingFiles(filesDir1, filesHashDir1);
            HashingFiles(filesDir2, filesHashDir2);

            //IF A FILE EXISTS IN DIR1 BUT NOT IN DIR2, IT SHOULD BE COPIED
            foreach (var hash in filesHashDir1)
            {
                try
                {
                    File.Copy(hash.filePath, $"{rootPathDir2}{Path.GetFileName(hash.filePath)}");
                }
                catch
                {
                    //IF A FILE EXISTS IN DIR1 AND IN DIR2, BUT CONTENT IS CHANGED, THEN FILE FROM DIR1 SHOULD OVERWRITE THE ONE FROM DIR2
                    File.Copy(hash.filePath, $"{rootPathDir2}{Path.GetFileName(hash.filePath)}", true);
                }
            }

            Console.ReadLine();
        }
    }

    public class Hash
    {
        public string hash;
        public string filePath;
    }

    public class HashFile
    {
        public static string HashingFiles(string rootPath)
        {

            byte[] buffer;
            int bytesRead;
            long size;
            long totalBytesRead = 0;

            using (Stream file = File.OpenRead(rootPath))
            {
                size = file.Length;
                using (HashAlgorithm hasher = MD5.Create())
                {
                    do
                    {
                        buffer = new byte[4096];
                        bytesRead = file.Read(buffer, 0, buffer.Length);
                        totalBytesRead += bytesRead;
                        hasher.TransformBlock(buffer, 0, bytesRead, null, 0);
                    }
                    while (bytesRead != 0);

                    hasher.TransformFinalBlock(buffer, 0, 0);

                    return MakeHashString(hasher.Hash);

                }
            }
        }

        public static string MakeHashString(byte[] hashBytes)
        {
            StringBuilder hash = new StringBuilder(32);
            foreach (byte b in hashBytes)
            {
                hash.Append(b.ToString("X2").ToLower());
            }
            return hash.ToString();
        }

    }

}
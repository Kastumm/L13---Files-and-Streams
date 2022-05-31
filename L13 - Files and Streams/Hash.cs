using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashFiles
{
    public class Hash
    {
        public string hash;
        public string filePath;

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

        public static void AddToHashList(List<string> filesDir, List<Hash> filesHashDir)
        {
            foreach (var file in filesDir)
            {
                var fileHash = Hash.HashingFiles(file);
                filesHashDir.Add(new Hash() { filePath = file, hash = fileHash });
            }
        }
    }
}

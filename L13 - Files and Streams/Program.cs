
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HashFiles;

namespace Lesson13
{

    public static class ProgramL13
    {

        public static void Main(string[] args)
        {
            string rootPathDir1 = @"D:\L13DIR\DIR1";
            string rootPathDir2 = @"D:\L13DIR\DIR2";

            DirectoryInfo d1 = new DirectoryInfo(rootPathDir1);
            DirectoryInfo d2 = new DirectoryInfo(rootPathDir2);

            DeleteFiles(d1, d2);
            CopyFiles(d1, d2);

            RunFolderWatcher(rootPathDir1);
            //RunFolderWatcher(rootPathDir2);

            Console.ReadLine();
        }
        private static void CopyFiles(DirectoryInfo d1, DirectoryInfo d2)
        {
            //IF A FILE EXISTS IN DIR1 BUT NOT IN DIR2, IT SHOULD BE COPIED
            foreach (var file in Directory.GetFiles(d1.FullName, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    File.Copy(file, $@"{d2.FullName}\{Path.GetFileName(file)}");
                }
                catch
                {
                    //IF A FILE EXISTS IN DIR1 AND IN DIR2, BUT CONTENT IS CHANGED, THEN FILE FROM DIR1 SHOULD OVERWRITE THE ONE FROM DIR2
                    File.Copy(file, $@"{d2.FullName}\{Path.GetFileName(file)}", true);
                }
            }
        }
        //IF A FILE EXISTS IN DIR2 BUT NOT IN DIR1, IT SHOULD BE REMOVED
        private static void DeleteFiles(DirectoryInfo d1, DirectoryInfo d2) //Finnaly fucking works
        {
            var d1Info = Directory.GetFiles(d1.FullName, "*.*", SearchOption.AllDirectories);
            foreach (var file in Directory.GetFiles(d2.FullName, "*.*", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(file);
                if (!d1Info.Contains($@"{d1.FullName}\{fileName}"))
                {
                    File.Delete(file);
                }
            }
        }
        //Workign here still...
        private static void RunFolderWatcher(string directoryPath)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(directoryPath);

            watcher.Path = directoryPath;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.Size;
            watcher.Filter = "*.*";

            watcher.Created += OnCreated;
            watcher.Changed += OnCreated;
            watcher.Deleted += OnDeleted;
            //watcher.Renamed += OnChanged;

            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

        }
        public static void OnCreated(object sender, FileSystemEventArgs e)
        {
            DirectoryInfo d1 = new DirectoryInfo(@"D:\L13DIR\DIR1");
            DirectoryInfo d2 = new DirectoryInfo(@"D:\L13DIR\DIR2");
            CopyFiles(d1,d2);
        }
        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            DirectoryInfo d1 = new DirectoryInfo(@"D:\L13DIR\DIR1");
            DirectoryInfo d2 = new DirectoryInfo(@"D:\L13DIR\DIR2");
            DeleteFiles(d1, d2);
        }

    }

}
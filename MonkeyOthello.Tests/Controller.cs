using MonkeyOthello.Learning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonkeyOthello.Tests
{
    class Controller
    {
        public static void Run()
        {
            //var start = 38;
            //var span = 4;
            var start = 54;
            var span = 1;
            for (var x = start; x <= 54; x += span)
            {
                CloseColosseumPlatforms();
                Thread.Sleep(5000);
                UpdateColosseumPlatforms();
                Thread.Sleep(5000);
                RunColosseumPlatforms(Enumerable.Range(x, span).ToArray());
                //wait until creating more than 10k items
                var i = 0;
                while (true)
                {
                    try
                    {
                        Console.WriteLine($"waited {i++} minute(s)");
                        Thread.Sleep(2000);
                        Console.WriteLine("check...");
                        var counts = CheckColosseumItemsCount(Enumerable.Range(x, span).ToArray());

                        if (counts.All(c => c > 10000))
                        {
                            break;
                        }
                    }catch(Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                CloseColosseumPlatforms();

                //awake
                Console.WriteLine($"Begin learning {x}");
                Learning(x, x + span-1);
                Console.WriteLine($"Done learning {x}");

                //backup last results
                Console.WriteLine($"Backup last results");
                DirectoryCopy(@"E:\projects\MonkeyOthello\tests\test-results\networks", 
                    $@"E:\projects\MonkeyOthello\tests\test-results\networks-{DateTime.Today:yyyyMMdd}", 
                    false);

                //copy results
                Console.WriteLine($"Copy results");
                var networksPath = Path.Combine(Environment.CurrentDirectory, @"networks\");
                DirectoryCopy(networksPath, @"E:\projects\MonkeyOthello\tests\test-results\networks", false);
                
                //backup last knowledge
                Console.WriteLine($"Backup last knowledge");
                DirectoryCopy(@"E:\projects\MonkeyOthello\tests\test-results\knowledge",
                    $@"E:\projects\MonkeyOthello\tests\test-results\knowledge-{DateTime.Today:yyyyMMdd}",
                    false);

                //copy results
                Console.WriteLine($"Copy knowledge");
                var knowledgePath = Path.Combine(Environment.CurrentDirectory, @"knowledge\");
                DirectoryCopy(knowledgePath, @"E:\projects\MonkeyOthello\tests\test-results\knowledge", false);
            }
        }

        public static void Learning(int from, int to)
        {
            DeepLearning.PrepareData($@"E:\projects\MonkeyOthello\tests\", from, to);
            DeepLearning.TrainAll(from, to);
            DeepLearning.TestAll(from, to);
        }


        private static int[] CheckColosseumItemsCount(params int[] numbers)
        {
            if (numbers == null || numbers.Length == 0)
            {
                Console.WriteLine("No ColosseumPlatforms.");
                return new int[0];
            }

            var counts = new List<int>();
            foreach (var i in numbers)
            {
                var targetPath = $@"E:\projects\MonkeyOthello\tests\k-dl-{i}\knowledge";
                if (Directory.Exists(targetPath))
                {
                    var count = Directory.GetFiles(targetPath)
                          .Select(file => File.ReadAllLines(file).Length)
                          .Sum();

                    Console.WriteLine($"{i}'s items: {count}");
                    counts.Add(count);
                }
            }

            return counts.ToArray();
        }

        private static void CloseColosseumPlatforms()
        {
            Console.WriteLine("closing all MonkeyOthello.Colosseum.Platform...");
            var processes = Process.GetProcessesByName("MonkeyOthello.Colosseum.Platform");
            Console.WriteLine($"total: {processes.Length}");
            foreach (var p in processes)
            {
                p.Kill();
                //p.Close();
                //p.Dispose();
            }
            Console.WriteLine("closed all MonkeyOthello.Colosseum.Platform.");
        }

        private static void UpdateColosseumPlatforms()
        {
            var releasePath = @"E:\projects\MonkeyOthello\MonkeyOthello.Colosseum.Platform\bin\Release";

            var dlls = Directory.GetFiles(releasePath, "*.dll");
            var exe = Path.Combine(releasePath, "MonkeyOthello.Colosseum.Platform.exe");
            var config = Path.Combine(releasePath, "MonkeyOthello.Colosseum.Platform.exe.config");
            var releaseFiles = new List<string>();
            releaseFiles.AddRange(dlls);
            releaseFiles.Add(exe);
            releaseFiles.Add(config);

            var targetBasePath = @"E:\projects\MonkeyOthello\tests";

            //Parallel.For(30, 54, i =>
            for (var i = 30; i <= 54; i++)
            {
                var targetFolder = $@"k-dl-{i}\";
                var targetPath = Path.Combine(targetBasePath, targetFolder);

                //copy relaese files
                Console.WriteLine($"{i}. copy files to {targetFolder}");

                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                foreach (var f in releaseFiles)
                {
                    var fileName = Path.GetFileName(f);
                    var targetFile = Path.Combine(targetPath, fileName);
                    File.Copy(f, targetFile, true);
                }

                //update config
                Console.WriteLine($"{i}. update config");
                var targetConfig = Path.Combine(targetPath, "MonkeyOthello.Colosseum.Platform.exe.config");

                var configContent = File.ReadAllText(targetConfig);
                configContent = configContent.Replace("{TrainDepth}", i.ToString());
                File.WriteAllText(targetConfig, configContent);

                //copy engines
                Console.WriteLine($@"{i}. copy engines to {targetFolder}\edax\");

                DirectoryCopy(Path.Combine(releasePath, @"edax\"),
                    Path.Combine(targetPath, @"edax\"),
                    true);

                //copy networks
                Console.WriteLine($@"{i}. copy networks to {targetFolder}\networks\");
                DirectoryCopy(Path.Combine(targetBasePath, @"test-results\networks\"),
                    Path.Combine(targetPath, @"networks\"),
                    true);

                Console.WriteLine($"{i}. done!");

            }
            //);

        }

        private static void RunColosseumPlatforms(params int[] numbers)
        {
            if (numbers == null || numbers.Length == 0)
            {
                Console.WriteLine("No ColosseumPlatforms.");
                return;
            }

            foreach (var i in numbers)
            {
                var exe = $@"E:\projects\MonkeyOthello\tests\k-dl-{i}\MonkeyOthello.Colosseum.Platform.exe";
                var process = new Process();
                process.StartInfo.FileName = exe;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(exe);
                process.Start();
                Console.WriteLine($"{i}. running.");
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}

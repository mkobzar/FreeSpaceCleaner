using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DiskCleaner
{
    internal class FreeSpaceCleaner
    {
        static void Main(string[] args)
        {
            if (!IsValidDirectoryPath(args[0]))
            {
                Usage();
                return;
            }
            var directory = args[0];

            if (!directory.EndsWith("\\"))
            {
                directory = directory + "\\";
            }


            var driveName = Path.GetPathRoot(directory);
            if (driveName == null)
            {
                Console.WriteLine("Path.GetPathRoot(Directory.GetCurrentDirectory()) == null");
                return;
            }

            var str = new StringBuilder();
            var fileSuffix = $"{directory}TmpZrr";
            var fileSize = 1024 * 1024;
            str.Append('1', fileSize);
            StreamWriter streamWriter = null;
            while (true)
            {
                try
                {
                    var drive = new DriveInfo(driveName);
                    var availableFreeSpace = drive.AvailableFreeSpace;
                    var j = fileSize * 1024 < availableFreeSpace ? 1024 : (int)availableFreeSpace / 1024;
                    if (fileSize > availableFreeSpace || j < 1) break;
                    var currentFileName = $"{fileSuffix}.{Guid.NewGuid()}";
                    streamWriter = new StreamWriter(currentFileName);
                    Console.WriteLine();  
                    for (var i = 0; i < j; i++)
                    {
                        streamWriter.Write(str);
                        if (i % 10 == 0)
                        {
                            Console.Write($"\r{currentFileName}   ....   {Math.Round((double)drive.AvailableFreeSpace / 1024 / 1024 / 1024, 2)} GB remaining          ");
                        }
                    }
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    if(!String.IsNullOrEmpty(e.Message) && e.Message.ToLower().Contains(" is denied"))
                    {
                        return;
                    }
                }
            }
            try
            {
                if (streamWriter != null)
                {
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            catch
            {

            }
            Console.WriteLine($"AvailableFreeSpace now: {new DriveInfo(driveName).AvailableFreeSpace} bites");
            while (true)
            {
                Console.WriteLine("hit 'y' to clean the disk");
                if (Console.ReadLine() == "y") break;
            }
            Console.WriteLine("cleaning free space");
            var fileList = Directory.GetFiles(".\\", $"{fileSuffix}*").ToList();
            fileList.ForEach(x =>
            {
                try
                {
                    File.Delete(x);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"cannot delete {x} file");
                    Console.WriteLine(e.Message);
                }
            });
            Console.Write($"\rDone. Free space on {driveName} drive is clean now                  ");
        }

        static void Usage()
        {
            Console.Write($"\r\nSecurely Wipe Free Disk Space");
            Console.Write($"\r\nFreeSpaceCleaner [drive:][path]");
        }

        public static bool IsValidDriveLetter(string drive)
        {
            if (string.IsNullOrEmpty(drive))
                return false;

            // Check if length is 1 (e.g., "C") or 2 (e.g., "C:")
            if (drive.Length != 1 && drive.Length != 2)
                return false;

            // Check if first character is a letter (A-Z, case-insensitive)
            if (!char.IsLetter(drive[0]))
                return false;

            // If length is 2, the second character must be ':'
            if (drive.Length == 2 && drive[1] != ':')
                return false;

            // Optionally: Check if the drive actually exists on the system
            // var drives = DriveInfo.GetDrives();
            // return drives.Any(d => d.Name.StartsWith(drive, StringComparison.OrdinalIgnoreCase));

            return true;
        }

        public static bool IsValidDirectoryPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            // Check for invalid path characters
            if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return false;

            if (IsValidDriveLetter(path))
            {
                return true;
            }

            try
            {
                // Optionally: Check if it's an absolute or rooted path
                if (!Path.IsPathRooted(path))
                    return false; // If you only want full paths (e.g., "C:\Folder" or "/home")

                // Optionally: Extract directory name (removes trailing file if any)
                string dirName = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(dirName))
                    return false;

                return true; // Path format is valid
            }
            catch (Exception) // Catches ArgumentException, PathTooLongException, etc.
            {
                return false;
            }
        }
    }
}

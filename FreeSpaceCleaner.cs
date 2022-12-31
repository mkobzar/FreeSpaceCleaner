using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DiskCleaner
{
    internal class FreeSpaceCleaner
    {
        static void Main()
        {
            var driveName = Path.GetPathRoot(Directory.GetCurrentDirectory());
            if (driveName == null)
            {
                Console.WriteLine("Path.GetPathRoot(Directory.GetCurrentDirectory()) == null");
                return;
            }

            var str = new StringBuilder();
            var fileSuffix = "TmpZrr";
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

                    for (var i = 0; i < j; i++)
                    {
                        streamWriter.Write(str);
                        if (i % 10 == 0)
                        {
                            Console.Write($"\r{Math.Round((double)drive.AvailableFreeSpace / 1024 / 1024 / 1024, 2)} GB remaining          ");
                        }
                    }
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
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
    }
}

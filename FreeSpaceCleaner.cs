using System;
using System.IO;
using System.Linq;
using System.Text;

namespace freeSpaceCleaner
{
    internal class FreeSpaceCleaner
    {
        static void Main()
        {
            var driveName = Path.GetPathRoot(Directory.GetCurrentDirectory());
            var str = new StringBuilder();
            var fileSuffix = "TmpZrr";
            var fileCopuint = 0;
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
                    var currentFileName = $"{fileSuffix}{fileCopuint++}";
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
                streamWriter.Flush();
                streamWriter.Close();
            }
            catch
            {

            }
            Console.Write("\rcleaning free space                ");
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

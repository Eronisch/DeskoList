using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace Core.Business.File
{
    public static class FileService
    {
        private class Folders
        {
            public string Source { get; private set; }
            public string Target { get; private set; }

            public Folders(string source, string target)
            {
                Source = source;
                Target = target;
            }
        }

        public static string GetBaseDirectory()
        {
            return HostingEnvironment.ApplicationPhysicalPath;
        }

        public static bool DoesDirectoryExist(string path)
        {
            return Directory.Exists(path);
        }

        public static string GetBinPath()
        {
            return Path.Combine(GetBaseDirectory(), "bin");
        }

        public static void CopyFile(string sourceFile, string destionationFile)
        {
            if (FileExists(sourceFile))
            {
                DeleteFile(destionationFile);
                System.IO.File.Copy(sourceFile, destionationFile);
            }
        }

        /// <summary>
        /// Moves the file to the destination path and replaces it
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destionationFile"></param>
        public static void MoveFile(string sourceFile, string destionationFile)
        {
            DeleteFile(destionationFile);

            System.IO.File.Move(sourceFile, destionationFile);
        }

        public static IEnumerable<string> GetAllowedImageExtensions()
        {
            return new[]
            {
                ".png",
                ".jpeg",
                ".jpg",
                ".bmp",
                ".gif"
            };
        }

        public static bool IsValidSize(Stream stream, int maxMb)
        {
            if (stream == null) { return false; }

            const int mbBytes = 1048576;

            double maxBytes = maxMb * mbBytes;

            return maxBytes >= stream.Length;
        }

        /// <summary>
        /// Saves the file at the specified location
        /// If the path doesn't exist it will be created
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="directoryPath"></param>
        /// <param name="fileName"></param>
        public static void SaveFile(Stream stream, string directoryPath, string fileName)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(GetBaseDirectory() + directoryPath);
            }

            using (var fileStream = System.IO.File.Create(string.Format("{0}{1}/{2}", GetBaseDirectory(), directoryPath, fileName)))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
        }

        /// <summary>
        /// Saves the file at the specified location
        /// If the path doesn't exist it will be created
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        public static void SaveFile(Stream stream,  string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (var fileStream = System.IO.File.Create(path))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
        }

        /// <summary>
        /// Creates the path if it doesn't exist yet
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Removes the directory if it exists
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive"></param>
        public static void RemoveDirectory(string path, bool recursive)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive);
            }
        }

        /// <summary>
        /// Remove the dll from the bin directory if the dll exists
        /// </summary>
        /// <param name="fileName"></param>
        public static void RemoveFileInBin(string fileName)
        {
            DeleteFile(Path.Combine(GetBaseDirectory(), "bin", fileName));
        }

        /// <summary>
        /// Deletes the file if it exists
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        /// <summary>
        /// Check if the file exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool FileExists(string path)
        {
            return System.IO.File.Exists(path);
        }

        /// <summary>
        /// Get all the text content from the file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileContent(string path)
        {
            return System.IO.File.ReadAllText(path);
        }

        /// <summary>
        /// Saves all the text to the file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileContent"></param>
        public static void UpdateFile(string path, string fileContent)
        {
            System.IO.File.WriteAllText(path, fileContent);
        }

        /// <summary>
        /// Copies the files to the new location and deletes the old files
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void MoveDirectory(string source, string target)
        {
            var stack = new Stack<Folders>();
            stack.Push(new Folders(source, target));

            while (stack.Count > 0)
            {
                var folders = stack.Pop();
                Directory.CreateDirectory(folders.Target);
                foreach (var file in Directory.GetFiles(folders.Source, "*.*"))
                {
                    string targetFile = Path.Combine(folders.Target, Path.GetFileName(file));
                    if (System.IO.File.Exists(targetFile)) System.IO.File.Delete(targetFile);
                    System.IO.File.Move(file, targetFile);
                }

                foreach (var folder in Directory.GetDirectories(folders.Source))
                {
                    stack.Push(new Folders(folder, Path.Combine(folders.Target, Path.GetFileName(folder))));
                }
            }
        }

        /// <summary>
        /// Removes invalid characters and replaces spaces with an underline
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CleanFileName(string fileName)
        {
            fileName = fileName.Replace(" ", "_");

            return Path.GetInvalidFileNameChars().Aggregate(fileName, (input, c)
                => input.Replace(c.ToString(), string.Empty));
        }

        /// <summary>
        /// Gets all the files in the given path in the top directory
        /// Returns an empty array when the path doesn't exist
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFiles(string path)
        {
            if (!DoesDirectoryExist(path)) { return new string[0];}

            return Directory.GetFiles(path);
        }

        /// <summary>
        /// Gets all the files in the given path
        /// Returns an empty array when the path doesn't exist
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern">Example: *.dll</param>
        /// <returns></returns>
        public static IEnumerable<string> GetFiles(string path, string searchPattern)
        {
            if (!DoesDirectoryExist(path)) { return new string[0]; }

            return Directory.GetFiles(path, searchPattern);
        }

        /// <summary>
        /// Gets all the files in the given path
        /// Returns an empty array when the path doesn't exist
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern">Example: *.dll</param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            if (!DoesDirectoryExist(path)) { return new string[0]; }

            return Directory.GetFiles(path, searchPattern, searchOption);
        }
    }
}

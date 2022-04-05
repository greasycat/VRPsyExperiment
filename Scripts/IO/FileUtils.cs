using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleFileBrowser;
using UnityEngine;

namespace IO
{
    public static class FileUtils
    {
        public static string ReadAll(string filename)
        {
            var results = "";
            using var fs  = File.OpenRead(filename);
            using var reader = new StreamReader(fs);
            results = reader.ReadToEnd();
            reader.Close();
            fs.Close();
            return results;
        }

        public static void WriteAll(string filename, string content, bool truncate=true)
        {

            using var fs = truncate ? File.Create(filename) : File.OpenWrite(filename);
            using var writer = new StreamWriter(fs);
            writer.Write(content);
            writer.Close();
            fs.Close();
        }
        
        public static bool CheckIfFileExists(string filename)
        {
            return File.Exists(filename);
        }

        public static bool CheckIfDirExists(string dirname)
        {
            return Directory.Exists(dirname);
        }

        public static void GetSingleFileFromDialog(FileBrowser.OnSuccess callback)
        {
            FileBrowser.ShowLoadDialog(callback, null, FileBrowser.PickMode.Files, false);
        }

    }
}
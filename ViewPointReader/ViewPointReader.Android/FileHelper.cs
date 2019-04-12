using System;
using System.IO;
using ViewPointReader.Droid;
using ViewPointReader.Data.Interfaces;

using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]
namespace ViewPointReader.Droid
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }
    }
}
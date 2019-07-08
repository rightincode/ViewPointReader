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
            var pathObject = Android.App.Application.Context.GetExternalFilesDir(null);
            return Path.Combine(pathObject.Path, filename);
            
            //TODO: UNCOMMENT FOR RELEASE BUILD
            //var pathObject = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //return Path.Combine(pathObject, filename);
        }
    }
}
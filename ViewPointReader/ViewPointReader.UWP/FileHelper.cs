using System.IO;
using ViewPointReader.UWP;
using Windows.Storage;
using ViewPointReader.Data.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]
namespace ViewPointReader.UWP
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, filename);
        }
    }
}

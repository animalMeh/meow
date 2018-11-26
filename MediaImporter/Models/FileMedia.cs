using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace MediaImporter.Models
{
    public static class Icon
    {
        public static async Task<BitmapImage> GetIcon(StorageFile file)
        {
            switch (file.FileType)
            {
                case ".mp3":
                case ".mpeg":
                    return new BitmapImage(new Uri("../Assets/MyLogo/note.png"));
                case ".jpg":
                case ".png":
                case ".jpeg":
                    BitmapImage b = new BitmapImage();
                    b.SetSource(await file.OpenAsync(FileAccessMode.Read));
                    return b;
                default: return null;
            }
        }
    }
}

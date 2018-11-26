using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace MediaImporter.Models
{
    public class PortableDrive : INotifyPropertyChanged
    {
        private string name;

        private IReadOnlyList<StorageFile> files;
        private IReadOnlyList<StorageFolder> folders;
    
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public IReadOnlyList<StorageFile> Files
        {
            get
            {
                return files;
            }
            set
            {
                files = value;
                OnPropertyChanged("Files");
            }
        }

        public IReadOnlyList<StorageFolder> Folders
        {
            get
            {
                return folders;
            }
            set
            {
                folders = value;
                OnPropertyChanged("Folders");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async void GetFilesInFolder( StorageFolder folder , IReadOnlyList<StorageFile> files)
        {
           files = await folder.GetFilesAsync();    
        }

        public async Task<BitmapImage> GetIconOfFileAsync(StorageFile file)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(await file.OpenAsync(FileAccessMode.ReadWrite));
            return bitmapImage;
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public override bool Equals(object obj)
        {
            return (Name == ((PortableDrive)obj).Name &&
                Files == ((PortableDrive)obj).Files &&
                Folders == ((PortableDrive)obj).Folders && base.Equals(obj)) ;
                
        }
    }
}

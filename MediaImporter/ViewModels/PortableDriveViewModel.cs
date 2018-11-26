using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.IO;
using MediaImporter.Models;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Pickers;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml.Media;
using Windows.Devices.Enumeration;

namespace MediaImporter.ViewModels
{
    public class PortableDriveViewModel : INotifyPropertyChanged
    {
        
        private PortableDrive selectedDrive;
        private ObservableCollection<PortableDrive> portableDrives;
        public PortableDrive SelectedDrive
        {
            get { return selectedDrive; }
            set
            {
                selectedDrive = value;
                OnPropertyChanged("SelectedDrive");
            }
        }

        public ObservableCollection<PortableDrive> PortableDrives
        {
            get { return portableDrives; }
            set
            {
                portableDrives = value;
                OnPropertyChanged("PortableDrivers");
            }
        }

        public PortableDriveViewModel()
        {
            PortableDrives = new ObservableCollection<PortableDrive>();
            //AddNewOne();
          //    GenerateDrivers();
        }

        private async void GenerateDrivers()
        {
            IReadOnlyList<StorageFile> files = null;
            IReadOnlyList<StorageFolder> foldersIn = null;

            var drives = await Windows.Storage.KnownFolders.RemovableDevices.GetItemsAsync();
            if (portableDrives.Count < drives.Count)
            {
                foreach (var drive in drives)
                {
                    if (PortableDrives.Where(p => p.Name == drive.Name).Select(b => b).Count() != 0)
                    {
                    }
                    else
                    {
                        var currentDrive = new PortableDrive { Name = drive.Name, Files = files, Folders = foldersIn };
                        PortableDrives.Add(currentDrive);   
                        currentDrive.Files = await ((StorageFolder)drive).GetFilesAsync();
                        currentDrive.Folders = await ((StorageFolder)drive).GetFoldersAsync();
                   }
                }
            }
        }


        public void AddNewOne()
        {
            GenerateDrivers();
        }

        public async void Remove()
        {
            var drives = await Windows.Storage.KnownFolders.RemovableDevices.GetItemsAsync();
            if(drives.Count < PortableDrives.Count)
            {
                if (drives.Count == 0)
                    PortableDrives.Clear();
                else
                {
                    foreach (var drive in drives)
                    {
                        if (PortableDrives.Where(p => p.Name == drive.Name).Select(b => b).Count() != 0)
                        {
                            PortableDrives.Remove(PortableDrives.ToList().Find(P => P.Name == drive.Name));
                        }
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

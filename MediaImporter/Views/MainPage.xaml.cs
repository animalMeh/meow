using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using MediaImporter.ViewModels;
using System.Collections;
using System.Runtime.InteropServices;
using MediaImporter.Models;
using System.IO;
using Windows.Storage;
using System.Collections.Generic;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MediaImporter.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DeviceWatcher UsbWatcher = DeviceInformation.CreateWatcher(DeviceClass.PortableStorageDevice);
        bool IsUserUsesDrives = false;
        bool IsUserInjectDrive = false;
        PortableDriveViewModel Drives;

        public MainPage()
        {
            this.InitializeComponent();
            Drives = new PortableDriveViewModel();
            DataContext = Drives;
            BackButton.Visibility = Visibility.Collapsed;
            UsbWatcher.Added += AddWatcher;
            UsbWatcher.Removed += Inject;
            Show();
            IconsListView.SelectedValue = null;
            UsbWatcher.Updated += (sender, deviceInfo) =>
            {
                Console.Beep();

            };
            UsbWatcher.EnumerationCompleted += (sender, deviceInfo) =>
            {
                Console.Beep();

            };
            
            UsbWatcher.Start();  
        }

        private void HumbButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }
        
        public async void DriveFolders_ItemClick(object sender, ItemClickEventArgs e)
        {
        
            BackButton.Visibility = Visibility.Visible;

            IReadOnlyList<StorageFolder> Folders =  await (e.ClickedItem as StorageFolder).GetFoldersAsync();
            DriveFolders.ItemsSource = Folders;
            IReadOnlyList<StorageFile> Files = await (e.ClickedItem as StorageFolder).GetFilesAsync();
            FolderFiles.ItemsSource = Files;
            
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            DriveFolders.ItemsSource = (IconsListView.SelectedItem as PortableDrive).Folders;
            FolderFiles.ItemsSource = (IconsListView.SelectedItem as PortableDrive).Files;
        }

        private void IconsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsUserUsesDrives = true;
            Show();
            if (!IsUserInjectDrive)
            {
                DriveFolders.ItemsSource = (IconsListView.SelectedItem as PortableDrive).Folders;
                FolderFiles.ItemsSource = (IconsListView.SelectedItem as PortableDrive).Files;
            }
            else IsUserInjectDrive = false;
        }

        private async void Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image img = (sender as Image);
            var c = ((BitmapImage)img.Source).UriSource.LocalPath;
            string fileType = Path.GetExtension(c);

            switch (fileType)
            {
                case ".mp3":
                case ".mpeg":
                    img.Source = new BitmapImage(new Uri("ms-appx:///Assets/MyLogo/note.png"));
                    break;
                case ".jpg":
                case ".JPG":
                case ".png":
                case ".PNG":
                case ".jpeg":
                        var file = await StorageFile.GetFileFromPathAsync(c);
                        using (var stream = await file.OpenAsync(FileAccessMode.Read))
                        {
                            var bmp = new BitmapImage();
                            await bmp.SetSourceAsync(stream);
                            img.Source = bmp;
                        }
                    return;
                default:
                    break;
            }
        }

        private async void Inject(DeviceWatcher sender, DeviceInformationUpdate e)
        {
            await this.Dispatcher.TryRunAsync(CoreDispatcherPriority.High, () => {
                Drives.Remove();
                HomeButton_Click(new object(), new RoutedEventArgs());
                IsUserInjectDrive = true;
            });
        }

        private async void AddWatcher(DeviceWatcher sender, DeviceInformation e)
        {
            await this.Dispatcher.TryRunAsync(CoreDispatcherPriority.High, () => {
                Drives.AddNewOne();
            });
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            IsUserUsesDrives = false;
            Show();
        }

        private void Show()
        {
            if(IsUserUsesDrives)
            {
                EmptyNovel.Visibility = Visibility.Collapsed;
                DriveFolders.Visibility = Visibility.Visible;
                FolderFiles.Visibility = Visibility.Visible;
            }
            else
            {
                EmptyNovel.Visibility = Visibility.Visible;
                DriveFolders.Visibility = Visibility.Collapsed;
                FolderFiles.Visibility = Visibility.Collapsed;
            }
        }

        private void IconsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            IsUserUsesDrives = true;

            Show();
        }
    }
}

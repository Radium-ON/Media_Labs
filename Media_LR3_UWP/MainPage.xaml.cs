using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Media_LR3_UWP.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Media_LR3_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        TreeViewNode personalFolder;
        TreeViewNode personalFolder2;
        private ObservableCollection<ExplorerItem> DataSource;
        public ObservableCollection<ImageModel> ScreenCollectionImages { get; set; }
        public ObservableCollection<ImageSource> ImageSources { get; set; } = new ObservableCollection<ImageSource>();

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
            DataSource = GetData();

            //InitializeSampleTreeView();
        }

        private void InitializeSampleTreeView()
        {
            TreeViewNode workFolder = new TreeViewNode() { Content = "Work Documents" };
            workFolder.IsExpanded = true;

            workFolder.Children.Add(new TreeViewNode() { Content = "XYZ Functional Spec" });
            workFolder.Children.Add(new TreeViewNode() { Content = "Feature Schedule" });
            workFolder.Children.Add(new TreeViewNode() { Content = "Overall Project Plan" });
            workFolder.Children.Add(new TreeViewNode() { Content = "Feature Resources Allocation" });

            TreeViewNode remodelFolder = new TreeViewNode() { Content = "Home Remodel" };
            remodelFolder.IsExpanded = true;

            remodelFolder.Children.Add(new TreeViewNode() { Content = "Contractor Contact Info" });
            remodelFolder.Children.Add(new TreeViewNode() { Content = "Paint Color Scheme" });
            remodelFolder.Children.Add(new TreeViewNode() { Content = "Flooring woodgrain type" });
            remodelFolder.Children.Add(new TreeViewNode() { Content = "Kitchen cabinet style" });

            personalFolder = new TreeViewNode() { Content = "Personal Documents" };
            personalFolder.IsExpanded = true;
            personalFolder.Children.Add(remodelFolder);

            sampleTreeView.RootNodes.Add(workFolder);
            sampleTreeView.RootNodes.Add(personalFolder);
        }

        private void sampleTreeView_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            return;
        }

        private ObservableCollection<ExplorerItem> GetData()
        {
            var list = new ObservableCollection<ExplorerItem>();
            ExplorerItem folder1 = new ExplorerItem()
            {
                Name = "Work Documents",
                Type = ExplorerItem.ExplorerItemType.Folder,
                Children =
                {
                    new ExplorerItem()
                    {
                        Name = "Functional Specifications",
                        Type = ExplorerItem.ExplorerItemType.Folder,
                        Children =
                        {
                            new ExplorerItem()
                            {
                                Name = "TreeView spec",
                                Type = ExplorerItem.ExplorerItemType.File,
                              }
                        }
                    },
                    new ExplorerItem()
                    {
                        Name = "Feature Schedule",
                        Type = ExplorerItem.ExplorerItemType.File,
                    },
                    new ExplorerItem()
                    {
                        Name = "Overall Project Plan",
                        Type = ExplorerItem.ExplorerItemType.File,
                    },
                    new ExplorerItem()
                    {
                        Name = "Feature Resources Allocation",
                        Type = ExplorerItem.ExplorerItemType.File,
                    }
                }
            };
            ExplorerItem folder2 = new ExplorerItem()
            {
                Name = "Personal Folder",
                Type = ExplorerItem.ExplorerItemType.Folder,
                Children =
                        {
                            new ExplorerItem()
                            {
                                Name = "Home Remodel Folder",
                                Type = ExplorerItem.ExplorerItemType.Folder,
                                Children =
                                {
                                    new ExplorerItem()
                                    {
                                        Name = "Contractor Contact Info",
                                        Type = ExplorerItem.ExplorerItemType.File,
                                    },
                                    new ExplorerItem()
                                    {
                                        Name = "Paint Color Scheme",
                                        Type = ExplorerItem.ExplorerItemType.File,
                                    },
                                    new ExplorerItem()
                                    {
                                        Name = "Flooring Woodgrain type",
                                        Type = ExplorerItem.ExplorerItemType.File,
                                    },
                                    new ExplorerItem()
                                    {
                                        Name = "Kitchen Cabinet Style",
                                        Type = ExplorerItem.ExplorerItemType.File,
                                    }
                                }
                            }
                        }
            };

            list.Add(folder1);
            list.Add(folder2);
            return list;
        }

        private async void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageFolder folder = KnownFolders.PicturesLibrary;
                IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
                List<ImageModel> templist = new List<ImageModel>();
                for (var i = 0; i < files.Count; i++)
                {
                    StorageFile file = files[i];

                    var imageProps = await file.Properties.GetImagePropertiesAsync();
                    var source = await FromStorageFileAsync(file);
                    templist.Add(new ImageModel(file.DisplayName, source, imageProps.DateTaken.ToString(),
                        imageProps.Height + "x" + imageProps.Width, file.DateCreated.ToString()));
                    ImageSources.Add(source);
                }

                ScreenCollectionImages = new ObservableCollection<ImageModel>(templist);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

        public static async Task<ImageSource> FromStorageFileAsync(StorageFile sf)
        {
            using (var randomAccessStream = await sf.OpenReadAsync())
            {
                var result = new BitmapImage();
                await result.SetSourceAsync(randomAccessStream);
                return result;
            }
        }
    }

    public class ExplorerItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public enum ExplorerItemType { Folder, File };
        public string Name { get; set; }
        public ExplorerItemType Type { get; set; }
        private ObservableCollection<ExplorerItem> m_children;
        public ObservableCollection<ExplorerItem> Children
        {
            get
            {
                if (m_children == null)
                {
                    m_children = new ObservableCollection<ExplorerItem>();
                }
                return m_children;
            }
            set
            {
                m_children = value;
            }
        }

        private bool m_isExpanded;
        public bool IsExpanded
        {
            get { return m_isExpanded; }
            set
            {
                if (m_isExpanded != value)
                {
                    m_isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }

        private bool m_isSelected;
        public bool IsSelected
        {
            get { return m_isSelected; }

            set
            {
                if (m_isSelected != value)
                {
                    m_isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }

        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class ExplorerItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FolderTemplate { get; set; }
        public DataTemplate FileTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            var explorerItem = (ExplorerItem)item;
            return explorerItem.Type == ExplorerItem.ExplorerItemType.Folder ? FolderTemplate : FileTemplate;
        }

    }
}

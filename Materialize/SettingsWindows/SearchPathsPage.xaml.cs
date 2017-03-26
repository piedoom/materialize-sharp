using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Materialize.SettingsWindows
{
    /// <summary>
    /// Interaction logic for SearchPathsPage.xaml
    /// </summary>
    public partial class SearchPathsPage : Page
    {
        /// <summary>
        /// databound list that gives us all of the directories to search for textures
        /// </summary>
        ObservableCollection<string> searchDirectories = new ObservableCollection<string>();

        public SearchPathsPage()
        {
            InitializeComponent();

            // set the source of our directory list to our collection object
            searchDirectoryListView.ItemsSource = searchDirectories;

            // set our object to saved settings
            var settings = Load();
            for (var i = 0; i < settings.Length; i++)
            {
                searchDirectories.Add(settings[i]);
            }
            
            // save these settings whenever they are changed
            searchDirectories.CollectionChanged += SearchDirectories_CollectionChanged;
        }

        private void SearchDirectories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Save();
        }

        /// <summary>
        /// When the "Add Search Path" button is clicked, open a dialog to select a folder, 
        /// and then add it to our searchDirectories list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addSearchDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            // open up our folder dialog
            using (var fbd = new FolderBrowserDialog())
            {
                // set the result to our selection
                DialogResult result = fbd.ShowDialog();

                // if not null, do the thing we set out to do.  Else, just do nothing.
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    // basically all we're doing is adding the directory to our object, and then saving the setting
                    // check that the currently selected directory does not yet exist
                    if (!searchDirectories.Contains(fbd.SelectedPath))
                    {
                        searchDirectories.Add(fbd.SelectedPath);
                    }
                }
            }
        }

        /// <summary>
        /// Save the current settings
        /// </summary>
        private void Save()
        {
            DatabaseManager.SetDirectorySearchPaths(searchDirectories.ToArray());
        }

        /// <summary>
        /// Get saved settings
        /// </summary>
        /// <returns></returns>
        private string[] Load()
        {
            return DatabaseManager.GetDirectorySearchPaths();
        }
    }
}

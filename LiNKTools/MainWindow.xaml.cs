using LiNKTools.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LiNKTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SessionListViewModel ViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new SessionListViewModel();

            DataContext = ViewModel;
            //listBoxSessions.ItemsSource = Items;
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".sqlite";
            openFileDialog.Filter = "SQLite Databases (*.sqlite)|*.sqlite";

            if (openFileDialog.ShowDialog() == true)
                textBoxFilePath.Text = openFileDialog.FileName;
        }

        private void buttonTestConnection_Click(object sender, RoutedEventArgs e)
        {
            DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");
            using (SQLiteConnection cnn = (SQLiteConnection)fact.CreateConnection())
            {
                cnn.ConnectionString = "Data Source=" + textBoxFilePath.Text;
                cnn.Open();

                SQLiteCommand command = new SQLiteCommand(
                    "SELECT Name, StartTime FROM Sessions", cnn);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ViewModel.Sessions.Add(new Session((string)reader[0], (long)reader[1]));
                }

                reader.Close();
            }
        }
    }
}

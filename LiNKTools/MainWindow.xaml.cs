using LiNKTools.ViewModels;
using LiNKTools.Model;
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
using TcxTools;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Microsoft.Practices.Unity;
using LiNKTools.Services;

namespace LiNKTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IUnityContainer _container;

        public SessionListViewModel ViewModel { get; set; }
        public MainWindow(IUnityContainer container)
        {
            InitializeComponent();
            _container = container;

            ViewModel = container.Resolve<SessionListViewModel>();

            DataContext = ViewModel;
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            var fileService = _container.Resolve<IFileService>();

            string selectedFilePath = fileService.OpenFileDialog();
            if (!string.IsNullOrEmpty(selectedFilePath))
                ViewModel.FilePath = selectedFilePath;
        }

        private void buttonFetchSessions_Click(object sender, RoutedEventArgs e)
        {
            DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");
            using (SQLiteConnection cnn = (SQLiteConnection)fact.CreateConnection())
            {
                cnn.ConnectionString = "Data Source=" + ViewModel.FilePath;
                cnn.Open();

                SQLiteCommand command = new SQLiteCommand(
                    "SELECT PK_SessionId, Name, StartTime, TotalElapsedTime, TotalDistance, AverageStrokeRate, AverageHeartRate, AverageSpeed, SpeedInput FROM Sessions", cnn);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = (int)(long)reader[0];
                    string name = (string)reader[1];
                    DateTime startTime = NKDateTimeConverter.NKToClr((long)reader[2]);
                    int totalElapsedTime = (int)(long)reader[3];
                    int totalDistance = (int)(long)reader[4];
                    double strokeRate = Convert.ToDouble((long)reader[5]) / 2;
                    int averageHeartRate = reader.IsDBNull(6) ? 0 : (int)(long)reader[6];
                    double speed = ((double)reader[7]) / 100;
                    SpeedInput input = (SpeedInput)(long)reader[8];

                    ViewModel.Sessions.Add(new Session(id, name, startTime, totalElapsedTime, totalDistance, strokeRate, averageHeartRate, speed, input));
                }

                reader.Close();
            }
        }
    }
}

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

            ViewModel = new SessionListViewModel();

            DataContext = ViewModel;
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            var fileService = _container.Resolve<IFileService>();

            ViewModel.FilePath = fileService.OpenFileDialog();
        }

        private void buttonTestConnection_Click(object sender, RoutedEventArgs e)
        {
            DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");
            using (SQLiteConnection cnn = (SQLiteConnection)fact.CreateConnection())
            {
                cnn.ConnectionString = "Data Source=" + textBoxFilePath.Text;
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
                    SpeedInput input = (SpeedInput)reader[8];

                    ViewModel.Sessions.Add(new Session(id, name, startTime, totalElapsedTime, totalDistance, strokeRate, averageHeartRate, speed, input));
                }

                reader.Close();
            }
        }

        private void buttonExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".tcx";

            if (dialog.ShowDialog() == true)
            {
                Session session = (Session)listBoxSessions.SelectedItem;

                DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");
                using (SQLiteConnection cnn = (SQLiteConnection)fact.CreateConnection())
                {
                    cnn.ConnectionString = "Data Source=" + ViewModel.FilePath;
                    cnn.Open();

                    SQLiteCommand command = new SQLiteCommand(
                        "SELECT PK_IntervalId, StartTime, TotalElapsedTime, TotalDistance, AverageStrokeRate, AverageHeartRate, AverageSpeed, TotalStrokeCount FROM Intervals" +
                        " WHERE FK_SessionId = @sessionId", cnn);
                    SQLiteParameter sessionParam = new SQLiteParameter("sessionId", System.Data.DbType.Int32);
                    sessionParam.Value = session.Id;
                    command.Parameters.Add(sessionParam);

                    SQLiteCommand dataRecordCommand = new SQLiteCommand(
                        "SELECT PK_DataRecordId, ElapsedTime, Latitude, Longitude, SpeedGps, SpeedImpeller, DistanceGps, DistanceImpeller, StrokeRate, HeartRate" +
                        " FROM DataRecords WHERE FK_IntervalId = @intervalId AND Type = 0", cnn); // Type filtered to 0 to just pick up strokes

                    SQLiteParameter intervalParam = new SQLiteParameter("sessionId", System.Data.DbType.Int32);
                    command.Parameters.Add(intervalParam);

                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = (int)(long)reader[0];
                        DateTime startTime = NKDateTimeConverter.NKToClr((long)reader[1]);
                        int totalElapsedTime = (int)(long)reader[2];
                        int totalDistance = (int)(long)reader[3];
                        double strokeRate = Convert.ToDouble((long)reader[4]) / 2;
                        int averageHeartRate = reader.IsDBNull(5) ? 0 : (int)(long)reader[5];
                        double speed = Convert.ToDouble((long)reader[6]) / 100;
                        int totalStrokeCount = (int)(long)reader[7];

                        Interval interval = new Interval(id, startTime, totalElapsedTime, totalDistance, strokeRate, averageHeartRate, speed, totalStrokeCount);

                        intervalParam.Value = interval.Id;

                        SQLiteDataReader dataRecordReader = dataRecordCommand.ExecuteReader();

                        while (dataRecordReader.Read())
                        {
                            DataRecord record = new DataRecord();
                            record.ElapsedTime = (int)reader[1];
                            record.Latitude = (double)reader[2];
                            record.Longitude = (double)reader[3];
                            record.SpeedGps = Convert.ToDouble((long)reader[4]) / 100;
                            record.SpeedImpeller = Convert.ToDouble((long)reader[5]) / 100;
                            record.DistanceGps = (int)reader[6];
                            record.DistanceImpeller = (int)reader[7];
                            record.StrokeRate = Convert.ToDouble((long)reader[8]) / 2;
                            record.HeartRate = (int)reader[9];
                        }

                        session.Intervals.Add(interval);
                    }

                    reader.Close();
                }
                
                Activity activity = new Activity() { Id = session.StartTime, Sport = Sport.Other };
                activity.Creator = new Device() { Name = "SpeedCoach GPS" };

                foreach (Interval interval in session.Intervals) {
                    ActivityLap lap = interval.ToLap();

                    // TODO: Cadence/Stroke Rate (type conversion)
                    double maxSpeed = 0;
                    byte maxHeartRate = 0;

                    foreach (DataRecord record in interval.DataRecords)
                    {
                        Trackpoint trackpoint = record.ToTrackpoint(lap.StartTime);

                        if (trackpoint.HeartRateBpm.Value > maxHeartRate)
                            maxHeartRate = trackpoint.HeartRateBpm.Value;
                        
                        switch (session.SpeedInput)
                        {
                            case SpeedInput.Gps:
                                if (record.SpeedGps > maxSpeed)
                                    maxSpeed = record.SpeedGps;
                                break;
                            case SpeedInput.Impeller:
                                if (record.SpeedImpeller > maxSpeed)
                                    maxSpeed = record.SpeedImpeller;
                                break;
                        }
                        lap.Track.Add(trackpoint);
                    }

                    lap.MaximumSpeed = maxSpeed;
                    lap.MaximumHeartRateBpm = new HeartRateInBeatsPerMinute() { Value = maxHeartRate };

                    activity.Lap.Add(lap);
                }

                TrainingCenterDatabase tcd = new TrainingCenterDatabase();
                tcd.Activities.Activity.Add(activity);

                XmlSerializer serializer = new XmlSerializer(typeof(TrainingCenterDatabase));

                using (XmlWriter writer = XmlWriter.Create(dialog.FileName))
                {
                    serializer.Serialize(writer, tcd);
                }
            }
        }
    }
}

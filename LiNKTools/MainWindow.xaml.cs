﻿using LiNKTools.ViewModels;
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
                    "SELECT PK_SessionId, Name, StartTime, TotalElapsedTime, TotalDistance, AverageStrokeRate, AverageHeartRate, AverageSpeed FROM Sessions", cnn);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = (int)(long)reader[0];
                    string name = (string)reader[1];
                    long startTime = (long)reader[2];
                    int totalElapsedTime = (int)(long)reader[3];
                    int totalDistance = (int)(long)reader[4];
                    double strokeRate = Convert.ToDouble((long)reader[5]) / 2;
                    int averageHeartRate = reader.IsDBNull(6) ? 0 : (int)(long)reader[6];
                    double speed = ((double)reader[7]) / 100;

                    ViewModel.Sessions.Add(new Session(id, name, startTime, totalElapsedTime, totalDistance, strokeRate, averageHeartRate, speed));
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
                    cnn.ConnectionString = "Data Source=" + textBoxFilePath.Text;
                    cnn.Open();

                    SQLiteCommand command = new SQLiteCommand(
                        "SELECT PK_IntervalId, StartTime, TotalElapsedTime, TotalDistance, AverageStrokeRate, AverageHeartRate, AverageSpeed, TotalStrokeCount FROM Intervals" +
                        " WHERE FK_SessionId = @sessionId", cnn);
                    SQLiteParameter sessionParam = new SQLiteParameter("sessionId", System.Data.DbType.Int32);
                    sessionParam.Value = session.Id;
                    command.Parameters.Add(sessionParam);

                    SQLiteCommand dataRecordCommand = new SQLiteCommand(
                        "SELECT PK_DataRecordId, ElapsedTime, Latitude, Longitude, SpeedGps, SpeedImpeller, DistanceGps, DistanceImpeller, StrokeRate, StrokeCount, HeartRate, Calories" +
                        " FROM DataRecords WHERE FK_IntervalId = @intervalId AND Type = 0", cnn); // Type filtered to 0 to just pick up strokes

                    SQLiteParameter intervalParam = new SQLiteParameter("sessionId", System.Data.DbType.Int32);
                    command.Parameters.Add(intervalParam);

                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = (int)(long)reader[0];
                        long startTime = (long)reader[1];
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
                        }

                        session.Intervals.Add(interval);
                    }

                    reader.Close();
                }
                
                Activity activity = new Activity() { Id = session.StartTime, Sport = Sport.Other };
                activity.Creator = new Device() { Name = "SpeedCoach GPS" };

                foreach (Interval interval in session.Intervals) {
                    ActivityLap lap = new ActivityLap();
                    lap.StartTime = interval.StartTime;
                    lap.TotalTimeSeconds = interval.TotalElapsedTime;
                    lap.DistanceMeters = interval.TotalDistance;
                    lap.AverageHeartRateBpm = new HeartRateInBeatsPerMinute() { Value = (byte)interval.AverageHeartRate };
                    lap.Intensity = Intensity.Active;
                    lap.TriggerMethod = TriggerMethod.Manual;

                    // TODO: Maximum Speed, Calories, Maximum Heart Rate, Cadence/Stroke Rate (type conversion)

                    foreach (DataRecord record in interval.DataRecords)
                    {
                        Trackpoint trackpoint = new Trackpoint();
                        trackpoint.Position = new Position() { LatitudeDegrees = record.Latitude, LongitudeDegrees = record.Longitude };
                        trackpoint.Time = lap.StartTime.AddMilliseconds(record.ElapsedTime);

                        lap.Track.Add(trackpoint);
                    }

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

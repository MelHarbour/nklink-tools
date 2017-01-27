using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiNKTools.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml;
using TcxTools;
using System.Xml.Serialization;
using System.Data.SQLite;
using System.Data.Common;
using LiNKTools.Services;

namespace LiNKTools.ViewModels
{
    public class SessionListViewModel : INotifyPropertyChanged
    {
        private string filePath;
        private string outputFilePath;
        private Session selectedSession;
        private RelayCommand exportCommand;
        private ObservableCollection<Session> sessions = new ObservableCollection<Session>();

        public RelayCommand ExportCommand
        {
            get
            {
                if (exportCommand == null)
                    exportCommand = new RelayCommand(param => this.Export(), param => this.CanExport);
                return exportCommand;
            }
        }

        public IFileService FileService { get; set; }

        public string FilePath
        {
            get { return filePath; }
            set { SetField(ref filePath, value); }
        }

        public string OutputFilePath
        {
            get { return outputFilePath; }
            set { SetField(ref outputFilePath, value); }
        }

        public ObservableCollection<Session> Sessions {
            get { return sessions; }
            set { sessions = value; }
        }

        public Session SelectedSession {
            get
            {
                return selectedSession;
            }
            set
            {
                selectedSession = value;
                ExportCommand.OnCanExecuteChanged();
            }
        }

        public SessionListViewModel(IFileService fileService)
        {
            FilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LiNK for Windows\\Program Data\\nklinkdatabase.sqlite");
            FileService = fileService;
        }

        void Export()
        {
            string selectedFile = FileService.SaveFileDialog();

            if (!string.IsNullOrEmpty(selectedFile))
                OutputFilePath = selectedFile;

            if (!string.IsNullOrEmpty(OutputFilePath))
            {
                DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");
                using (SQLiteConnection cnn = (SQLiteConnection)fact.CreateConnection())
                {
                    cnn.ConnectionString = "Data Source=" + FilePath;
                    cnn.Open();

                    SQLiteCommand command = new SQLiteCommand(
                        "SELECT PK_IntervalId, StartTime, TotalElapsedTime, TotalDistance, AverageStrokeRate, AverageHeartRate, AverageSpeed, TotalStrokeCount FROM Intervals" +
                        " WHERE FK_SessionId = @sessionId", cnn);
                    SQLiteParameter sessionParam = new SQLiteParameter("sessionId", System.Data.DbType.Int32);
                    sessionParam.Value = SelectedSession.Id;
                    command.Parameters.Add(sessionParam);

                    SQLiteCommand dataRecordCommand = new SQLiteCommand(
                        "SELECT PK_DataRecordId, ElapsedTime, Latitude, Longitude, SpeedGps, SpeedImpeller, DistanceGps, DistanceImpeller, StrokeRate, HeartRate" +
                        " FROM DataRecords WHERE FK_IntervalId = @intervalId AND Type = 0", cnn); // Type filtered to 0 to just pick up strokes

                    SQLiteParameter intervalParam = new SQLiteParameter("intervalId", System.Data.DbType.Int32);
                    dataRecordCommand.Parameters.Add(intervalParam);

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
                            record.ElapsedTime = (int)(long)dataRecordReader[1];
                            record.Latitude = (double)dataRecordReader[2];
                            record.Longitude = (double)dataRecordReader[3];
                            record.SpeedGps = Convert.ToDouble((long)dataRecordReader[4]) / 100;
                            record.SpeedImpeller = Convert.ToDouble((long)dataRecordReader[5]) / 100;
                            record.DistanceGps = (int)(long)dataRecordReader[6];
                            record.DistanceImpeller = (int)(long)dataRecordReader[7];
                            record.StrokeRate = Convert.ToDouble((long)dataRecordReader[8]) / 2;
                            record.HeartRate = (int)(long)dataRecordReader[9];

                            interval.DataRecords.Add(record);
                        }

                        SelectedSession.Intervals.Add(interval);
                    }

                    reader.Close();
                }

                Activity activity = new Activity() { Id = SelectedSession.StartTime, Sport = Sport.Other };
                activity.Creator = new Device() { Name = "SpeedCoach GPS" };

                foreach (Interval interval in SelectedSession.Intervals)
                {
                    ActivityLap lap = interval.ToLap();

                    // TODO: Cadence/Stroke Rate (type conversion)
                    double maxSpeed = 0;
                    byte maxHeartRate = 0;

                    foreach (DataRecord record in interval.DataRecords)
                    {
                        Trackpoint trackpoint = record.ToTrackpoint(lap.StartTime);

                        if (trackpoint.HeartRateBpm.Value > maxHeartRate)
                            maxHeartRate = trackpoint.HeartRateBpm.Value;

                        switch (SelectedSession.SpeedInput)
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

                using (XmlWriter writer = XmlWriter.Create(OutputFilePath))
                {
                    (new XmlSerializer(typeof(TrainingCenterDatabase))).Serialize(writer, tcd);
                }
            }
        }

        bool CanExport
        {
            get
            {
                return SelectedSession != null;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

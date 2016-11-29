using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiNKTools.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LiNKTools.ViewModels
{
    public class SessionListViewModel : INotifyPropertyChanged
    {
        private string filePath;
        private ObservableCollection<Session> sessions = new ObservableCollection<Session>();

        public string FilePath
        {
            get { return filePath; }
            set { SetField(ref filePath, value); }
        }

        public ObservableCollection<Session> Sessions {
            get { return sessions; }
            set { sessions = value; }
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

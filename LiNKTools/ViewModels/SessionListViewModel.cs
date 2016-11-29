using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiNKTools.Model;

namespace LiNKTools.ViewModels
{
    public class SessionListViewModel
    {
        private ObservableCollection<Session> sessions = new ObservableCollection<Session>();

        public string FilePath { get; set; }

        public ObservableCollection<Session> Sessions {
            get { return sessions; }
            set { sessions = value; }
        }
    }
}

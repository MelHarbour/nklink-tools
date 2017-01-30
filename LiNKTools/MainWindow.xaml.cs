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
using Microsoft.Practices.Unity;

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
    }
}

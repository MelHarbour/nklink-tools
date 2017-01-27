using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiNKTools.Services
{
    public class FileService : IFileService
    {
        public string OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".sqlite";
            openFileDialog.Filter = "SQLite Databases (*.sqlite)|*.sqlite";
            openFileDialog.InitialDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LiNK for Windows\\Program Data");

            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;
            else
                return string.Empty;
        }

        public string SaveFileDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Garmin Training Center XML (*.tcx)|*.tcx";
            dialog.DefaultExt = ".tcx";
            dialog.FileName = "export.tcx";

            if (dialog.ShowDialog() == true)
                return dialog.FileName;
            else
                return string.Empty;
        }
    }
}

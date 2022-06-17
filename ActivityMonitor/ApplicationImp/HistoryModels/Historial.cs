using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitor.ApplicationImp.HistoryModels
{
    public class Historial
    {
        public string path { get; set; }
        public string query { get; set; }
        public string name { get; set; }

        public void GetDataTable()
        {
            string temp_directory = @"C:\TempHistory";
            if (!Directory.Exists(temp_directory))
            {
                DirectoryInfo di = Directory.CreateDirectory(temp_directory);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            string target = @"C:\TempHistory\History";

            if (File.Exists(target))
            {
                File.Delete(target);
            }

            File.Copy(path, target);
        }
    }
}

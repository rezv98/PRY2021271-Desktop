using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ActivityMonitor.ApplicationImp.HistoryModels
{
    public class Historial
    {
        public string path { get; set; }
        public string query { get; set; }
        public string name { get; set; }

        public DataTable GetDataTable()
        {
            using (SQLiteConnection cn = new SQLiteConnection("Data Source=" + path + ";Version=3;New=False;Compress=True;"))
            {
                try
                {
                    cn.Open();
                    SQLiteDataAdapter sd = new SQLiteDataAdapter(query, cn);
                    DataTable dt = new DataTable();
                    sd.Fill(dt);
                    cn.Close();
                    return dt;
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.Message);
                    //MessageBox.Show("An error occurred while trying to retrieve your " + name + " browser history.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return null;
        }
    }
}

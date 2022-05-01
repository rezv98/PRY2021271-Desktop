using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ActivityMonitor.ApplicationImp.HistoryModels;
using ActMon.Services;
using ActMon.Services.Interfaces;

namespace ActMon.Forms
{
    public partial class FormHistory : Form
    {
        public FormHistory()
        {
            InitializeComponent();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private async void FormHistory_Load(object sender, EventArgs e)
        {
            //Obtener de chrome
            //ChromeHistory chrome = new ChromeHistory();
            //ChromeDataGrid.DataSource = chrome.GetDataTable();

            ////Obtener de Opera
            //OperaHistory opera = new OperaHistory();
            //OperaDataGrid.DataSource = opera.GetDataTable();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitor.ApplicationImp.ScreenshotModels
{
    public class ScreenshotRequest
    {
        public string Name { get; set; }
        public string Blob { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
    }
}

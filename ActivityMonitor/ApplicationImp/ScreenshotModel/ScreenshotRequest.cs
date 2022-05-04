using System;
using System.Collections.Generic;
using System.Text;

namespace Cap_Pantalla.Model
{
    public class ScreenshotRequest
    {
        public string Name { get; set; }
        public string Blob { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
    }
}

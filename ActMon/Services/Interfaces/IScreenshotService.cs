using Cap_Pantalla.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cap_Pantalla.Services
{
    public interface IScreenshotService
    {
        Task<dynamic> SendScreenshot(ScreenshotRequest request, string token, int userId);
    }
}
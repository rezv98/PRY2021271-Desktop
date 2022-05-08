using ActivityMonitor.ApplicationImp.ScreenshotModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActMon.Services.Interfaces
{
    public interface IScreenshotService
    {
        Task<dynamic> SendScreenshot(ScreenshotRequest request, string token, int userId);
    }
}

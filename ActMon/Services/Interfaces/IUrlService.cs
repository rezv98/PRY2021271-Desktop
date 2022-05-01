using ActivityMonitor.ApplicationImp.HistoryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActMon.Services.Interfaces
{
    public interface IUrlService
    {
        Task<dynamic> SendUrl(UrlRequest request, string token, int userId);
    }
}

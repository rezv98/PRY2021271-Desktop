using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitor.ApplicationImp.HistoryModels
{
    public class OperaHistory : Historial
    {
        public OperaHistory()
        {
            path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Opera Software\Opera Stable\History";
            query = @"Select url as URL, title as Title, Time(last_visit_time/1000000 + (strftime('%s','1601-01-01')),'unixepoch','localtime')as Time, Date(last_visit_time/1000000 + (strftime('%s','1601-01-01')),'unixepoch') as Date
                        FROM urls ORDER BY last_visit_time DESC LIMIT 30";
            name = "Opera";
        }
    }
}

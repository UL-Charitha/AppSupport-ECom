using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ItineraryInfo
    {
        #region Properties

        public string flightID { get; set; }
        public string stationDep { get; set; }
        public string stationArr { get; set; }
        public DateTime dateTimeDep { get; set; }
        public DateTime dateTimeArr { get; set; } 

        #endregion
    }
}

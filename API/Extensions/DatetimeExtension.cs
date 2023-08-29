using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class DatetimeExtension
    {
        public static int CalculateAge(this DateTime obd){
            var current = DateTime.Today;
            var ages = current.Year -  obd.Year;
            if(current.Date > obd.AddYears(-ages)) ages--;
            return ages;
        }
    }
}
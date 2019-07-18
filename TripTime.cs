using System;

namespace CAB201_Flying_Postman
{
    /// <summary>
    /// Class <c>TripTime</c> is responsible for handling all aspects of time
    /// with a given trip.
    /// </summary>
    internal class TripTime
    {
        private int hour;
        private int min;

        /// <summary>
        /// Method <c>TripTime</c> is the constructor for the class TripTime
        /// </summary>
        /// <param name="HHmm">A string that represents the start time of a trip in
        /// 24hr time format</param>;
        public TripTime(string HHmm)
        {
            Convert24HrString(HHmm);
        }

        /// <summary>
        /// Method <c>Convert24HrString</c> is responsible for converting a string
        /// formatted in 24hr time an integer representation of the same time. 
        /// </summary>
        /// <param name="HHmm">A string representing the 24hr time to convert</param>
        private void Convert24HrString(string HHmm)
        {
            var DELIMITER = ':';

            var splitString = HHmm.Split(DELIMITER);

            this.hour = Convert.ToInt32(splitString[0]);
            this.min = Convert.ToInt32(splitString[1]);
        }

        /// <summary>
        /// Method <c>GetTime</c> is responsible for returning the current time value
        /// as a formatted 24hr string.
        /// </summary>
        /// <returns>A string representing the current time in 24hr format</returns>
        public string GetTime()
        {
            return $"{this.hour:00}:{this.min:00}";
        }

        /// <summary>
        /// Method <c>AddTime</c> is responsible for adding time to the current
        /// trip time.
        /// </summary>
        /// <param name="hour">An integer representing hours to be added to the
        ///     trip time</param>
        /// <param name="min">An integer representing minutes to be added to the
        ///     trip time</param>
        public void AddTime(int hour, int min)
        {
            //Calculate the hour without remainder
            var tmpHr = (int)Math.Floor((double)((this.min + min) / 60));

            //Calculate the remainder if one and add remove it from 
            //the minute value;
            if (this.min + min >= 60)
            {
                this.min = (this.min + min) - 60 * tmpHr;
            }
            else
            {
                this.min = this.min + min;

            }

            //If the current hour plus the input our is greater
            //than 24 then loop back to 0
            if (this.hour + hour + tmpHr <= 23)
            {
                this.hour += tmpHr + hour;
            }
            else
            {
                this.hour = (this.hour + tmpHr + hour) - 24;
            }
        }

        /// <summary>
        /// Method <c>AddTime</c> is responsible for adding time to the current
        /// trip time.
        /// </summary>
        /// <param name="min">An integer representing minutes to be added to the
        ///     trip time</param>
        public void AddTime(int min)
        {
            AddTime(0, min);
        }

        /// <summary>
        /// Method <c>ConvertMinutesToHourMin</c> is responsible for converting
        /// a minute value to hours/minutes.
        /// </summary>
        /// <param name="minutes">An integer representing the minutes to be converted</param>
        /// <returns>An integer array contain the hour and minutes value of the input
        ///     in that order</returns>
        public int[] ConvertMinutesToHourMin(int minutes)
        {
            var raw = minutes / 60.0;
            var hour = (int)raw;
            var min = (int)(60 * (raw - hour));
            int[] ret = { hour, min };
            return ret;
        }

        /// <summary>
        /// Method <c>ConvertHrMinToDayHrMin</c> is responsible for converting
        /// a hours and minutes into days/hours/minutes.
        /// </summary>
        /// <param name="hr">An integer representing the hour value to be converted</param>
        /// <param name="min">An integer representing the minute value to be converted</param>
        /// <returns>An array on integers containing the values for
        ///     days/hour/minutes in that order</returns>
        public int[] ConvertHrMinToDayHrMin(int hr, int min)
        {
            double day = 0;
            double hour = 0;
            if (hr >= 24)
            {
                day = hr / 24;
                Math.Floor(day);
                hour = hr - (day * 24);
            }

            int[] dayHrMin = { (int)day, (int)hour, min };
            return dayHrMin;
        }

    }
}

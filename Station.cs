using System;

namespace CAB201_Flying_Postman
{
    /// <summary>
    /// Class <c>Station</c> is responsible for holding data pertaining  to an individual
    /// station within a tour.
    /// </summary>
    internal class Station 
    {
        private readonly string stationName;
        private readonly int xPos;
        private readonly int yPos;

        /// <summary>
        /// Method <c>Station</c> is the constructor for the Station class.
        /// </summary>
        /// <param name="name">A string representing the station name.</param>
        /// <param name="xPos">An integer representing the stations X position.</param>
        /// <param name="yPos">An integer representing the stations Y position.</param>
        public Station(string name, int xPos, int yPos)
        {
            stationName = name;
            this.xPos = xPos;
            this.yPos = yPos;
        }

        /// <summary>
        /// Method <c>ReturnName</c> is responsible for returning the name of the
        /// this instance of station.
        /// </summary>
        /// <returns>A string that represents the station name.</returns>
        public string GetName()
        {
            return this.stationName;
        }

        /// <summary>
        /// Method <c>DistanceTo</c> is responsible for getting the euclidean distance
        /// between this station and another.
        /// </summary>
        /// <param name="destination">A station object that represents the station
        ///     measure to.</param>
        /// <returns>A double precision floating point value representing the
        /// distance between this station and the input station.</returns>
        public double DistanceTo(Station destination)
        {
            var xSqrt = Math.Pow((this.xPos - destination.xPos),2);
            var ySqrt = Math.Pow((this.yPos - destination.yPos),2);

            return Math.Sqrt(xSqrt + ySqrt);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CAB201_Flying_Postman
{
    /// <summary>
    /// Class <c>Trip</c> is responsible for all aspects of processing the input station list into
    /// a formatted and efficient tour itinerary.
    ///
    /// Note: it contains three options for tour efficiency, LevelTwo can be uncommented in the
    /// method <c>Tour</c> to run, otherwise it will default to LevelFour.
    /// For any trip containing 12 or less stations LevelFour will run the LevelThree algorithm.
    /// </summary>
    internal class Trip
    {
        private readonly Station[] stations;
        private readonly Plane plane;
        private readonly TripTime time;
        private readonly string outPath;

        private Station[] itineraryArray;
        private double processTime;
        private int algorithmLevel;
        private double smallest;

        /// <summary>
        /// Constructor for setting up the Trip class for use
        /// </summary>
        /// <param name="stations">An array of Station representing the stations to be sorted</param>
        /// <param name="plane">A instance of a plane class that contains the plane specifications</param>
        /// <param name="time">An instance of TripTime that has been setup for the trip</param>
        /// <param name="outPath">A string representing the output path</param>
        public Trip(Station[] stations, Plane plane, TripTime time, string outPath)
        {
            this.stations = stations;
            this.plane = plane;
            this.time = time;
            this.outPath = outPath;
        }

        /// <summary>
        /// Method <c>LevelTwoPartOne</c> is responsible for the initial setup of the itinerary .
        /// </summary>
        /// <param name="input"></param>
        private void LevelTwoPartOne(List<Station> input)
        {
            algorithmLevel = 2;
            var postOffice = stations[0];
            var tmpStations = new List<Station> {postOffice,
                postOffice};
            var inList = input;

            //Remove PO from in list
            inList.RemoveAt(0);


            for (int i = 1, k = inList.Count; k > 0; i++, k--)
            {
                //Insert first station in list and get trip distance.
                tmpStations.Insert(i, inList[0]);
                var shortestDistance = TotalDistance(tmpStations);

                //Set tmp station for holding the current shortest station
                var shortestStation = tmpStations[i];

                // Iterate through the list to find the station that returns the 
                // shortest round trip distance.
                for (var j = 1; j < inList.Count; j++)
                {
                    //Remove station at position i and replace it with next station  in the 
                    //input list then get the new distance. 
                    tmpStations.Remove(tmpStations[i]);
                    tmpStations.Insert(i, inList[j]);
                    var newDistance = TotalDistance(tmpStations);

                    //If new distance is less than old then tmp station and old shortest distance
                    //is updated.
                    if (newDistance < shortestDistance)
                    {
                        shortestStation = tmpStations[i];
                        shortestDistance = newDistance;
                    }
                }
                //Remove station at position i  from the list and replace it with the
                //shortest station.
                tmpStations.RemoveAt(i);
                tmpStations.Insert(i, shortestStation);

                //Remove the added station from the list of remaining sortedStations 
                inList.Remove(tmpStations[i]);

            }
            LevelTwoPartTwo(tmpStations);
        }

        /// <summary>
        /// Method <c>LevelTwoPartTwo</c> takes an input list of Stations and moves each sortedStations
        /// through the list by one position for an entire cycle while checking the total length
        /// of the tour at the end of each movement to determine whether the modified tour is
        /// shorter than the previously found shortest tour.
        /// </summary>
        /// <param name="input">A list of sortedStations representing the current tour</param>
        private void LevelTwoPartTwo(List<Station> input)
        {
            var tmpStations = input;
            var dist1 = TotalDistance(input);
            var shortestTrip = new Station[input.Count];

            //Iterate through every station between the first and last station in list (both post office)
            for (var i = 1; i < tmpStations.Count - 2; i++)
            {
                var counter = tmpStations.Count - 2;

                //move the station through the list until it returns to its original position
                //NOTE: Poor implementation FIX THIS!!!!!!!!
                for (int j = i, k = j + 1; counter >= 0; j++,k++, --counter)
                {
                    var tmpStation = tmpStations[j];

                    if (k > tmpStations.Count - 2)
                    {
                        tmpStations.Remove(tmpStation);
                        tmpStations.Insert(1, tmpStation);

                        j = 1;
                        k = j + 1;
                    }
                    else
                    {
                        tmpStations.Remove(tmpStation);
                        tmpStations.Insert(k, tmpStation);
                    }

                    var dist2 = TotalDistance(tmpStations);

                    if (!(dist2 < dist1)) continue;
                    shortestTrip = tmpStations.ToArray();
                    dist1 = TotalDistance(shortestTrip.ToList());
                }
            }

            // Check if there was a shorter trip provided and return base trip if not
            itineraryArray = (shortestTrip[0] != null) ? shortestTrip : input.ToArray();
        }

        /// <summary>
        /// Method <c>LevelThree</c> uses Heap's algorithm to find every possible permutation
        /// of objects in the list.
        ///
        /// Note: this method is extremely inefficient for large list, recommend not using it for
        /// any list with more than 12 objects.
        /// </summary>
        /// <param name="inList">A list of sortedStations representing the sortedStations in the tour</param>
        private void LevelThree(List<Station> inList)
        {
            algorithmLevel = 3;
            inList.RemoveAt(0);
            var tmp = inList.ToArray();
            LevelThree(tmp.Length, tmp);
        }

        private void LevelThree(int k, Station[] array)
        {
            var tmp2 = new List<Station> {stations[0], stations[0]};
            if (k == 1)
            {
                var tmp1 = array.ToList();
                tmp2.InsertRange(1, tmp1);
                if (TotalDistance(tmp2) < smallest)
                {
                    smallest = TotalDistance(tmp2);
                    itineraryArray = tmp2.ToArray();
                }
            }
            else
            {
                LevelThree(k - 1, array);
                for (var i = 1; i < k - 1; i++)
                {
                    Station tmp;
                    if (k % 2 == 0)
                    {
                        tmp = array[i];
                        array[i] = array[k - 1];
                        array[k - 1] = tmp;
                    }
                    else
                    {
                        tmp = array[0];
                        array[0] = array[k - 1];
                        array[k - 1] = tmp;
                    }
                    LevelThree(k - 1, array);
                }
            }
        }

        /// <summary>
        /// Method <c>Level Four</c> takes a list of sortedStations and uses the nearest neighbor algorithm
        ///
        /// Note: this method alone does not find the shortest round trip and is a starting point for
        /// a shortest round trip <see cref="LevelFourStepTwo"/>.
        /// Sadly this approach came up short as it barely even got to where the level 2 algorithm
        /// should have been.
        /// </summary>
        /// <param name="stations"></param>
        private void LevelFour(Station[] stations)
        {
            algorithmLevel = 4;
            if (stations.Length <= 12)
            {
                smallest = TotalDistance(stations.ToList());
                LevelThree(stations.ToList());
                return;
            }
            var trip = new List<Station>();
            var nnList = stations.ToList();

            var postOffice = stations[0];

            var current = postOffice;

            while (nnList.Count > 0)
            {
                var tmp = new Station("", 0, 0);
                var distance = (int)TotalDistance(stations.ToList());
                if (nnList.Count == 1)
                {
                    trip.Add(nnList.First());
                    nnList.Clear();
                    break;
                }

                foreach (var station in nnList)
                {
                    var newDist = (int)current.DistanceTo(station);

                    if (newDist >= distance && nnList.Count != 2) continue;
                    tmp = station;
                    distance = (int)current.DistanceTo(station);
                }

                nnList.Remove(tmp);
                trip.Add(tmp);
                current = tmp;

            }
            trip.Add(postOffice);
            LevelFourStepTwo(trip);
        }

        /// <summary>
        /// Method <c>LevelFourStepTwo</c> take a list created using the nearest neighbor algorithm
        /// and moves the last 5 elements through the list to find a neater round trip than the nn
        /// algorithm offers.
        /// </summary>
        /// <param name="input"></param>
        private void LevelFourStepTwo(List<Station> input)
        {
            input.Reverse();
            var tmpArray = input.ToArray();
            var mod = new List<Station>();

            var lowest = TotalDistance(tmpArray.ToList());
            const int stationsToReposition = 5;


            for (var i = 1; i < stationsToReposition; i++)
            {
                tmpArray = input.ToArray();
                for (var j = i; j < tmpArray.Length - 2; j++)
                {
                    if (j > tmpArray.Length - 2)
                    {
                        j = 0;
                    }
                    
                    var tmp = tmpArray[j];
                    tmpArray[j] = tmpArray[j + 1];
                    tmpArray[j + 1] = tmp;

                    var total = TotalDistance(tmpArray.ToList());

                    if (!(total <= lowest)) continue;
                    mod = tmpArray.ToList();
                    lowest = TotalDistance(mod.ToList());
                }
            }

            itineraryArray = (mod.Count>0) ? mod.ToArray() : input.ToArray();
        }

        /// <summary>
        /// Method <c>Itinerary</c> is responsible for creating a formatted itinerary from an
        /// input list of optimised sortedStations, it also calculates the times during the trip in which
        /// to refuel and gives an estimated total trip time and distance.
        /// </summary>
        /// <param name="sortedStations">A list representing the sorted station to be printed
        ///     as an itinerary</param>
        private void Itinerary(List<Station> sortedStations)
        {
            var tmStations = sortedStations.ToArray();

            var totalTripTime = time.ConvertMinutesToHourMin(DistanceToMinute(TotalDistance(sortedStations)) +
                                  (int)(DistanceToMinute(TotalDistance(sortedStations)) /
                                        (plane.Range * 60)) * plane.RefuelTime);

            //Itinerary Header
            var itineraryHeader = "";
            var tourLevel = $"Optimising tour length: Level {algorithmLevel}...\r\n";
            var processElapsed = $"Elapsed time: {processTime} seconds\r\n";
            var tourTime = "";
            if (totalTripTime[0] < 24)
            {
                tourTime = $"Tour time: {totalTripTime[0]} hours {totalTripTime[1]} minutes\r\n";
            }
            else
            {
                totalTripTime = time.ConvertHrMinToDayHrMin(totalTripTime[0], totalTripTime[1]);
                tourTime = $"Tour time: {totalTripTime[0]} days " +
                           $"{totalTripTime[1]} hours " +
                           $"{totalTripTime[2]} minutes\r\n";
            }
            var tourLength = $"Tour length: {TotalDistance(sortedStations):0.0000}\r\n";
            var refuel = $"**********Refuel {plane.RefuelTime} Minutes**********\r\n";

            itineraryHeader = tourLevel + processElapsed + tourTime + tourLength;

            Console.Write(itineraryHeader);

            //Itinerary body
            var timeSinceRefuel = 0;
            var itineraryBody = "";
            for (int i = 0, j = 1; i < tmStations.Length - 1; i++, j++)
            {
                var stationStartTime = time.GetTime();
                var tripTime = DistanceToMinute(tmStations[i].DistanceTo(tmStations[j]));
                timeSinceRefuel += tripTime;
                time.AddTime(tripTime);
                itineraryBody +=
                    $"{tmStations[i].GetName(),-7}  ->  " +
                    $"{tmStations[j].GetName(),7}\t" +
                    $"{stationStartTime,-5}  {time.GetTime(),-5}\r\n";

                if (i < tmStations.Length - 1)
                {
                    var jTemp = j + 1;
                    if (jTemp == tmStations.Length) jTemp = 0;
                    var next = DistanceToMinute(tmStations[i + 1].DistanceTo(tmStations[jTemp]));
                    if (timeSinceRefuel + next >= plane.Range * 60)
                    {
                        timeSinceRefuel = 0;
                        itineraryBody += refuel;
                        time.AddTime(plane.RefuelTime);
                    }
                }

            }
            Console.Write(itineraryBody);

            //Output to file if output argument has been passed
            if (outPath != null)
            {
                OutputTrip(itineraryHeader + itineraryBody);
            }
        }

        public void Tour()
        {
            var stopwatch = Stopwatch.StartNew();

            //LevelTwoPartOne(sortedStations.ToList());

            //smallest = TotalDistance(stations.ToList());
            //LevelThree(stations.ToList());

            LevelFour(stations);

            stopwatch.Stop();

            double tmpTime = stopwatch.ElapsedMilliseconds;
            processTime = tmpTime / 1000;
            Itinerary(itineraryArray.ToList());
        }

        /// <summary>
        /// Method <c>OutputTrip</c> is responsible for writing the itinerary to an external file
        /// </summary>
        /// <param name="itinerary">>A string representing the formatted itinerary to be written
        ///     to file. </param>
        public void OutputTrip(string itinerary)
        {
            var itineraryOutput = new StreamWriter(outPath, false);

            itineraryOutput.Write(itinerary);

            itineraryOutput.Close();
        }

        /// <summary>
        /// Method <c>TotalDistance</c> takes a list of sorted stations and iterates through them
        /// using euclidean distance to calculate the distance between a station and the next in
        /// the list.
        /// </summary>
        /// <param name="sortedStations"></param>
        /// <returns>A list of stations representing the trip to be measured</returns>
        public double TotalDistance(List<Station> sortedStations)
        {
            double tmp = 0;
            if (sortedStations[0] == null)
            {
                return 0;
            }

            for (int i = 0, j = 1; i < sortedStations.Count; i++, j++)
            {
                if (i == sortedStations.Count - 1)
                {
                    return tmp;
                }
                var dist = sortedStations[i].DistanceTo(sortedStations[j]);
                tmp += dist;
            }
            return tmp;
        }

        public int[] DistanceToTime(double distance)
        {
            var tmp = new int[2];
            //Get the time in decimal value
            var rawCalc = distance / plane.Speed;
            //Separate the whole value as hour
            var hour = (int)rawCalc;
            //Separate the decimal value
            var minRaw = rawCalc - hour;
            //Convert the decimal value into minutes and cast into int
            minRaw = ((minRaw / 100) * 60) * 100;
            var min = (int)minRaw;

            tmp[0] = hour;
            tmp[1] = min;

            return tmp;
        }

        /// <summary>
        /// Method <c>DistanceToTime</c> is responsible for converting euclidean distance
        /// to minutes;
        /// </summary>
        /// <param name="distance">A double precision float representing the distance to
        ///     be converted</param>
        /// <returns>An integer representing the time conversion of the input value</returns>
        private int DistanceToMinute(double distance)
        {
            var tmp = (distance / ((double)plane.Speed / 2)) * 60;

            return (int)tmp;
        }
    }
}

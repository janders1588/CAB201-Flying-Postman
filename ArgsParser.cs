using System;
using System.IO;
using System.Linq;

namespace CAB201_Flying_Postman
{
    /// <summary>
    /// Class <c>ArgsParser</c> checks argument input validity and parses input files.
    /// </summary>
    internal class ArgsParser
    {
        private static Station[] _stations;
        private static Plane _plane;
        private static TripTime _time;
        private static bool _parseCleared;
        private string outPath;

        /// <summary>
        /// Method <c>ParseArgs</c> handles parsing the input files and checking for any bad
        /// inputs within the files. <c>ValidateArgs</c> is run first to ensure the arguments
        /// and files bring parsed exist and are valid.
        /// 
        /// </summary>
        /// <param name="args">The command line argument array to be checked and parsed</param>
        public void ParseArgs(string[] args)
        {
            //Run args array though the ValidateArgs method to validate arguments
            try
            {
                ValidateArgs(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            var STATION = args[0];
            var PLANE = args[1];
            var TIME = args[2];

            const char DELIMITER = ' ';
            string[] fields;

            //Parse station input
            var stationCount = File.ReadLines(STATION).Count();
            _stations = new Station[stationCount];
            var stationFile = new FileStream(STATION, FileMode.Open, FileAccess.Read);
            var stationReader = new StreamReader(stationFile);
            var recordIn = stationReader.ReadLine();

            //Iterate over the mail file line by line and convert into a Station object,
            //if the line is not valid catch the exception and return to the main program.
            for (var i = 0; i < _stations.Length; i++)
            {
                if (recordIn == null) continue;
                fields = recordIn.Split(DELIMITER);
                try
                {
                    _stations[i] = new Station(fields[0],
                        Convert.ToInt32(fields[1]),
                        Convert.ToInt32(fields[2]));
                    recordIn = stationReader.ReadLine();
                }
                catch (Exception)
                {
                    Console.WriteLine("Input on line {0} in {1} is not the correct format \n" +
                              "Format need: StationName (int)X-pos (int)Y-pos", i + 1,
                                Path.GetFileName(STATION));
                    return;
                }
            }
            stationFile.Close();
            stationReader.Close();

            //Parse plane input
            var planeSpecs = new FileStream(PLANE, FileMode.Open, FileAccess.Read);
            var specsReader = new StreamReader(planeSpecs);
            recordIn = specsReader.ReadLine();

            //If line exists then convert to Plane object, if the line is not valid catch
            //the exception and return to the main program.
            if (recordIn != null)
            {
                fields = recordIn.Split(DELIMITER);

                try
                {
                    _plane = new Plane
                    {
                        Range = Convert.ToSingle(fields[0]),
                        Speed = Convert.ToInt32(fields[1]),
                        TakeoffTime = Convert.ToInt32(fields[2]),
                        LandingTime = Convert.ToInt32(fields[3]),
                        RefuelTime = Convert.ToInt32(fields[4])
                    };
                }
                catch (Exception)
                {
                    Console.WriteLine("Input in {0} is not the correct format \n" +
                              "Format needed: (int)Range (int)Speed (int)Take-Off-Time " +
                              "(int)Landing-Time (int)Refuel-Time", Path.GetFileName(PLANE));
                    return;
                }
            }

            planeSpecs.Close();
            specsReader.Close();

            //Time input Argument returns an instance of the setup time class 
            _time = new TripTime(TIME);

            //Output
            try
            {
                if (args.Length == 5)
                {
                    outPath = args[4];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            _parseCleared = true;
        }

        ///// <summary>
        ///// Method <c>ValidateArgs</c> checks the array of arguments provided to the program and
        ///// makes sure they are valid and clear to be passed into the program. It will also
        ///// check the existence of input files and output paths.
        ///// </summary>
        ///// <param name="args">The command line argument array to be checked</param>
        //private static void ValidateArgs(string[] args)
        //{
        //    const int mailArg = 0;
        //    const int planeArg = 1;
        //    const int timeArg = 2;
        //    const int outFlag = 3;
        //    const int outPathArg = 4;
        //    const int minimumArguments = 3;

        //    if (args.Length >= minimumArguments)
        //    {
        //        for (var i = 0; i < args.Length; i++)
        //        {
        //            switch (i)
        //            {
        //                // Check mail and plane input arguments
        //                case mailArg:
        //                case planeArg:
        //                {

        //                    // Check for proper file extension 
        //                    if (!(args[i].EndsWith(".txt")))
        //                    {
        //                        throw new FileLoadException($"Argument {i+1} must be a .txt file");
        //                    }

        //                    // Check that file exists
        //                    if (!File.Exists(args[i]))
        //                    {
        //                        var inputNeeded = "";
        //                        if (i == mailArg) inputNeeded = "mail";
        //                        if (i == planeArg) inputNeeded = "plane spec";

        //                        throw new FileLoadException($"The {inputNeeded} file does " +
        //                                                    "not exist");
        //                    }

        //                    break;
        //                }

        //                // Check time argument
        //                // Check for standard 24hr time by length
        //                case timeArg when args[i].Length != 5:
        //                    throw new ArgumentException("Invalid 24hr time, need HH:mm");

        //                // Check for colon (used as delimiter later) 
        //                case timeArg when args[i][2] != ':':
        //                    throw new ArgumentException("Argument 3 must be time input, " +
        //                                                "format needed HH:mm");

        //                // Check hour and minute are valid integers
        //                case timeArg:
        //                {
        //                    var splitString = args[i].Split(':');
        //                    int inputHour;
        //                    int inputMin;
        //                    try
        //                    {
        //                        inputHour = Convert.ToInt32(splitString[0]);
        //                        inputMin = Convert.ToInt32(splitString[1]);
        //                    }
        //                    catch (Exception)
        //                    {
        //                        throw new ArgumentException("Time not in valid format HH:mm");
        //                    }

        //                    // Check hour and minute are within valid 24hr time range
        //                    if (inputHour >= 24)
        //                    {
        //                        throw new ArgumentException("Hour not in range 0-23");
        //                    }

        //                    if (inputMin >= 60)
        //                    {
        //                        throw new ArgumentException("Minute not in range 0-59");
        //                    }

        //                    break;
        //                }

        //                // Check output argument
        //                case outFlag when args[i] == "-o":
        //                {
        //                    // Check if out path has been provided
        //                    if (args.Length != 5)
        //                    {
        //                        throw new ArgumentException("Please provide an output path");
        //                    }

        //                    // Check that out file is correct format
        //                    if (!args[outPathArg].EndsWith(".txt"))
        //                    {
        //                        throw new ArgumentException("Output file must be .txt");
        //                    }

        //                    // Check that out directory exists
        //                    if (!Directory.Exists(Path.GetDirectoryName(args[i + 1])))
        //                    {
        //                        var filename = Path.GetFileName(args[outPathArg]);
        //                        args[outPathArg] = Path.Combine(Directory.GetCurrentDirectory(),filename) ;
        //                        //throw new ArgumentException("Output path is not valid");

        //                    }

        //                    break;
        //                }
        //            }
        //        }

        //    }
        //    else
        //    {
        //        HelpScreen();
        //        throw new ArgumentException("");
        //    }
        //}

        private static void ValidateArgs(string[] args)
        {
            const int mailArg = 0;
            const int planeArg = 1;
            const int timeArg = 2;
            const int outFlag = 3;
            const int outPathArg = 4;
            const int minimumArguments = 3;

            if (args.Length >= minimumArguments)
            {
                for (var i = 0; i < args.Length; i++)
                {
                    // Check mail and plane input arguments
                    if (i == mailArg || i == planeArg)
                    {
                        // Check for proper file extension 
                        if (!(args[i].EndsWith(".txt")))
                        {
                            throw new FileLoadException($"Argument {i + 1} must be a .txt file");
                        }

                        // Check that file exists
                        if (!File.Exists(args[i]))
                        {
                            var inputNeeded = "";
                            if (i == mailArg) inputNeeded = "mail";
                            if (i == planeArg) inputNeeded = "plane spec";

                            throw new FileLoadException($"The {inputNeeded} file does " +
                                                        "not exist");
                        }
                    }

                    // Check time argument
                    // Check for standard 24hr time by length
                    if (i == timeArg)
                    {
                        if (args[i].Length != 5)
                            throw new ArgumentException("Invalid 24hr time, need HH:mm");

                        if (args[i][2] != ':')
                            throw new ArgumentException("Argument 3 must be time input, " +
                                                         "format needed HH:mm");

                        var splitString = args[i].Split(':');
                        int inputHour;
                        int inputMin;
                        try
                        {
                            inputHour = Convert.ToInt32(splitString[0]);
                            inputMin = Convert.ToInt32(splitString[1]);
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Time not in valid format HH:mm");
                        }

                        // Check hour and minute are within valid 24hr time range
                        if (inputHour >= 24)
                        {
                            throw new ArgumentException("Hour not in range 0-23");
                        }

                        if (inputMin >= 60)
                        {
                            throw new ArgumentException("Minute not in range 0-59");
                        }

                    }

                    // Check output argument
                    if (i == outFlag)
                    {
                        if (args[i] == "-o")
                        {
                            // Check if out path has been provided
                            if (args.Length != 5)
                            {
                                throw new ArgumentException("Please provide an output path");
                            }

                            // Check that out file is correct format
                            if (!args[outPathArg].EndsWith(".txt"))
                            {
                                throw new ArgumentException("Output file must be .txt");
                            }

                            // Check that out directory exists
                            if (!Directory.Exists(Path.GetDirectoryName(args[i + 1])))
                            {
                                var filename = Path.GetFileName(args[outPathArg]);
                                args[outPathArg] = Path.Combine(Directory.GetCurrentDirectory(), filename);
                                //throw new ArgumentException("Output path is not valid");

                            }
                        }
                    }

                }
            }
            else
            {
                HelpScreen();
                throw new ArgumentException("");
            }
            
        }


        private static void HelpScreen()
        {
            Console.WriteLine("Welcome to Assignment 1 Flying Postman");
            Console.WriteLine("To run the program please provide some additional arguments when executing");
            Console.WriteLine("The order of these arguments are specific so please follow closely");
            Console.WriteLine("******************************Mandatory*************************************");
            Console.WriteLine("Argument 1 - The path to the .txt file containing all of the station information.");
            Console.WriteLine("Argument 2 - The path to the .txt file containing the plane specifications.");
            Console.WriteLine("Argument 3 - The start time of the trip in 24hr format. e.g. 12:00");
            Console.WriteLine("******************************Optional**************************************");
            Console.WriteLine("Argument 4 - The flag '-o' to tell the program to output to a file");
            Console.WriteLine("Argument 5 - The path and filename to output to");
            Console.WriteLine("");
            Console.WriteLine($"Example (console only) - '{System.AppDomain.CurrentDomain.FriendlyName}" +
                      " mail.txt plane-spec.txt 12:00'");
            Console.WriteLine($"Example (console and output) '{System.AppDomain.CurrentDomain.FriendlyName}" +
                      " mail.txt plane-spec.txt 12:00 -o output.txt'");
        }


        /// <summary>
        /// Method <c>ParsedStations</c> returns the private array _stations.  
        /// </summary>
        /// <returns>An array of <c>Station</c> built from the mail input file.</returns>
        public Station[] ParsedStations()
        {
            return _stations;
        }

        /// <summary>
        /// Method <c>ParsedPlane</c> returns the private variable _plane.
        /// </summary>
        /// <returns>Returns the parsed plane input as a <c>Plane</c> object.</returns>
        public Plane ParsedPlane()
        {
            return _plane;
        }

        /// <summary>
        /// Method <c>ParsedTripTime</c> returns the private variable _time.
        /// </summary>
        /// <returns>An instance of TripTime that hap been set up according to the argument input</returns>
        public TripTime ParsedTripTime()
        {
            return _time;
        }

        /// <summary>
        /// Method <c>ParsedOutPath</c> returns the private variable outPath.
        /// </summary>
        /// <returns>A string representing the validated output path.</returns>
        public string ParsedOutPath()
        {
            return outPath;
        }

        /// <summary>
        /// Method <c>ParsedCleared</c> returns the private variable _parseCleared.
        /// </summary>
        /// <returns>A boolean value that represents whether or not the arguments were
        ///     successfully parsed.</returns>
        public bool ParseCleared()
        {
            return _parseCleared;
        }
    }
}

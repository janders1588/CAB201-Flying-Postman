using System;

namespace CAB201_Flying_Postman
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var parser = new ArgsParser();
            parser.ParseArgs(args);

            if (parser.ParseCleared())
            {
                var trip = new Trip(parser.ParsedStations(), parser.ParsedPlane(), parser.ParsedTripTime(),parser.ParsedOutPath());
                trip.Tour();
            }
            else
            {
                Console.WriteLine("\nPlease fix input errors and try again.");
            }

            Console.ReadLine();
        }
    }
}

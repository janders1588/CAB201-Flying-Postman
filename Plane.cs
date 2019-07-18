namespace CAB201_Flying_Postman
{
    /// <summary>
    /// Class <c>Plane</c> is responsible for storing the specifications
    /// of the plane being used for the tour.
    /// </summary>
    internal class Plane
    {
        public float Range { get; set; }
        public int Speed { get; set; }
        public int TakeoffTime { get; set; }
        public int LandingTime { get; set; }
        public int RefuelTime { get; set; }
    }
}

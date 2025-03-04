public class BookingDestineationIdViewModel
{
    public class Rootobject
    {
        public bool status { get; set; }
        public string message { get; set; }
        public long timestamp { get; set; }
        public List<Destination> data { get; set; }
    }
}

public class Destination
{
    public string dest_id { get; set; }
}
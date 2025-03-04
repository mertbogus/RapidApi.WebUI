public class BookingHotelModel
{
    public class Rootobject
    {
        public bool status { get; set; }
        public string message { get; set; }
        public long timestamp { get; set; }
        public Data data { get; set; }
    }
    public class Data
    {
        public List<Hotel> hotels { get; set; }
    }

    public class Hotel
    {
        public Property property { get; set; }
    }

    public class Property
    {
        public string name { get; set; }
        public PriceBreakdown priceBreakdown { get; set; }
        public double reviewScore { get; set; }
        public string reviewScoreWord { get; set; }
        public List<string> photoUrls { get; set; }
        public int reviewCount { get; set; }
        public string wishlistName { get; set; }
    }

    public class PriceBreakdown
    {
        public GrossPrice grossPrice { get; set; }
        public GrossPrice strikethroughPrice { get; set; }
    }

    public class GrossPrice
    {
        public string currency { get; set; }
        public double value { get; set; }
    }
}
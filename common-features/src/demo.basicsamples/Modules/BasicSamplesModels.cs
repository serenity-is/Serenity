namespace Serenity.Demo.BasicSamples;

public class OrdersByShipperRequest : ServiceRequest
{
}

public class OrdersByShipperResponse : ServiceResponse
{
    public List<Dictionary<string, object>> Values { get; set; }
    public List<string> ShipperKeys { get; set; }
    public List<string> ShipperLabels { get; set; } 
}

public class OrdersByShipperItem
{
    public string label;
    public int value;
}
using Microsoft.AspNetCore.Components;

namespace GreenPoints.C_;
[EventHandler("onmapchange", typeof(AreaEventArgs),
    enableStopPropagation: true, enablePreventDefault: true)]
[EventHandler("onlatlon", typeof(LatLonEventArgs),
    enableStopPropagation: true, enablePreventDefault: true)]
public static class EventHandlers
{
}

public class AreaEventArgs : EventArgs
{
    public double Lat1 { get; set; }
    public double Lng1 { get; set; }
    public double Lat2 { get; set; }
    public double Lng2 { get; set; }
}

public class LatLonEventArgs : EventArgs
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}
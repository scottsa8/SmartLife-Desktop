
using Microsoft.UI.Xaml;
using System;

namespace SmartLife;

public class Device
{
    public Device(string n, string ID, string i, bool onl, bool stat, string h,string sa, int bri, Visibility s, Visibility r, bool f)
    {
        brightness = bri;
        online = onl;
        state = stat;
        hue = h;
        sat = sa;
        name = n;
        icon = i;
        id = ID;
        Show_Slider = s;
        Show_Rect = r;
        favourite = f;
        
    }
    public string name { get; set; }
    public string id { get; set; }
    public string icon { get; set; }
    public bool online { get; set; }
    public bool state { get; set; }
    public string hue { get; set; }
    public string sat { get; set; }
    public int brightness { get; set; }
    public Visibility Show_Slider;
    public Visibility Show_Rect;
    public bool favourite { get; set; }
}

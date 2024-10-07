using Microsoft.UI.Xaml.Media;

namespace SmartLife;
public class Notification
{
    public string Name { get; set; }
    public string Info{ get; set; }
    public Brush Severity{ get; set; }
    public string Icon { get; set; }
    public string Time {  get; set; }

    public Notification(string n, string i, Brush s,string ic, String t)
    {
        Name = n;
        Severity = s;
        Info = i;
        Icon = ic;
        Time = t;
    }
  
}

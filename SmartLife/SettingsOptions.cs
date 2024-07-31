using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLife;

public sealed partial class SettingsOptions
{

    public string Title { get; set; }
    public string Icon { get; set; }

    public SettingsOptions(string t, string i)
    {
        Title = t;
        Icon = i;
    }

}

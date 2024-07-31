using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Syncfusion.UI.Xaml.Editors;
using Syncfusion.UI.Xaml.Sliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace SmartLife;

public partial class ApiCaller
{
    private static readonly HttpClient client = new();
    private readonly String URL = "https://px1.tuyaus.com/homeassistant";
    public ApiCaller() { }
    //getting token (logging in)
    public async Task<int> GetToken(String user, String pass)
    {
        var values = new Dictionary<string, string>
        {
              { "userName", user },
              { "password", pass },
              { "countryCode", "1" },
              { "bizType", "smart_life" },
              { "from", "tuya" }, 
        };
        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(URL + "/auth.do", content);

        string responseString = await response.Content.ReadAsStringAsync();
        //strip "" from string
        responseString = responseString.Replace("\"", "");
        //strip {} from JSON format
        responseString = responseString.Trim(new char[] { '{', '}' });
        //check if auth error
        if (!responseString.Split(",")[0].Split(":")[1].Equals("error"))
        {
            App.GetUser().setAT(responseString.Split(",")[0].Split(":")[1]);
            App.GetUser().setRT(responseString.Split(",")[1].Split(":")[1]);
            App.GetUser().StoreCreds(responseString.Split(",")[0].Split(":")[1], responseString.Split(",")[1].Split(":")[1]);
            return 10;
        }
        //print error for now
        else
        {
            System.Diagnostics.Debug.WriteLine(responseString);
            System.Diagnostics.Debug.WriteLine(responseString.Split(",")[1].Split(":")[1]);
            App.GetUser().setAT("error");
            if(responseString.Contains("auth exceed"))
            {
                return 0;
            }else if(responseString.Contains("Username or password error!"))
            {
                return -1;
            } else if(responseString.Contains("Invalid parms."))
            {
                return -1;
            }
           
            return 0;
        }
    }
    public async void Discover()
    {
        if (App.GetUser().GetAT() == "") { return; }
        //compress into json
        var headers = new Dictionary<string, string>
        {
            {"name","Discovery" },
            {"namespace","discovery" },
            {"payloadVersion","1" }
        };
        var payload = new Dictionary<string, string>
        {
            {"accessToken",App.GetUser().GetAT() }
        };
        var data = new Dictionary<string, Dictionary<string, string>>
        {
            {"header",headers },
            { "payload",payload}
        };

        var response = await client.PostAsync(URL + "/skill", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
        string responseString = await response.Content.ReadAsStringAsync();
        Root js = new Root();
        System.Diagnostics.Debug.WriteLine(responseString);
        System.Diagnostics.Debug.WriteLine(responseString.Split(":")[1].Split(",")[0]);
        if (responseString.Contains("FrequentlyInvoke") || responseString.Split(":")[1].Split(",")[0] == "{}")
        {
            App.UpdateNotification(false, "Cannot refresh devices", "You can only refresh once every 20mins!", new SolidColorBrush(Colors.PaleVioletRed), "\uE946");
            System.Diagnostics.Debug.WriteLine("trying to load off memory");
            js = App.GetUser().ReadDevices();
            if (js == null)
            {
                System.Diagnostics.Debug.WriteLine("nothing saved in memory");
                return;
            }
            System.Diagnostics.Debug.WriteLine("Using saved data");
        }
        else
        {
            js = JsonConvert.DeserializeObject<Root>(responseString)!;
            System.Diagnostics.Debug.WriteLine("Writing to memory");
            App.GetUser().StoreDevices(js);
        }
        for (int i = 0; i < js.payload.devices.Count; i++)
        {
            if (js.payload.devices[i].dev_type.Equals("light"))
            {
                Visibility Add_Brightness_Slider;
                Visibility rect;
                if (js.payload.devices[i].data.brightness == -1)
                {
                    Add_Brightness_Slider = Visibility.Collapsed;
                    rect = Visibility.Visible;
                }
                else { Add_Brightness_Slider= Visibility.Visible; rect = Visibility.Collapsed; }
                if(js.payload.devices[i].data.brightness == 1000) { js.payload.devices[i].data.brightness = 100; }
                Device ToBeAdded = new Device(
                             js.payload.devices[i].name,
                             js.payload.devices[i].id,
                             js.payload.devices[i].icon,
                             js.payload.devices[i].data.online,
                             js.payload.devices[i].data.state,
                             "100", "1",
                             js.payload.devices[i].data.brightness,
                             Add_Brightness_Slider, rect, false

                );

                if (App.GetDevices().Any(item => item.id == ToBeAdded.id))
                {
                    ToBeAdded = null!;
                    continue;
                }
                else
                {
                    App.GetDevices().Add(ToBeAdded);
                }
                ToBeAdded = null!;
            }
            else
            {
                //scenes
            }
        }
    }
    public async void TurnOnOff(object sender, bool retry)
    {
        
        if (App.GetUser().GetAT() == "") {return;}
        String value = "";
        String tag = "";
        Button b = null!;
        try{b = (Button)sender;}
        catch (Exception) { b = null!; }
        System.Diagnostics.Debug.WriteLine(b);
        if (b != null)
        {
            tag = b.Tag.ToString()!;
            if (b.Name.Equals("onB")) { value = "1"; }
            else if (b.Name.Equals("offB")) { value = "0"; }
        }else{
            int temp = Convert.ToInt32(sender);
            tag = App.GetDevices()[temp].id;
            if (App.GetDevices()[temp].state) { value = "0"; }
            else { value = "1"; }
        }
        String response = await ConfigDevice(tag, "turnOnOff", "value", value);
        if (response.Contains("header") && response.Contains("code") && response.Contains("SUCCESS")) 
        {
            var index = App.GetDevices().Select((item, index) => new { Item = item, Index = index }).First(i => i.Item.id == tag).Index;
            App.GetDevices()[index].state = !App.GetDevices()[index].state;
        }
        else
        {
            if (!retry)
            {
                Refresh();
                TurnOnOff(sender, true);
            }
        }
    }
    public async void Brightness(object sender, bool retry)
    {
        SfSlider s = (SfSlider)sender;
        String tag = s.Tag.ToString()!;
        int val = Convert.ToInt32(Math.Round(s.Value));
        if (s.Value < 11 && s.Value!=0) { val = 11; }else if(s.Value == 0) { val = 0; }
        String response = await ConfigDevice(tag, "brightnessSet", "value", val.ToString());
        if (response.Contains("header") && response.Contains("code") && response.Contains("SUCCESS"))
        {
            var index = App.GetDevices().Select((item, index) => new { Item = item, Index = index }).First(i => i.Item.id == tag).Index;
            App.GetDevices()[index].brightness = val;
            App.GetDevices()[index].state = true;
        }
        else
        {
            if (!retry)
            {
                System.Diagnostics.Debug.WriteLine("retrying");
                Refresh();
                Brightness(sender, true);
            }
        }
    }
    public async void ChangeColor(object sender, bool retry, SelectedBrushChangedEventArgs args)
    {
        var Col = ((SolidColorBrush)args.NewBrush).Color.ToHsv();
        SfDropDownColorPicker sf = (SfDropDownColorPicker)sender;
        string tag = sf.Tag.ToString()!;
        var NewColor = new Dictionary<string, string>
        {
            {"hue", Col.H.ToString() },
            {"saturation",Col.S.ToString() },
            {"brightness","100" }
        };
        String response = await ConfigDevice(tag, "colorSet", "color", JsonConvert.SerializeObject(NewColor));
        if (response.Contains("header") && response.Contains("code") && response.Contains("SUCCESS"))
        {
            var index = App.GetDevices().Select((item, index) => new { Item = item, Index = index }).First(i => i.Item.id == tag).Index;
            App.GetDevices()[index].hue=Col.H.ToString();
            App.GetDevices()[index].sat = Col.S.ToString();
            App.GetDevices()[index].state = true;
        }
        else
        {
            if (!retry)
            {
                Refresh();
                ChangeColor(sender, true,args);
            }
        }
    }
    public async void Refresh()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("refreshed token!");
            Random rnd = new Random();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(URL + "/access.do?grant_type=refresh_token&refresh_token=" + App.GetUser().GetRT() + "&rand=" + rnd.Next(0, 1))
            };
            var response = client.Send(request);
            string responseString = await response.Content.ReadAsStringAsync();
            responseString = responseString.Replace("\"", "");
            responseString = responseString.Trim(new char[] { '{', '}' });
            if (!responseString.Split(",")[0].Split(":")[1].Equals("error"))
            {
                string access_token = responseString.Split(",")[0].Split(":")[1];
                string refresh_token = responseString.Split(",")[1].Split(":")[1];
                App.GetUser().setAT(access_token);
                App.GetUser().setRT(refresh_token);
                App.GetUser().StoreCreds(access_token, refresh_token);
            }
        }
        catch (Exception) { }

    }
    public async Task<String> ConfigDevice(string id, string action, string valueName, string newState)
    {
        var headers = new Dictionary<string, string>
        {
            {"name",action },
            {"namespace","control" }, 
            {"payloadVersion","1" }
        };
        var payload = new Dictionary<string, string>
        { 
            {"accessToken",App.GetUser().GetAT() },
            {"devId",id}
        };
        var data = new Dictionary<string, Dictionary<string, string>>
        {
            {"header",headers },
            { "payload",payload}
        };
        data["payload"][valueName] = newState;
        var response = await client.PostAsync(URL + "/skill", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
        string responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }
}


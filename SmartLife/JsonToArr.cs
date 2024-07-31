// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

public class Data
{
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
    [DefaultValue(-1)]
    public int brightness { get; set; }

    public string color_mode { get; set; }
    public bool online { get; set; }
    public bool state { get; set; }
}

public class Device
{
    public Data data { get; set; }
    public string name { get; set; }
    public string icon { get; set; }
    public string id { get; set; }
    public string dev_type { get; set; }
    public string ha_type { get; set; }
}

public class Header
{
    public string code { get; set; }
    public int payloadVersion { get; set; }
}

public class Payload
{
    public List<Device> devices { get; set; }
    public List<object> scenes { get; set; }
}

public class Root
{
    public Payload payload { get; set; }
    public Header header { get; set; }
}


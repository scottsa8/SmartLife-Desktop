using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using System;

namespace SmartLife;


public partial class User
{

    private static readonly string path = Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location,@"..\creds.tmp"));
    private static readonly string devices =  Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location, @"..\devices.json"));
    private static string? Access_Token;
    private static string? Refresh_Token;
    private static bool Save = false;
    public User()
    {
        
    }
    public void login()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine(Save);
            if (Save)
            {
                StreamReader sr = new StreamReader(path);
                string? line = sr.ReadLine();
                if (line == null)
                {
                    App.setLogin(false);
                    return;
                }
                while (line != null)
                {
                    if (line.StartsWith("T:") && line.Length > 3)
                    {
                        Access_Token = line.Split(":")[1];
                    }
                    else if (line.StartsWith("R:") && line.Length > 3)
                    {
                        Refresh_Token = line.Split(":")[1];
                    }
                    else
                    {
                        App.setLogin(false);
                        return;
                    }
                    line = sr.ReadLine();
                }
                sr.Close();
                App.setLogin(true);

            }
            else { App.setLogin(false); }
        }
        catch (Exception)
        {
            App.setLogin(false);
        }
    }
    public String GetAT()
    {
        return Access_Token!;
    }
    public String GetRT()
    {
        return Refresh_Token!;
    }
    public void setAT(String a)
    {
        Access_Token = a;
    }
    public void setRT(String a)
    {
        Refresh_Token = a;
    }
    public void StoreCreds(string at, string rt)
    {
        if (Save)
        {
            System.Diagnostics.Debug.WriteLine(path);
            try
            {
                //System.Diagnostics.Debug.WriteLine("AT STORED:"+at +"   RT STORED: "+rt);
                File.Delete(path);
                StreamWriter sw = File.AppendText(path);
                sw.WriteLine("T:" + at);
                sw.WriteLine("R:" + rt);
                sw.Close();
                File.SetAttributes(path, System.IO.FileAttributes.Hidden);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }
    }
    public void StoreDevices(Root data)
    {
        try
        {

            File.Delete(devices);
            StreamWriter sw = File.AppendText(devices);
            string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
            sw.WriteLine(jsonString);
            sw.Close();
            File.SetAttributes(devices, System.IO.FileAttributes.Hidden);

        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        finally
        {
            Console.WriteLine("Executing finally block.");

        }
    }
    public Root ReadDevices()
    {
        try
        {
            StreamReader streamReader = new StreamReader(devices);
            string js = streamReader.ReadToEnd();
            streamReader.Close();
            return JsonConvert.DeserializeObject<Root>(js)!;
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            return null!;
        }            
    }
    public bool GetSaveState()
    {
        System.Diagnostics.Debug.WriteLine("returning:" + Save);
        return Save;
    }
    public void SetSaveState(bool s)
    {
        Save = s;
        App.UpdateSettings(new Windows.Graphics.SizeInt32(-1, -1));
        System.Diagnostics.Debug.WriteLine("new state:" + Save);
    }
    public void DelSave()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Deleteing:" + path) ;
            File.Delete(path);
        }
        catch (Exception e )
        {
            System.Diagnostics.Debug.WriteLine(e);
        }
    }
}

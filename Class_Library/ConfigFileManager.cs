using System.Windows;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace YankeeShower
{
    public enum RegistrationStatus
        {
            Ok,
            Expired,
            RegisterUsedChanged,
            RegistrationCodeError,
            ConfigurationFileError
        }

    public static class ConfigFileManager
    {

        public static bool GetConfigSettings()
        {           
           return ReadConfigData();        
        }

        public static bool ReadConfigData()
        {
            string result = string.Empty;
            string filepath = GetLocalExePath() + Constants.configfilename;
            bool loadedok = false;
            try
            {
                if (File.Exists(filepath))
                {
                    using (StreamReader r = new StreamReader(filepath))
                    {
                        var json = r.ReadToEnd();
                        var jobj = JObject.Parse(json);
                        Application.Current.Resources[Constants.licensee] = jobj[Constants.licensee].ToString();
                        Application.Current.Resources[Constants.eulaccepted] = jobj[Constants.eulaccepted].ToString();
                        Application.Current.Resources[Constants.LeftLogoStr] = jobj[Constants.LeftLogoStr].ToString();
                        Application.Current.Resources[Constants.RightLogoStr] = jobj[Constants.RightLogoStr].ToString();
                        Application.Current.Resources[Constants.FooterTextStr] = jobj[Constants.FooterTextStr].ToString();
                        Application.Current.Resources[Constants.FooterTextColorStr] = jobj[Constants.FooterTextColorStr].ToString();
                        List<string> l = JsonConvert.DeserializeObject<List<string>>(jobj[Constants.MRUList].ToString());
                        Application.Current.Resources[Constants.MRUList] = l;
                    }
                    loadedok = true;
                }                
                return loadedok;
            }
            catch
            {
                loadedok = false;
            }
            return loadedok;
        }

        public static string ReadJsonValue(string key)
        {
            string result = string.Empty;
            string filepath = GetLocalExePath() + Constants.configfilename;                               
            try
            {
                if (File.Exists(filepath))
                {
                    using (StreamReader r = new StreamReader(filepath))
                    {
                        var json = r.ReadToEnd();
                        var jobj = JObject.Parse(json);
                        result = jobj[key].ToString();
                    }
                }
            }
            catch { }
            return result;
        }

        public static string WriteJsonValue(string key, string value)
        {
            string result = string.Empty;
            string filepath = GetLocalExePath() + Constants.configfilename;
            string writeResult = string.Empty;
            try
            {
                if (File.Exists(filepath))
                {
                    using (StreamReader r = new StreamReader(filepath))
                    {
                        var json = r.ReadToEnd();
                        var jobj = JObject.Parse(json);
                        jobj[key] = value;
                        writeResult = jobj.ToString();
                    }                    
                    File.WriteAllText(filepath, writeResult);
                }
            }
            catch { }
            return result;
        }

        public static string WriteJsonList(string key, List<string> values)
        {
            string result = string.Empty;
            string filepath = GetLocalExePath() + Constants.configfilename;
            string writeResult = string.Empty;
            try
            {
                if (File.Exists(filepath))
                {
                    using (StreamReader r = new StreamReader(filepath))
                    {
                        var json = r.ReadToEnd();
                        var jobj = JObject.Parse(json);                                               
                        jobj[key] = JsonConvert.SerializeObject(values);                        
                        writeResult = jobj.ToString();
                    }                   
                    File.WriteAllText(filepath, writeResult);
                }
            }
            catch { }
            return result;
        }

        public static string GetLocalExePath()
        {
            try
            {
                System.Reflection.Assembly asmly = System.Reflection.Assembly.GetExecutingAssembly();
                //To get the Directory path
                return Path.GetDirectoryName(asmly.Location);
            }
            catch
            {
                return string.Empty;
            }
        }                      
    }
}

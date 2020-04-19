using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Configuration;

namespace ITSS.Repository.Relay
{
    public class ConfigurationUtils
    {
        public static string GetStringValueFromAppSettings([CallerMemberName] string key = null)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static T GetParsedValueFromAppSettings<T>([CallerMemberName] string key = null)
        {
            string strValue = ConfigurationManager.AppSettings[key];
            return ParseStringValue<T>(strValue, key);
        }

        public static T ParseStringValue<T>(string strValue, string key = null)
        {
            if (strValue == null)           
                throw new ConfigurationErrorsException($"Required parameter {key ?? ""} is not found");
            
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                return (T)converter.ConvertFromString(strValue);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException($"Parameter {key ?? ""} must be of type {typeof(T).Name}", ex);
            }
        }
    }
}

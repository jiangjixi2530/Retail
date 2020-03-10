using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Retail.Util.Extend
{
    public class JObjectToModel<T>
    {
        public static void JObjectTansToModel(JObject obj, T t)
        {
            try
            {
                var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (System.Reflection.PropertyInfo info in properties)
                {
                    if (obj.ContainsKey(info.Name))
                    {
                        if (info.PropertyType == typeof(int))
                        {
                            info.SetValue(t, obj[info.Name].ToString().ToInt());
                        }
                        if (info.PropertyType == typeof(DateTime) && obj[info.Name].ToString().Equals(string.Empty))
                        {
                            info.SetValue(t, obj[info.Name].ToString().ConvertStringToDateTime());
                        }
                        if (info.PropertyType == typeof(decimal))
                        {
                            info.SetValue(t, obj[info.Name].ToString().SafeToDecimal());
                            if (info.PropertyType == typeof(decimal))
                            {
                                info.SetValue(t, obj[info.Name].ToString().SafeToDecimal());
                            }
                            if (info.PropertyType == typeof(string))
                            {
                                info.SetValue(t, obj[info.Name].ToString());
                            }

                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}

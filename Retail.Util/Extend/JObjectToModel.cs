using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Retail.Util.Extend
{
    public static class JObjectToModel
    {
        /// <summary>
        /// JObject转成Model
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="t"></param>
        public static T JObjectTransToModel<T>(this JObject jObject)
        {
            T t = Activator.CreateInstance<T>();
            try
            {
                Dictionary<string, object> obj = new Dictionary<string, object>(jObject.ToObject<IDictionary<string, object>>(), StringComparer.CurrentCultureIgnoreCase);
                var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (PropertyInfo info in properties)
                {
                    //主键不赋值
                    if (info.Name.ToUpper().Equals("ID"))
                    {
                        continue;
                    }
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
                        }
                        if (info.PropertyType == typeof(string))
                        {
                            info.SetValue(t, obj[info.Name].ToString());
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return t;
        }

        public static T JTokenTransToModel<T>(this JToken j)
        {
            try
            {
                JObject obj = (JObject)JsonConvert.DeserializeObject(j.ObjToString());
                return obj.JObjectTransToModel<T>();
            }
            catch (Exception)
            {

            }
            return Activator.CreateInstance<T>();
        }
    }
}

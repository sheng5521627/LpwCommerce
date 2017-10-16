using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public static class DataReaderExtensions
    {
        /// <summary>
        /// 将datareader转换成对象集合
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="reader"></param>
        /// <param name="fieldsToSkip"></param>
        /// <param name="piList"></param>
        /// <returns></returns>
        public static IList<TType> DateReaderToObjectList<TType>(this IDataReader reader, string fieldsToSkip = null, Dictionary<string, PropertyInfo> piList = null)
            where TType : new()
        {
            if (reader == null)
                return null;

            var items = new List<TType>();
            //查询对象中的属性信息
            if (piList == null)
            {
                piList = new Dictionary<string, PropertyInfo>();
                var props = typeof(TType).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in props)
                {
                    piList.Add(prop.Name.ToLower(), prop);
                }
            }

            while (reader.Read())
            {
                var instance = new TType();
                DataReaderToObject(reader, instance, fieldsToSkip, piList);
                items.Add(instance);
            }
            return items;
        }

        /// <summary>
        /// 将datareader转换成对象
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="instance"></param>
        /// <param name="fieldToSkip"></param>
        /// <param name="piList"></param>
        public static void DataReaderToObject(this IDataReader reader, object instance, string fieldToSkip = null, Dictionary<string, PropertyInfo> piList = null)
        {
            if (reader.IsClosed)
                throw new InvalidOperationException("datareader已关闭");

            if (string.IsNullOrEmpty(fieldToSkip))
            {
                fieldToSkip = string.Empty;
            }
            else
            {
                fieldToSkip = "," + fieldToSkip + ",";
            }

            fieldToSkip = fieldToSkip.ToLower();

            //查询对象中的属性信息
            if (piList == null)
            {
                piList = new Dictionary<string, PropertyInfo>();
                var props = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in props)
                {
                    piList.Add(prop.Name.ToLower(), prop);
                }
            }

            for (int index = 0; index < reader.FieldCount; index++)
            {
                string name = reader.GetName(index).ToLower();
                if (piList.ContainsKey(name))
                {
                    var prop = piList[name];
                    if (fieldToSkip.Contains("," + name + ","))
                        continue;
                    if (prop != null && prop.CanWrite)
                    {
                        var value = reader.GetValue(index);
                        prop.SetValue(instance, value == DBNull.Value ? null : value, null);
                    }
                }
            }
        }
    }
}

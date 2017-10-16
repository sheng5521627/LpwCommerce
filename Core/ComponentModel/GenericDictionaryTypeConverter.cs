using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ComponentModel
{
    public class GenericDictionaryTypeConverter<K, V> : TypeConverter
    {
        protected readonly TypeConverter typeConverterKey;
        protected readonly TypeConverter typeConverterValue;

        public GenericDictionaryTypeConverter()
        {
            typeConverterKey = TypeDescriptor.GetConverter(typeof(K));
            if (typeConverterKey == null)
                throw new InvalidOperationException(string.Format("不存在该类型{0}的转换器", typeof(K).FullName));
            typeConverterValue = TypeDescriptor.GetConverter(typeof(V));
            if (typeConverterValue == null)
                throw new InvalidOperationException(string.Format("不存在该类型{0}的转换器", typeof(V).FullName));
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string input = (string) value;
                string[] items = string.IsNullOrEmpty(input)
                    ? new string[0]
                    : input.Split(new[] {';'}).Select(x => x.Trim()).ToArray();
                var result = new Dictionary<K, V>();
                Array.ForEach(items, s =>
                {
                    string[] keyValueStr = string.IsNullOrEmpty(s)
                        ? new string[0]
                        : s.Split(',').Select(x => x.Trim()).ToArray();
                    if (keyValueStr.Length == 2)
                    {
                        object dictionaryKey = (K)typeConverterKey.ConvertFromInvariantString(keyValueStr[0]);
                        object dictionaryValue = (V) typeConverterValue.ConvertFromInvariantString(keyValueStr[1]);

                        if (dictionaryKey != null && dictionaryValue != null)
                        {
                            if (!result.ContainsKey((K)dictionaryKey))
                            {
                                result.Add((K) dictionaryKey, (V) dictionaryValue);
                            }
                        }
                    }
                });

                return result;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string result = string.Empty;
                if (value != null)
                {
                    int counter = 0;
                    var dictionary = (IDictionary<K, V>) value;
                    foreach (var keyValue in dictionary)
                    {
                        result += string.Format("{0},{1}", Convert.ToString(keyValue.Key, CultureInfo.InvariantCulture),
                            Convert.ToString(keyValue.Value, CultureInfo.InvariantCulture));
                        if (counter != dictionary.Count - 1)
                        {
                            result += ";";
                        }
                        counter++;
                    }
                }
                return result;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

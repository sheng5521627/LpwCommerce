using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ComponentModel
{
    /// <summary>
    /// 
    /// </summary>
    public class GenericListTypeConverter<T> : TypeConverter
    {
        protected readonly TypeConverter typeConverter;

        public GenericListTypeConverter()
        {
            typeConverter = TypeDescriptor.GetConverter(typeof(T));
            if (typeConverter == null)
                throw new InvalidOperationException("No type converter exists for type " + typeof(T).FullName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected virtual string[] GetStringArray(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var result = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                return result;
            }
            return new string[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if(sourceType == typeof(string))
            {
                string[] items = GetStringArray(sourceType.ToString());
                return items.Any();
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if(value is string)
            {
                string[] items = GetStringArray((string)value);
                var result = new List<T>();
                Array.ForEach(items, item => 
                {
                    object o = typeConverter.ConvertFromInvariantString(item);
                    if (o != null)
                    {
                        result.Add((T)o);
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
                    for(int i = 0; i < ((IList<T>)value).Count; i++)
                    {
                        var str = Convert.ToString(((IList<T>)value)[i], CultureInfo.InvariantCulture);
                        result += str;
                        if (i != ((IList<T>)value).Count - 1)
                        {
                            result += ",";
                        }
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

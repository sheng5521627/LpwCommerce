using Core.ComponentModel;
using Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Core
{
    public partial class CommonHelper
    {
        /// <summary>
        /// 用户邮箱验证
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string EnsureSubscriberEmailOrThrow(string email)
        {
            string output = EnsureNotNull(email);
            output = output.Trim();
            output = EnsureMaximumLength(output, 255);

            if (IsValidEmail(output))
                throw new NopException("email is not valid.");

            return output;
        }

        /// <summary>
        /// 确保字符串不为null
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EnsureNotNull(string str)
        {
            if (str == null)
                return string.Empty;
            return str;
        }

        /// <summary>
        /// 判断是否是邮箱格式
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            email = email.Trim();
            var result = Regex.IsMatch(email, "^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$", RegexOptions.IgnoreCase);
            return result;
        }

        /// <summary>
        /// 截取字符串的最大长度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLength"></param>
        /// <param name="postfix"></param>
        /// <returns></returns>
        public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (str.Length > maxLength)
            {
                var result = str.Substring(0, maxLength);
                if (!string.IsNullOrEmpty(postfix))
                {
                    result += postfix;
                }
                return result;
            }

            return str;
        }

        /// <summary>
        /// 生成随机数字码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            string str = string.Empty;
            for (int i = 0; i < length; i++)
            {
                str = string.Concat(str, random.Next(10).ToString());
            }
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        /// <summary>
        /// 保证字符串只包含数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EnsureNumericOnly(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            var result = new StringBuilder();
            foreach (char c in str)
            {
                if (char.IsDigit(c))
                    result.Append(c);
            }
            return result.ToString();
        }

        /// <summary>
        /// 确保给定的字符串为null或empty
        /// </summary>
        /// <param name="stringsToVolidate"></param>
        /// <returns></returns>
        public static bool AreNullOrEmpty(params string[] stringsToVolidate)
        {
            bool result = false;
            Array.ForEach(stringsToVolidate, str =>
            {
                if (string.IsNullOrEmpty(str))
                    result = true;
            });
            return result;
        }

        /// <summary>
        /// 判断两个数组是否相等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;
            if (a1 == null || a2 == null)
                return false;
            if (a1.Length != a2.Length)
                return false;
            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i]))
                    return false;
            }
            return true;
        }

        private static AspNetHostingPermissionLevel? _trustLevel;
        /// <summary>
        /// 获取应用程序的信任级别
        /// </summary>
        /// <returns></returns>
        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!_trustLevel.HasValue)
            {
                _trustLevel = AspNetHostingPermissionLevel.None;

                foreach (AspNetHostingPermissionLevel trustLevel in new[] {
                                AspNetHostingPermissionLevel.Unrestricted,
                                AspNetHostingPermissionLevel.High,
                                AspNetHostingPermissionLevel.Medium,
                                AspNetHostingPermissionLevel.Low,
                                AspNetHostingPermissionLevel.Minimal
                            })
                {
                    try
                    {
                        new AspNetHostingPermission(trustLevel).Demand();
                        _trustLevel = trustLevel;
                        break; //we've set the highest permission we can
                    }
                    catch (System.Security.SecurityException)
                    {
                        continue;
                    }
                }
            }
            return _trustLevel.Value;
        }

        /// <summary>
        /// 设置实例的属性值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            if (instance == null)
                throw new ArgumentException("instance");
            if (propertyName == null)
                throw new ArgumentException("propertyName");

            Type instanceType = instance.GetType();
            PropertyInfo propertyInfo = instanceType.GetProperty(propertyName);
            if (propertyInfo == null)
                throw new NopException(string.Format("该类型{0}的实例不存在对应的属性{1}", instanceType, propertyName));
            if (!propertyInfo.CanWrite)
                throw new NopException(string.Format("该类型{0}的实例属性{1}没有set访问器", instanceType, propertyName));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T To<T>(object value)
        {
            return (T)To(value, typeof(T));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public static object To(object value,Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 将值转换成给定的类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static object To(object value, Type destinationType, CultureInfo cultureInfo)
        {
            if (value != null)
            {
                var sourceType = value.GetType();
                TypeConverter destinationTypeConverter = GetNopCustomTypeConverter(destinationType);
                TypeConverter sourceTypeConverter = GetNopCustomTypeConverter(sourceType);
                if (destinationType != null && destinationTypeConverter.CanConvertFrom(sourceType))
                    return destinationTypeConverter.ConvertFrom(null, cultureInfo, value);
                if (sourceTypeConverter != null && sourceTypeConverter.CanConvertTo(null, destinationType))
                    return sourceTypeConverter.ConvertTo(null, cultureInfo, value, destinationType);
                if (destinationType.IsEnum && value is int)
                {
                    return Enum.ToObject(destinationType, (int)value);
                }
                if (!destinationType.IsInstanceOfType(sourceType))
                    return Convert.ChangeType(value, destinationType, cultureInfo);
            }
            return value;
        }

        /// <summary>
        /// 获取自定义的转换器
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TypeConverter GetNopCustomTypeConverter(Type type)
        {
            if (type == typeof(List<int>))
                return new GenericListTypeConverter<int>();
            if (type == typeof(List<decimal>))
                return new GenericListTypeConverter<decimal>();
            if (type == typeof(List<string>))
                return new GenericListTypeConverter<string>();
            if (type == typeof(ShippingOption))
                return new ShippingOptionTypeConverter();
            if (type == typeof(List<ShippingOption>) || type == typeof(IList<ShippingOption>))
                return new ShippingOptionListTypeConverter();

            return TypeDescriptor.GetConverter(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertEnum(string str)
        {
            string result = string.Empty;
            char[] letters = result.ToCharArray();

            foreach (char c in letters)
            {
                if (c.ToString() != c.ToString().ToLower())
                    result += " " + c.ToString();
                else
                    result += c.ToString();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SetTelerikCulture()
        {
            var culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static int GetDifferenceInYears(DateTime startDate, DateTime endDate)
        {
            int age = endDate.Year - startDate.Year;
            if (startDate > endDate.AddYears(-age))
                age--;
            return age;
        }

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public static string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                //hosted
                return HostingEnvironment.MapPath(path);
            }

            //not hosted. For example, run in unit tests
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }
    }
}

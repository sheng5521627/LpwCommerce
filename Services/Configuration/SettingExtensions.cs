using Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Services.Configuration
{
    public static class SettingExtensions
    {
        /// <summary>
        /// 获取配置信息的key值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPropType"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static string GetSettingKey<T, TPropType>(this T entity, Expression<Func<T, TPropType>> keySelector)
            where T : ISettings, new()
        {
            var member = keySelector.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format("Expression {0} refers to a method,not a property", keySelector));

            var propInfo = member.Member as PropertyInfo;
            if(propInfo ==null)
                throw new ArgumentException(string.Format("Expression {0} refers to a method,not a property", keySelector));

            var key = typeof(T).Name + "." + propInfo.Name;
            return key;
        }
    }
}

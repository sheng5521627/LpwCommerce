using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Authentication.External
{
    /// <summary>
    /// Claims translator(翻译)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial interface IClaimsTranslator<T>
    {
        UserClaims Translate(T response);
    }
}

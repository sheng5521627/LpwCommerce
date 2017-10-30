using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Core.Infrastructure;

namespace Web.Framework
{
    public class NopValidatorFactory : AttributedValidatorFactory
    {
        //private readonly InstanceCache _cache = new InstanceCache();

        public override IValidator GetValidator(Type type)
        {
            if (type != null)
            {
                var attibute = (ValidatorAttribute)Attribute.GetCustomAttribute(type, typeof(ValidatorAttribute));
                if(attibute !=null && attibute.ValidatorType != null)
                {
                    var instance = EngineContext.Current.ContainerManager.ResolveUnregistered(attibute.ValidatorType);
                    return instance as IValidator;
                }
            }
            return null;
        }
    }
}

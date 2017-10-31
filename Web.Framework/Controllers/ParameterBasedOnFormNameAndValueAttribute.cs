﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Web.Framework.Controllers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ParameterBasedOnFormNameAndValueAttribute : FilterAttribute, IActionFilter
    {
        private readonly string _name;
        private readonly string _value;
        private readonly string _actionParameterName;

        public ParameterBasedOnFormNameAndValueAttribute(string name, string value, string actionParameterName)
        {
            this._name = name;
            this._value = value;
            this._actionParameterName = actionParameterName;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var formValue = filterContext.RequestContext.HttpContext.Request.Form[_name];
            filterContext.ActionParameters[_actionParameterName] = !string.IsNullOrEmpty(formValue) &&
                                                                   formValue.ToLower().Equals(_value.ToLower());
        }
    }
}

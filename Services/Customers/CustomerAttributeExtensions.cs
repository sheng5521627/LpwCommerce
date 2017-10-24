using Core.Domain.Catalog;
using Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Customers
{
    public static class CustomerAttributeExtensions
    {
        public static bool ShouldHaveValues(this CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                return false;

            if (customerAttribute.AttributeControlType == AttributeControlType.TextBox ||
               customerAttribute.AttributeControlType == AttributeControlType.MultilineTextbox ||
               customerAttribute.AttributeControlType == AttributeControlType.Datepicker ||
               customerAttribute.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            return true;
        }
    }
}

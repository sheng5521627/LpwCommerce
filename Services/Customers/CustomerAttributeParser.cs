using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Customers;
using Services.Localization;
using System.Xml;
using System.Diagnostics;

namespace Services.Customers
{
    public partial class CustomerAttributeParser : ICustomerAttributeParser
    {
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ILocalizationService _localizationService;

        public CustomerAttributeParser(ICustomerAttributeService customerAttributeService,
            ILocalizationService localizationService)
        {
            this._customerAttributeService = customerAttributeService;
            this._localizationService = localizationService;
        }

        protected virtual IList<int> ParseCustomerAttributeIds(string attibutesXml)
        {
            var ids = new List<int>();
            if (string.IsNullOrEmpty(attibutesXml))
                return ids;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attibutesXml);
                foreach (XmlNode node in xmlDoc.SelectNodes(@"//Attributes/CustomerAttribute"))
                {
                    if (node != null && node.Attributes["ID"] != null)
                    {
                        string str = node.Attributes["ID"].InnerText.Trim();
                        int id;
                        if (int.TryParse(str, out id))
                        {
                            ids.Add(id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return ids;
        }


        public string AddCustomerAttribute(string attributesXml, CustomerAttribute ca, string value)
        {
            string result = string.Empty;
            try
            {
                var xmlDoc = new XmlDocument();
                if (string.IsNullOrEmpty(attributesXml))
                {
                    var element1 = xmlDoc.CreateElement("Attributes");
                    xmlDoc.AppendChild(element1);
                }
                else
                {
                    xmlDoc.LoadXml(attributesXml);
                }
                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");
                XmlElement attributeElemet = null;
                var nodeList1 = xmlDoc.SelectSingleNode(@"//Attributes/CustomerAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1 != null && node1.Attributes["ID"] != null)
                    {
                        string str = node1.Attributes["ID"].InnerText.Trim();
                        int id;
                        if (int.TryParse(str, out id))
                        {
                            if (id == ca.Id)
                            {
                                attributeElemet = (XmlElement)node1;
                                break;
                            }
                        }
                    }
                }
                if (attributeElemet == null)
                {
                    attributeElemet = xmlDoc.CreateElement("CustomerAttribute");
                    attributeElemet.SetAttribute("ID", ca.Id.ToString());
                    rootElement.AppendChild(attributeElemet);
                }

                var attributeValueElement = xmlDoc.CreateElement("CustomerAttributeValue");
                attributeValueElement.InnerText = value;
                attributeElemet.AppendChild(attributeValueElement);

                result = xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
            }

            return result;
        }

        public IList<string> GetAttributeWarnings(string attributesXml)
        {
            var warnings = new List<string>();
            var attributes1 = ParseCustomerAttributes(attributesXml);
            var attributes2 = _customerAttributeService.GetAllCustomerAttributes();
            foreach(var a2 in attributes2)
            {
                if (a2.IsRequired)
                {
                    bool found = false;
                    foreach(var a1 in attributes1)
                    {
                        if(a1.Id == a2.Id)
                        {
                            var valueStrs = ParseValues(attributesXml, a1.Id);
                            foreach(string valueStr in valueStrs)
                            {
                                if (!string.IsNullOrEmpty(valueStr))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!found)
                    {
                        var notFoundWarning = string.Format(_localizationService.GetResource("ShoppingCart.SelectAttribute"), a2.GetLocalized(a => a.Name));
                        warnings.Add(notFoundWarning);
                    }
                }
            }
            return warnings;
        }

        public IList<CustomerAttribute> ParseCustomerAttributes(string attributesXml)
        {
            var result = new List<CustomerAttribute>();
            var ids = ParseCustomerAttributeIds(attributesXml);
            foreach(var id in ids)
            {
                var attribute = _customerAttributeService.GetCustomerAttributeById(id);
                if(attribute != null)
                {
                    result.Add(attribute);
                }
            }
            return result;
        }

        public IList<CustomerAttributeValue> ParseCustomerAttributeValues(string attributesXml)
        {
            var values = new List<CustomerAttributeValue>();
            var attributes = ParseCustomerAttributes(attributesXml);
            foreach(var attribute in attributes)
            {
                if (!attribute.ShouldHaveValues())
                    continue;

                var valueStrs = ParseValues(attributesXml, attribute.Id);
                foreach(string valueStr in valueStrs)
                {
                    if (!string.IsNullOrEmpty(valueStr))
                    {
                        int id;
                        if(int.TryParse(valueStr,out id))
                        {
                            var value = _customerAttributeService.GetCustomerAttributeValueById(id);
                            if (value != null)
                                values.Add(value);
                        }
                    }
                }
            }
            return values;
        }

        public IList<string> ParseValues(string attributesXml, int customerAttributeId)
        {
            var selectedCustomerAttributeValues = new List<string>();
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);
                foreach (XmlNode node in xmlDoc.SelectNodes(@"//Attributes/CustomerAttribute"))
                {
                    if(node !=null && node.Attributes["ID"] != null)
                    {
                        string str = node.Attributes["ID"].InnerText.Trim();
                        int id;
                        if(int.TryParse(str,out id))
                        {
                            if(id == customerAttributeId)
                            {
                                var nodeList = node.SelectNodes(@"CustomerAttributeValue/Value");
                                foreach(XmlNode node1 in nodeList)
                                {
                                    string value = node1.InnerText.Trim();
                                    selectedCustomerAttributeValues.Add(value);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return selectedCustomerAttributeValues;
        }
    }
}

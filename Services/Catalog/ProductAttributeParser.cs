using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Catalog;
using System.Xml;
using System.Diagnostics;

namespace Services.Catalog
{
    public partial class ProductAttributeParser : IProductAttributeParser
    {
        #region Fields

        private readonly IProductAttributeService _productAttributeService;

        #endregion

        #region Ctor

        public ProductAttributeParser(IProductAttributeService productAttributeService)
        {
            this._productAttributeService = productAttributeService;
        }

        #endregion

        /// <summary>
        /// Gets selected product attribute mapping identifiers
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Selected product attribute mapping identifiers</returns>
        protected virtual IList<int> ParseProductAttributeMappingIds(string attributesXml)
        {
            var ids = new List<int>();
            if (String.IsNullOrEmpty(attributesXml))
                return ids;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);

                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/ProductAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["ID"] != null)
                    {
                        string str1 = node1.Attributes["ID"].InnerText.Trim();
                        int id;
                        if (int.TryParse(str1, out id))
                        {
                            ids.Add(id);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
            return ids;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributesXml"></param>
        /// <param name="recipientName">收件人</param>
        /// <param name="recipientEmail"></param>
        /// <param name="senderName"></param>
        /// <param name="senderEmail"></param>
        /// <param name="giftCardMessage"></param>
        /// <returns></returns>
        public string AddGiftCardAttribute(string attributesXml, string recipientName, string recipientEmail, string senderName, string senderEmail, string giftCardMessage)
        {
            string result = string.Empty;
            try
            {
                recipientName = recipientName.Trim();
                recipientEmail = recipientEmail.Trim();
                senderName = senderName.Trim();
                senderEmail = senderEmail.Trim();

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
                var giftCardElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo");
                if (giftCardElement == null)
                {
                    giftCardElement = xmlDoc.CreateElement("GiftCardInfo");
                    rootElement.AppendChild(giftCardElement);
                }

                var recipientNameElement = xmlDoc.CreateElement("RecipientName");
                recipientNameElement.InnerText = recipientName;
                giftCardElement.AppendChild(recipientNameElement);

                var recipientEmailElement = xmlDoc.CreateElement("RecipientEmail");
                recipientEmailElement.InnerText = recipientEmail;
                giftCardElement.AppendChild(recipientEmailElement);

                var senderNameElement = xmlDoc.CreateElement("SenderName");
                senderNameElement.InnerText = senderName;
                giftCardElement.AppendChild(senderNameElement);

                var senderEmailElement = xmlDoc.CreateElement("SenderEmail");
                senderEmailElement.InnerText = senderEmail;
                giftCardElement.AppendChild(senderEmailElement);

                var messageElement = xmlDoc.CreateElement("Message");
                messageElement.InnerText = giftCardMessage;
                giftCardElement.AppendChild(messageElement);

                result = xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
            return result;
        }

        public string AddProductAttribute(string attributesXml, ProductAttributeMapping productAttributeMapping, string value)
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

                var rootElement = xmlDoc.SelectSingleNode(@"//Attributes");
                XmlElement attributeElement = null;
                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/ProductAttribute");
                //find existing
                foreach (XmlNode node in nodeList1)
                {
                    if (node != null && node.Attributes["ID"] != null)
                    {
                        string str1 = node.Attributes["ID"].InnerText.Trim();
                        int id;
                        if(int.TryParse(str1,out id))
                        {
                            if(id == productAttributeMapping.Id)
                            {
                                attributeElement = (XmlElement)node;
                                break;
                            }
                        }
                    }
                }
                if(attributeElement == null)
                {
                    attributeElement = xmlDoc.CreateElement("ProductAttribute");
                    attributeElement.SetAttribute("ID", productAttributeMapping.Id.ToString());
                    rootElement.AppendChild(attributeElement);
                }

                var attributeValueElement = xmlDoc.CreateElement("ProductAttributeValue");
                attributeElement.AppendChild(attributeValueElement);

                var attributeValueValueElement = xmlDoc.CreateElement("Value");
                attributeValueValueElement.InnerText = value;
                attributeValueElement.AppendChild(attributeValueValueElement);

                result = xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
            return result;
        }

        public bool AreProductAttributesEqual(string attributesXml1, string attributesXml2, bool ignoreNonCombinableAttributes)
        {
            var attributes1 = ParseProductAttributeMappings(attributesXml1);
            var attributes2 = ParseProductAttributeMappings(attributesXml2);
            if (ignoreNonCombinableAttributes)
            {
                attributes1 = attributes1.Where(m => !m.IsNonCombinable()).ToList();
                attributes2 = attributes2.Where(m => !m.IsNonCombinable()).ToList();
            }

            if (attributes1.Count != attributes2.Count)
                return false;

            bool attributesEqual = true;

            foreach(var attribute1 in attributes1)
            {
                bool hasAttribute = false;
                foreach(var attribute2 in attributes2)
                {
                    if(attribute1.Id == attribute2.Id)
                    {
                        hasAttribute = true;
                        var values1Str = ParseValues(attributesXml1, attribute1.Id);
                        var values2Str = ParseValues(attributesXml2, attribute2.Id);
                        if(values1Str.Count == values2Str.Count)
                        {
                            foreach(string str1 in values1Str)
                            {
                                bool hasValue = false;
                                foreach(string str2 in values2Str)
                                {
                                    if (str1.Trim() == str2.Trim())
                                    {
                                        hasValue = true;
                                        break;
                                    }
                                }
                                if (hasValue == false)
                                {
                                    attributesEqual = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            attributesEqual = false;
                            break;
                        }
                    }
                }
                if(hasAttribute == false)
                {
                    attributesEqual = false;
                    break;
                }
            }

            return attributesEqual;
        }

        public ProductAttributeCombination FindProductAttributeCombination(Product product, string attributesXml, bool ignoreNonCombinableAttributes = true)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var combinations = _productAttributeService.GetAllProductAttributeCombinations(product.Id);
            return combinations.FirstOrDefault(x =>
                AreProductAttributesEqual(x.AttributesXml, attributesXml, ignoreNonCombinableAttributes));
        }

        public IList<string> GenerateAllCombinations(Product product, bool ignoreNonCombinableAttributes = false)
        {
            throw new NotImplementedException();
        }

        public void GetGiftCardAttribute(string attributesXml, out string recipientName, out string recipientEmail, out string senderName, out string senderEmail, out string giftCardMessage)
        {
            throw new NotImplementedException();
        }

        public bool? IsConditionMet(ProductAttributeMapping pam, string selectedAttributesXml)
        {
            throw new NotImplementedException();
        }

        public IList<ProductAttributeMapping> ParseProductAttributeMappings(string attributesXml)
        {
            var result = new List<ProductAttributeMapping>();
            var ids = ParseProductAttributeMappingIds(attributesXml);
            foreach(var id in ids)
            {
                var attribute = _productAttributeService.GetProductAttributeMappingById(id);
                if(attribute != null)
                {
                    result.Add(attribute);
                }
            }
            return result;
        }

        public IList<ProductAttributeValue> ParseProductAttributeValues(string attributesXml)
        {
            var values = new List<ProductAttributeValue>();
            var attributes = ParseProductAttributeMappings(attributesXml);
            foreach(var attibute in attributes)
            {
                if (attibute.ShouldHaveValues())
                    continue;

                var valuesStr = ParseValues(attributesXml, attibute.Id);
                foreach(var valueStr in valuesStr)
                {
                    if (string.IsNullOrEmpty(valueStr))
                    {
                        int id;
                        if(int.TryParse(valueStr, out id))
                        {
                            var value = _productAttributeService.GetProductAttributeValueById(id);
                            if (value != null)
                                values.Add(value);
                        }
                    }
                }
            }
            return values;
        }

        public IList<string> ParseValues(string attributesXml, int productAttributeMappingId)
        {
            var selectedValues = new List<string>();
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);
                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/ProductAttribute");
                foreach(XmlNode node1 in nodeList1)
                {
                    if (node1 != null && node1.Attributes["ID"] != null)
                    {
                        string str1 = node1.Attributes["ID"].InnerText.Trim();
                        int id;
                        if(int.TryParse(str1,out id))
                        {
                            if(id == productAttributeMappingId)
                            {
                                var nodeList2 = node1.SelectNodes(@"ProductAttributeValue/Value");
                                foreach(XmlNode node2 in nodeList2)
                                {
                                    string value = node2.InnerText.Trim();
                                    selectedValues.Add(value);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
            return selectedValues;
        }

        public string RemoveProductAttribute(string attributesXml, ProductAttributeMapping productAttributeMapping)
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

                XmlElement attributeElement = null;
                //find existing
                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/ProductAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["ID"] != null)
                    {
                        string str1 = node1.Attributes["ID"].InnerText.Trim();
                        int id;
                        if (int.TryParse(str1, out id))
                        {
                            if (id == productAttributeMapping.Id)
                            {
                                attributeElement = (XmlElement)node1;
                                break;
                            }
                        }
                    }
                }

                //found
                if (attributeElement != null)
                {
                    rootElement.RemoveChild(attributeElement);
                }

                result = xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
            return result;
        }
    }
}

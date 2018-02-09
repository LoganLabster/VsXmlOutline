using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace XmlOutline.CustomScripts
{
    public class NameGeneration : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var node = XElement.Parse(((XmlElement)value).OuterXml);
            
            switch (node.Name.LocalName)
            {
                case "Asset":
                    return ": " + Utilities.GetAttr(node, "AssetPath");
                case "GUIPanel":
                    return ": " + Utilities.GetAttr(node, "AssetPath");
                case "GUIDialogue":
                    return ": " + Utilities.GetAttr(node, "ExternalXmlPath");
                case "GUIScreen":
                    return ": " + Utilities.GetAttr(node, "ExternalXmlPath");
                case "GUIPopup":
                    return ": " + Utilities.GetAttr(node, "ExternalXmlPath");
                case "Label":
                    return ": " + Utilities.GetAttr(node, "Align");
                case "Screen":
                    return ": " + Utilities.GetAttr(node, "GUIScreenId");
                case "Option":
                    return ": " +Utilities.GetAttr(node, "Sentence");
                case "MoteTo":
                    return ": " + Utilities.GetAttr(node, "Element");
                case "StartConversation":
                    return ": " + Utilities.GetAttr(node, "ConversationTargetId");
                case "Scene":
                    return ": " + Utilities.GetAttr(node, "Title");
                case "Debug":
                    return ": " + Utilities.GetAttr(node, "Message");
                case "GotoState":
                    return ": " + Utilities.GetAttr(node, "StateId");

                default:
                    if (node.Attribute("Id")?.Value != null)
                    {
                        return ": " + Utilities.GetAttr(node, "Id");
                    }
                    
                    return "";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

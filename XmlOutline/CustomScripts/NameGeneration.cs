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

//            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
//            stackPanel.SetValue(VirtualizingStackPanel.IsVirtualizingProperty, true);
//            stackPanel.SetValue(VirtualizingStackPanel.VirtualizationModeProperty, VirtualizationMode.Standard);

//            var texts = new[] { new TextBlock(), new TextBlock() };
//            texts[0].Foreground = new SolidColorBrush(Colors.LightSkyBlue);
//            texts[1].Foreground = new SolidColorBrush(Colors.YellowGreen);
//
//            texts[0].FontSize = 14;
//            texts[0].FontWeight = FontWeights.Medium;
//
//            texts[1].FontSize = 13;
//            texts[1].FontStyle = FontStyles.Italic;
//
//            stackPanel.Children.Add(texts[0]);
//            stackPanel.Children.Add(texts[1]);
            
            switch (node.Name.LocalName)
            {
                case "Asset":
                    return ": " + Utilities.GetAttr(node, "AssetPath");
//                    texts[0].Text = "Asset: ";
//                    texts[1].Text = Utilities.GetAttr(node, "AssetPath");
//                    return stackPanel;
                case "GUIPanel":
                    return ": " + Utilities.GetAttr(node, "AssetPath");
//                    texts[0].Text = "GUI Panel: ";
//                    texts[1].Text = Utilities.GetAttr(node, "ExternalXmlPath");
//                    return stackPanel;
                case "GUIDialogue":
                    return ": " + Utilities.GetAttr(node, "ExternalXmlPath");
//                    texts[0].Text = "GUI Dialogue: ";
//                    texts[1].Text = Utilities.GetAttr(node, "ExternalXmlPath");
//                    return stackPanel;
                case "GUIScreen":
                    return ": " + Utilities.GetAttr(node, "ExternalXmlPath");
//                    texts[0].Text = "GUI Screen: ";
//                    texts[1].Text = Utilities.GetAttr(node, "ExternalXmlPath");
//                    return stackPanel;
                case "GUIPopup":
                    return ": " + Utilities.GetAttr(node, "ExternalXmlPath");
//                    texts[0].Text = "GUI Popup: ";
//                    texts[1].Text = Utilities.GetAttr(node, "ExternalXmlPath");
//                    return stackPanel;
                case "Label":
                    return ": " + Utilities.GetAttr(node, "Align");
//                    texts[0].Text = "Label: ";
//                    texts[1].Text = Utilities.GetAttr(node, "Align");
//                    return stackPanel;
                case "Screen":
                    return ": " + Utilities.GetAttr(node, "GUIScreenId");
//                    texts[0].Text = "Screen: ";
//                    texts[1].Text = Utilities.GetAttr(node, "GUIScreenId");
//                    return stackPanel;
                case "Option":
                    return ": " +Utilities.GetAttr(node, "Sentence");
//                    texts[0].Text = "Option: ";
//                    texts[1].Text = Utilities.GetAttr(node, "Sentence");
//                    return stackPanel;
                case "MoteTo":
                    return ": " + Utilities.GetAttr(node, "Element");
//                    texts[0].Text = "Move to: ";
//                    texts[1].Text = Utilities.GetAttr(node, "Element");
//                    return stackPanel;
                case "StartConversation":
                    return ": " + Utilities.GetAttr(node, "ConversationTargetId");
//                    texts[0].Text = "StartConversation: ";
//                    texts[1].Text = Utilities.GetAttr(node, "ConversationTargetId");
//                    return stackPanel;
                case "Scene":
                    return ": " + Utilities.GetAttr(node, "Title");
//                    texts[0].Text = "Scene: ";
//                    texts[1].Text = Utilities.GetAttr(node, "Title");
//                    return stackPanel;
                case "Debug":
                    return ": " + Utilities.GetAttr(node, "Message");
//                    texts[0].Text = "Debug: ";
//                    texts[1].Text = Utilities.GetAttr(node, "Message");
//                    return stackPanel;
                case "GotoState":
                    return ": " + Utilities.GetAttr(node, "StateId");
//                    texts[0].Text = "Goto State: ";
//                    texts[1].Text = Utilities.GetAttr(node, "StateId");
//                    return stackPanel;

                default:
//                    texts[0].Text = node.Name.LocalName;
                    if (node.Attribute("Id")?.Value != null)
                    {
//                        texts[0].Text += ": ";
//                        texts[1].Text = Utilities.GetAttr(node, "Id");
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

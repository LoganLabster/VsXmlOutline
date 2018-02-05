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


            var node = XElement.Parse(((XmlElement)value).OuterXml); //(XElement) value;
//            v.NodeType == XmlNodeType.Element

            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            stackPanel.SetValue(VirtualizingStackPanel.IsVirtualizingProperty, true);
            stackPanel.SetValue(VirtualizingStackPanel.VirtualizationModeProperty, VirtualizationMode.Standard);

            var texts = new[] { new TextBlock(), new TextBlock() };
            texts[0].Foreground = new SolidColorBrush(Colors.LightSkyBlue);
            texts[1].Foreground = new SolidColorBrush(Colors.YellowGreen);

            texts[0].FontSize = 14;
            texts[0].FontWeight = FontWeights.Medium;

            texts[1].FontSize = 13;
            texts[1].FontStyle = FontStyles.Italic;

            stackPanel.Children.Add(texts[0]);
            stackPanel.Children.Add(texts[1]);
            
            switch (node.Name.LocalName)
            {
                case "Time":
                    texts[0].Text = "Time: ";
                    texts[1].Text = Utilities.GetAttr(node, "Id");
                    return stackPanel;
                case "ScoreManager":
                    texts[0].Text = "Score Manager";
                    return stackPanel;
                case "Asset":
                    texts[0].Text = "Asset: ";
                    texts[1].Text = Utilities.GetAttr(node, "AssetPath");
                    return stackPanel;
                case "MediaCenter":
                    texts[0].Text = "Media Center: ";
                    texts[1].Text = Utilities.GetAttr(node, "Id");
                    return stackPanel;
                case "AudioCenter":
                    texts[0].Text = "Audio Center";
                    return stackPanel;
                case "GUIPanel":
                    texts[0].Text = "GUI Panel: ";
                    texts[1].Text = Utilities.GetAttr(node, "ExternalXmlPath");
                    return stackPanel;
                case "GUIDialogue":
                    texts[0].Text = "GUI Dialogue: ";
                    texts[1].Text = Utilities.GetAttr(node, "ExternalXmlPath");
                    return stackPanel;
                case "GUIScreen":
                    texts[0].Text = "GUI Screen: ";
                    texts[1].Text = Utilities.GetAttr(node, "ExternalXmlPath");
                    return stackPanel;
                case "GUIPopup":
                    texts[0].Text = "GUI Popup: ";
                    texts[1].Text = Utilities.GetAttr(node, "ExternalXmlPath");
                    return stackPanel;
                case "GUITooltip3D":
                    texts[0].Text = "GUI Tooltip 3D: ";
                    texts[1].Text = Utilities.GetAttr(node, "Id");
                    return stackPanel;
                case "Label":
                    texts[0].Text = "Label: ";
                    texts[1].Text = Utilities.GetAttr(node, "Align");
                    return stackPanel;
                case "Screen":
                    texts[0].Text = "Screen: ";
                    texts[1].Text = Utilities.GetAttr(node, "GUIScreenId");
                    return stackPanel;
                case "Option":
                    texts[0].Text = "Option: ";
                    texts[1].Text = Utilities.GetAttr(node, "Sentence");
                    return stackPanel;
                case "MoteTo":
                    texts[0].Text = "Move to: ";
                    texts[1].Text = Utilities.GetAttr(node, "Element");
                    return stackPanel;
                case "StartConversation":
                    texts[0].Text = "StartConversation: ";
                    texts[1].Text = Utilities.GetAttr(node, "ConversationTargetId");
                    return stackPanel;
                case "Scene":
                    texts[0].Text = "Scene: ";
                    texts[1].Text = Utilities.GetAttr(node, "Title");
                    return stackPanel;
                case "Debug":
                    texts[0].Text = "Debug: ";
                    texts[1].Text = Utilities.GetAttr(node, "Message");
                    return stackPanel;
                case "GotoState":
                    texts[0].Text = "Goto State: ";
                    texts[1].Text = Utilities.GetAttr(node, "StateId");
                    return stackPanel;

                default:
                    texts[0].Text = node.Name.LocalName;
                    if (node.Attribute("Id")?.Value != null)
                    {
                        texts[0].Text += ": ";
                        texts[1].Text = Utilities.GetAttr(node, "Id");
                    }

                    return stackPanel;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XmlOutline.CustomScripts
{
    public class Utilities
    {

        public static string GenerateName(XElement node)
        {
            switch (node.Name.LocalName)
            {
                case "Time":
                    return "Time - " + GetAttr(node, "Id");
                case "ScoreManager":
                    return "Score Manager";
                case "Asset":
                    return "Asset - " + GetAttr(node, "AssetPath");
                case "MediaCenter":
                    return "Media Center - " + GetAttr(node, "Id");
                case "AudioCenter":
                    return "Audio Center";
                case "GUIPanel":
                    return "GUI Panel - " + GetAttr(node, "ExternalXmlPath");
                case "GUIDialogue":
                    return "GUI Dialogue - " + GetAttr(node, "ExternalXmlPath");
                case "GUIScreen":
                    return "GUI Screen - " + GetAttr(node, "ExternalXmlPath");
                case "GUIPopup":
                    return "GUI Popup - " + GetAttr(node, "ExternalXmlPath");
                case "GUITooltip3D":
                    return "GUI Tooltip 3D - " + GetAttr(node, "Id");
                case "Label":
                    return "Label - " + GetAttr(node, "Align");
                case "Screen":
                    return "Screen - " + GetAttr(node, "GUIScreenId");
                case "Option":
                    return "Option - " + GetAttr(node, "Sentence");
                case "MoteTo":
                    return "Move to - " + GetAttr(node, "Element");
                case "StartConversation":
                    return "StartConversation - " + GetAttr(node, "ConversationTargetId");
                case "Scene":
                    return "Scene - " + GetAttr(node, "Title");
                case "Debug":
                    return "Debug - " + GetAttr(node, "Message");
                case "GotoState":
                    return "Goto State - " + GetAttr(node, "StateId");
                    
                default:
                    if(node.Attribute("Id")?.Value != null)
                        return node.Name.LocalName + " - " + node.Attribute("Id")?.Value;
                    return node.Name.LocalName;
            }   
        }
        
        private static string GetAttr(XElement node, string id)
        {
            return node.Attribute(id) != null ? node.Attribute(id)?.Value : "";
        }
    }
}

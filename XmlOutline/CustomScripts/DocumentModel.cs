using System;
using System.Diagnostics;
using System.Xml.Linq;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xaml;
using System.Xml;
using System.Xml.XPath;


namespace XmlOutline.CustomScripts
{
    /// <summary>
    /// Hold the full path and name as an identifier, the xml document and the outline structure
    /// </summary>
    class DocumentModel
    {
        public string FullPath;
        private XDocument xmlDocument;
        public TreeView Tree;
        
        /// <summary>
        /// constructor, saves the full path of the file.
        /// </summary>
        /// <param name="path"></param>
        public DocumentModel(string path)
        {
            FullPath = path;
//	        UpdateXml();
		}

        /// <summary>
        /// saves the xml data locally and updates the treeview
        /// </summary>
        public void UpdateXml()
        {
//            var xml = System.IO.File.ReadAllText(FullPath);
            xmlDocument = XDocument.Load(FullPath, LoadOptions.SetLineInfo);
            xmlDocument.DescendantNodes().OfType<XComment>().Remove();
            Tree = new TreeView {Name = "treeview_1"};
            var firstNode = xmlDocument.Descendants().First();
            var treeItm = new TreeViewItem
            {
                Header = firstNode.Name.LocalName + " - " + firstNode.FirstAttribute,
                Tag = firstNode.AbsoluteXPath()
            };
            treeItm.Selected += NodeSelected;
            Tree.Items.Add(treeItm);
            AddNodes(xmlDocument.Descendants().First(), treeItm);
        }

        /// <summary>
        /// A deepth first recursive function that traverses the xml document and creates a treeview
        /// </summary>
        /// <param name="lastNode"></param>
        /// <param name="lastTreeItm"></param>
        public void AddNodes(XElement lastNode, TreeViewItem lastTreeItm)
        {
            var ancestors = lastNode.Descendants();
            var xElements = ancestors.ToList();
            if (xElements.Any())
            {
                var treeItm = new TreeViewItem
                {
                    Header = xElements.First().Name.LocalName,
                    Tag = xElements.First().AbsoluteXPath()
                };
                treeItm.Selected += NodeSelected;
                lastTreeItm.Items.Add(treeItm);
                AddNodes(xElements.First(), treeItm);
            }


            XElement sibl = (XElement)lastNode.NextNode;
            if (sibl != null)
            {
                var treeItm = new TreeViewItem
                {
                    Header = sibl.Name.LocalName,
                    Tag = sibl.AbsoluteXPath()
                };
                treeItm.Selected += NodeSelected;
                ((TreeViewItem) lastTreeItm.Parent).Items.Add(treeItm);
                AddNodes(sibl, treeItm);
            } 
        }

        public void NodeSelected(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var xPath = (string) ((TreeViewItem)sender).Tag;
            var xmlNode = (IXmlLineInfo)xmlDocument.XPathSelectElement(xPath);
            if (xmlNode != null)
            {
                var lineNumber = xmlNode.LineNumber;
                OutlineManager.Instance.TreeElementSelected(lineNumber);
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.Xml.Linq;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.VisualStudio.Shell;


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
		}

        public TreeView UpdateTree()
        {
            if (Tree == null)
                return Tree = CreateTree();

            var tree = CreateTree();

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(tree); i++)
            {
                var child = (TreeViewItem)VisualTreeHelper.GetChild(tree, i);
                var originalChild = (TreeViewItem)VisualTreeHelper.GetChild(Tree, i);

                if (child.Header == originalChild.Header)
                {
                    child.IsExpanded = originalChild.IsExpanded;
                }
            }
            return Tree;
        }

        
        /// <summary>
        /// saves the xml data locally and updates the treeview
        /// </summary>
        private TreeView CreateTree ()
        {
            xmlDocument = XDocument.Load(FullPath, LoadOptions.SetLineInfo);
            xmlDocument.DescendantNodes().OfType<XComment>().Remove();
//            Tree = new TreeView
            var tree = new TreeView
            {
                Name = "treeview_1",
                Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#1e1e1e")
            };

            
            var firstNode = xmlDocument.Descendants().First();
            var treeItm = new TreeViewItem
            {
                Header = Utilities.GenerateName(firstNode),
                Tag = firstNode.AbsoluteXPath(),
                Foreground = Brushes.WhiteSmoke
            };
            
            treeItm.Selected += NodeSelected;
            tree.Items.Add(treeItm);
            AddNodes(xmlDocument.Descendants().First(), treeItm);
            return tree;
        }
        
        
        /// <summary>
        /// A deepth first recursive function that traverses the xml document and creates a treeview
        /// </summary>
        /// <param name="lastNode"></param>
        /// <param name="lastTreeItm"></param>
        public void AddNodes(XElement lastNode, TreeViewItem lastTreeItm)
        {
            var xElements = lastNode.Descendants().ToList();
            if (xElements.Any())
            {
                var treeItm = new TreeViewItem
                {
                    Header = Utilities.GenerateName(xElements.First()),
                    Tag = xElements.First().AbsoluteXPath(),
                    Foreground = Brushes.WhiteSmoke
                };
                treeItm.Selected += NodeSelected;
                lastTreeItm.Items.Add(treeItm);
                AddNodes(xElements.First(), treeItm);
            }


            var sibl = (XElement)lastNode.NextNode;
            if (sibl != null)
            {
                var treeItm = new TreeViewItem
                {
                    Header = Utilities.GenerateName(sibl),
                    Tag = sibl.AbsoluteXPath(),
                    Foreground = Brushes.WhiteSmoke
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
            Debug.WriteLine("here it is : : : " + ((TreeViewItem)sender).IsExpanded);
            if (xmlNode != null)
            {
                var lineNumber = xmlNode.LineNumber;
                OutlineManager.Instance.TreeElementSelected(lineNumber);
            }
        }
    }
}




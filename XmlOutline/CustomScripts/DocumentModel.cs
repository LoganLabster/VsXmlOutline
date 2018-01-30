using System;
using System.Xml.Linq;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
		}

        /// <summary>
        /// Updates the treeview based on the XML
        /// Attempts to keep the nodes open if they were before
        /// </summary>
        /// <returns></returns>
        public TreeView UpdateTree()
        {
            if (Tree == null)
            {
                Tree = CreateTree();
                LogicalTreeHelper.GetChildren(Tree).Cast<TreeViewItem>().ToList().First().IsExpanded = true;

                return Tree;
            }
            
            var tree = CreateTree();
            if (tree == null) return null;

            /*TODO This is incredibly fucking heavy when the document reaches 1000 lines or more, at 9000 lines my program just downright froze.
              I will need to find a way to do it, but for now it works without this feature
             */
//            var allNodes = xmlDocument.Descendants().ToList();
//            foreach (var node in allNodes)
//            {
//                var fixedName = Utilities.XpathToName(node.AbsoluteXPath());
//                var newItem = (TreeViewItem)LogicalTreeHelper.FindLogicalNode(tree, fixedName);
//                var oldItem = (TreeViewItem)LogicalTreeHelper.FindLogicalNode(Tree, fixedName);
//
//                if (newItem != null && oldItem != null)
//                {
//                    newItem.IsExpanded = oldItem.IsExpanded;
//                }
//            }

            var newItems = LogicalTreeHelper.GetChildren(tree).Cast<TreeViewItem>().ToList();
            newItems.First().IsExpanded = true;

            Tree = tree;
            return Tree;
        }

        
        /// <summary>
        /// saves the xml data locally and updates the treeview
        /// </summary>
        private TreeView CreateTree ()
        {
            try
            {
                xmlDocument = XDocument.Load(FullPath, LoadOptions.SetLineInfo);
                xmlDocument.DescendantNodes().OfType<XComment>().Remove();
                var tree = new TreeView
                {
                    Name = "treeview_1",
                    Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#1e1e1e")
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
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
                var nameFixed = Utilities.XpathToName(xElements.First().AbsoluteXPath());

                var treeItm = new TreeViewItem
                {
                    Header = Utilities.GenerateName(xElements.First()),
                    Tag = xElements.First().AbsoluteXPath(),
                    Foreground = Brushes.WhiteSmoke,
                    Name = nameFixed
                };
                treeItm.Selected += NodeSelected;
                lastTreeItm.Items.Add(treeItm);
                AddNodes(xElements.First(), treeItm);
            }

            var sibl = (XElement)lastNode.NextNode;
            if (sibl != null)
            {
                var nameFixed = Utilities.XpathToName(sibl.AbsoluteXPath());

                var treeItm = new TreeViewItem
                {
                    Header = Utilities.GenerateName(sibl),
                    Tag = sibl.AbsoluteXPath(),
                    Foreground = Brushes.WhiteSmoke,
                    Name = nameFixed
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




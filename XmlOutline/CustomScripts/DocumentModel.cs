using System;
using System.Collections;
using System.Collections.Generic;
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
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;


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
            {
                Tree = CreateTree();
                LogicalTreeHelper.GetChildren(Tree).Cast<TreeViewItem>().ToList().First().IsExpanded = true;

                return Tree;
            }


            var tree = CreateTree();
            if (tree == null) return null;


//            throw new Exception("Traverse the document and keep items expanded if they were expanded");

            /*
            //            var newItems = LogicalTreeHelper.GetChildren(tree).Cast<TreeViewItem>().ToList();
            //            var originalItems = LogicalTreeHelper.GetChildren(Tree).Cast<TreeViewItem>().ToList();
            //
            //            for (int i = 0; i < newItems.Count; i++)
            //            {
            //                var newItem = newItems[i];
            //
            //                var selected = originalItems.First(x => x.Header == newItem.Header);
            //                if (selected != null)
            //                {
            //                    newItem.IsExpanded = selected.IsExpanded;
            //                }
            //            }
            //            while (enumerable.GetEnumerator().MoveNext())
            //            {
            //                var item = (TreeViewItem) enumerable.GetEnumerator().Current;
            //                var isExpanded = item.IsExpanded;
            //
            //            }

            //            var childCount = VisualTreeHelper.GetChildrenCount(Tree);
            //            for (var i = 0; i < childCount; i++)
            //            {
            //                var child = (TreeViewItem)VisualTreeHelper.GetChild(tree, i);
            //                var originalChild = (TreeViewItem)VisualTreeHelper.GetChild(Tree, i);
            //
            //                if (child.Header == originalChild.Header)
            //                {
            //                    child.IsExpanded = originalChild.IsExpanded;
            //                }
            //            }
            */


            var allNodes = xmlDocument.Descendants().ToList();
            var count = allNodes.Count();
            foreach (var node in allNodes)
            {
                var newItem = (TreeViewItem)LogicalTreeHelper.FindLogicalNode(tree, node.AbsoluteXPath());
                var oldItem = (TreeViewItem)LogicalTreeHelper.FindLogicalNode(Tree, node.AbsoluteXPath());

                if (newItem != null && oldItem != null)
                {
                    newItem.IsExpanded = oldItem.IsExpanded;
                }
            }

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
                var nameFixed = xElements.First().AbsoluteXPath().Replace("/", "_slash_");
                nameFixed = nameFixed.Replace("[", "_start_").Replace("]", "_end_");

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
                var nameFixed = sibl.AbsoluteXPath().Replace("/", "_slash_");
                nameFixed = nameFixed.Replace("[", "_start_").Replace("]", "_end_");

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




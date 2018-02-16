using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using XmlOutline.CustomScripts;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Utilities = XmlOutline.CustomScripts.Utilities;

namespace XmlOutline
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for OutlineWindowControl.
    /// </summary>
    public partial class OutlineWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutlineWindowControl"/> class.
        /// </summary>
        public OutlineWindowControl()
        {
            this.InitializeComponent();
        }
        
        /// <summary>
        /// Called when a treeviewitem is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            e.Handled = true;
            if (e.NewValue == null) return;
            var path = Utilities.FindXPath((XmlNode)e.NewValue);
            OutlineManager.Instance.TreeElementSelected(path);
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            var elem = (XmlElement) ((TreeViewItem) e.OriginalSource).DataContext;
            var path = Utilities.FindXPath(elem);
            
            var node = new NodeData
            {
                Path = path,
                Header = (XmlElement)((TreeViewItem)e.OriginalSource).Header,
            };
            var existingNode = OutlineManager.Instance.nodes.FirstOrDefault(x => Equals(x.Path, path));
            if (existingNode != null) return;
            OutlineManager.Instance.nodes.Add(node);
        }

        private void TreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            var elem = (XmlElement)((TreeViewItem)e.OriginalSource).DataContext;
            var path = Utilities.FindXPath(elem);

            var selectedNode = OutlineManager.Instance.nodes.FirstOrDefault(x => Equals(x.Path, path));
            if(selectedNode != null)
                OutlineManager.Instance.nodes.Remove(selectedNode);
        }

//        private void TreeView_Updated(object sender, DependencyPropertyChangedEventArgs e)
//        {
////            eventArgs.Handled = true;
//            if (sender == null) return;
//            if(OutlineManager.Instance.nodes == null || OutlineManager.Instance.nodes.Count == 0) return;
//            foreach (var node in OutlineManager.Instance.nodes)
//            {
//                var pathStrings = node.Path.Split('/').ToList();
//                pathStrings.RemoveAll(string.IsNullOrEmpty);
//                for (int i = 0; i < pathStrings.Count; i++)
//                {
//                    pathStrings[i] = pathStrings[i].Replace("{", "").Replace("}", "");
//                    pathStrings[i] = pathStrings[i].Replace("[", "|").Replace("]", "");
//                }
//                //Needs to run after the tree has refreshed
//                ExpandTree(TreeItems.ItemContainerGenerator, pathStrings, 0);
//                //TODO the only way to do this propperly is with a recursive method .. .-. '-' *-' *_* '-* '_' .-. ..
//            }
//        }
//
//        void ExpandTree(ItemContainerGenerator itemses, List<string> treeNodes, int step)
//        {
//            //Get container and expand, if there are more nodes in the treenodes then continue to children
//            //How do I get the current container?
//            var name = treeNodes[step].Split('|')[0];
//            var number = int.Parse(treeNodes[step].Split('|')[1]) - 1;
//            var selected = itemses.Items.Cast<XmlElement>().Where(x => x.LocalName == name).ToList()[number];
//            var selectedItem = ((TreeViewItem)itemses.ContainerFromItem(selected));
//            selectedItem.IsExpanded = true;
//
//            step++;
//            if (treeNodes.Count > step)
//            {
//                ExpandTree(selectedItem.ItemContainerGenerator, treeNodes, step);
//            }
//            //            itemses.ContainerFromItem()
//        }
    }
}
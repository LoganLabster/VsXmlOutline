using System;
using System.Linq;
using System.Windows;
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
                Header = (XmlElement)((TreeViewItem)e.OriginalSource).Header
            };
            var existingNode = OutlineManager.Instance.nodes.FirstOrDefault(x => Equals(x.Path, path));
            if (existingNode != null) return;
            OutlineManager.Instance.nodes.Add(node);
            
            


//            var existingNode = OutlineManager.Instance.nodes.FirstOrDefault(x => Equals(x.TreeItem, (TreeViewItem)e.OriginalSource));
//            if (existingNode != null) return;
//
//            var node = new NodeData
//            {
//                TreeItem = e.OriginalSource as TreeViewItem
//            };
//            OutlineManager.Instance.nodes.Add(node);
        }

        private void TreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            var elem = (XmlElement)((TreeViewItem)e.OriginalSource).DataContext;
            var path = Utilities.FindXPath(elem);

            var selectedNode = OutlineManager.Instance.nodes.FirstOrDefault(x => Equals(x.Path, path));
            if(selectedNode != null)
                OutlineManager.Instance.nodes.Remove(selectedNode);





//            var selectedNode =
//                OutlineManager.Instance.nodes.FirstOrDefault(x => Equals(x.TreeItem, (TreeViewItem) e.OriginalSource));
//            if (selectedNode != null)
//                OutlineManager.Instance.nodes.Remove(selectedNode);
        }
    }
}
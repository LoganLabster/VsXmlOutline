using System;
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
    }
}
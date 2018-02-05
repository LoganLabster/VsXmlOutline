using System.Windows;
using System.Xml;
using System.Xml.Linq;

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

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //E contains both the entire (old) and the selected (new), could just calculate the linenumber here
            OutlineManager.Instance.TreeElementSelected(XElement.Parse(((XmlElement)e.NewValue).OuterXml));
        }
    }
}
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace XmlOutline.CustomScripts
{
    class MyTreeViewNode : TreeViewItem//, INotifyPropertyChanged
    {
        public MyTreeViewNode()
        {
            
        }

        
//        private bool _isExpanded;
//        public new bool IsExpanded
//        {
//            get { return _isExpanded; }
//            set
//            {
//                if (value != _isExpanded)
//                {
//                    _isExpanded = value;
//                    this.OnPropertyChanged();
//                }
//
//                // Expand all the way up to the root.
//                if (_isExpanded)
//                    ((MyTreeViewNode) Parent).IsExpanded = true;
//            }
//        }
//
//        public event PropertyChangedEventHandler PropertyChanged;
//
//        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
//        {
//            if (PropertyChanged != null)
//                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using XmlOutline.CustomScripts;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Window = EnvDTE.Window;

namespace XmlOutline
{
    public class OutlineManager
    {
        public static OutlineManager Instance;

        public List<OpennedDocument> Documents = new List<OpennedDocument>();

        private DTE dte;
        private Events events;
        private WindowEvents windowEvents;
        private DocumentEvents documentEvents;
        private Window codeWindow;

        private OutlineWindowControl _outlineWindow;
        public OutlineWindowControl OutlineWindowInstance => _outlineWindow ?? (_outlineWindow = (OutlineWindowControl) OutlineWindow.Instance.Content);

        /// <summary>
        /// Initializes the manager and sets up all event handlers
        /// </summary>
        public OutlineManager()
        {
            Instance = this;

            dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            if (dte == null) throw new Exception("dte could not be found");
            events = dte.Events;
            windowEvents = events.WindowEvents;
            documentEvents = events.DocumentEvents;

            windowEvents.WindowActivated += OnWindowActivated;
            documentEvents.DocumentClosing += OnDocumentClosed;
            windowEvents.WindowCreated += OnWindowOpenned;
            
            documentEvents.DocumentSaved += RefreshData;
        }

        private void OnWindowOpenned(Window window)
        {
            if (window.Document?.Language == "XML")
            {
                if (window == codeWindow) return;
                var doc = new OpennedDocument
                {
                    DocName = window.Document.FullName,
                    ExpandedNodes = new List<string>()
                };
                Documents.Add(doc);
                ToggleTree(true);

                if (OutlineWindowInstance.TreeItems.DataContext == null || OutlineWindowInstance.TreeItems.DataContext is string)
                {
                    var provider = new XmlDataProvider()
                    {
                        Source = new Uri(window.Document.Path + window.Document.Name),
                        XPath = "./*"
                    };
                    OutlineWindowInstance.TreeItems.DataContext = provider;
                }
                else
                {
                    RefreshData();
                }

                if (codeWindow == null ||
                    codeWindow != window)
                    codeWindow = window;
            }
            else if (window.Kind != "Tool")
            {
                codeWindow = null;
                ToggleTree(false);
            }
        }
        
        /// <summary>
        /// Removes the closed window from the list if it's an XML document
        /// </summary>
        /// <param name="document"></param>
        private void OnDocumentClosed(Document document)
        {
            Documents.RemoveAll(x => x.DocName == document.FullName);
            
//            if (document.ActiveWindow != codeWindow) return;
            if(dte.ActiveDocument == null)
                ToggleTree(false);
        }

        
        public void TreeElementSelected(string path)
        {
            if (path == null || dte.ActiveWindow.Type != EnvDTE.vsWindowType.vsWindowTypeToolWindow) return;
            var d = (TextDocument)dte.ActiveDocument.Object("TextDocument");
            var p = d.StartPoint.CreateEditPoint();
            var s = p.GetText(d.EndPoint);
            var doc = XDocument.Parse(s, LoadOptions.SetLineInfo);
            IXmlLineInfo info = doc.XPathSelectElement(path);
            if (info != null)
            {
                int lineNumber = info.HasLineInfo() ? info.LineNumber : -1;
                ((EnvDTE.TextDocument)dte.ActiveDocument.Object()).Selection.GotoLine(lineNumber);
            }
        }

        /// <summary>
        /// Refreshes the data in the provider and also ensures that the treeview is rebuilt at the right time
        /// </summary>
        /// <param name="document"></param>
        private void RefreshData(Document document = null)
        {
            var prov = (XmlDataProvider) OutlineWindowInstance.TreeItems.DataContext;
            ToggleTree(true);
            prov.Refresh();
            //TODO can I move these so they are only called once?
            prov.DataChanged -= UnpackOpened;
            prov.DataChanged += UnpackOpened;
        }

        private void UnpackOpened(object sender, EventArgs e)
        {
            var currentDoc = Documents.Single(x => x.DocName == dte.ActiveDocument.FullName);
            currentDoc.IsRefreshing = true;

            for (var index = 0; index < currentDoc.ExpandedNodes.Count; index++)
            {
                var node = currentDoc.ExpandedNodes[index];
                var pathStrings = node.Split('/').ToList();
                pathStrings.RemoveAll(string.IsNullOrEmpty);
                for (int i = 0; i < pathStrings.Count; i++)
                {
                    pathStrings[i] = pathStrings[i].Replace("{", "").Replace("}", "");
                    pathStrings[i] = pathStrings[i].Replace("[", "|").Replace("]", "");
                }

                ExpandTree(OutlineWindowInstance.TreeItems.ItemContainerGenerator, pathStrings, 0,
                    OutlineWindowInstance.TreeItems);
            }

            currentDoc.IsRefreshing = false;
        }

        void ExpandTree(ItemContainerGenerator itemses, List<string> treeNodes, int step, ItemsControl last)
        {
            while (true)
            {
                last.UpdateLayout();
                var name = treeNodes[step].Split('|')[0];
                var number = int.Parse(treeNodes[step].Split('|')[1]) - 1;
                var selected = itemses.Items.Cast<XmlElement>().Where(x => x.LocalName == name).ToList()[number];
                var selectedItem = ((TreeViewItem) itemses.ContainerFromItem(selected));
                if (selectedItem == null) return;
                selectedItem.IsExpanded = true;

                step++;
                if (treeNodes.Count > step)
                {
                    itemses = selectedItem.ItemContainerGenerator;
                    last = selectedItem;
                    continue;
                }
                break;
            }
        }
        
        /// <summary>
        /// Called when a new window has focus
        /// </summary>
        /// <param name="gotFocus"></param>
        /// <param name="lostFocus"></param>
        private void OnWindowActivated(Window gotFocus, Window lostFocus)
        {
            if (gotFocus.Document?.Language == "XML")
            {
                ((XmlDataProvider) OutlineWindowInstance.TreeItems.DataContext).Source =
                    new Uri(gotFocus.Document.Path + gotFocus.Document.Name);
                RefreshData();
            }
            else if (gotFocus.Kind != "Tool")
            {
                codeWindow = null;
                ToggleTree(false);
            }
        }

        /// <summary>
        /// toggles whether or not the tree is rendered
        /// </summary>
        /// <param name="showTree"></param>
        public void ToggleTree(bool showTree)
        {
            if (showTree)
            {
                OutlineWindowInstance.TreeItems.Visibility = Visibility.Visible;
                OutlineWindowInstance.LogoPanel.Visibility = Visibility.Hidden;
            }
            else
            {
                OutlineWindowInstance.TreeItems.Visibility = Visibility.Hidden;
                OutlineWindowInstance.LogoPanel.Visibility = Visibility.Visible;
            }
        }
        
        /// <summary>
        /// Called when a node in the treeview is expanded
        /// </summary>
        /// <param name="path">The XPath of the node</param>
        public void NodeExpanded(string path)
        {
            var currentDoc = Documents.Single(x => x.DocName == dte.ActiveDocument.FullName);
            currentDoc.AddExpandedNode(path);
        }

        /// <summary>
        /// Called when a node in the treeview is collapsed
        /// </summary>
        /// <param name="path">The XPath of the node</param>
        public void NodeCollapsed(string path)
        {
            var currentDoc = Documents.Single(x => x.DocName == dte.ActiveDocument.FullName);
            currentDoc.RemoveExpandedNode(path);
        }
    }
}
 
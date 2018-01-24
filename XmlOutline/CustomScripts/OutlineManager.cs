using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Controls;
using XmlOutline.CustomScripts;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace XmlOutline
{
    
    public class OutlineManager
    {
        private DTE dte;
        private List<DocumentModel> xmlDocuments = new List<DocumentModel>();

        public static OutlineManager Instance;
        
        public OutlineManager()
        {
            Instance = this;

            dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            if (dte == null) throw new Exception("dte could not be found");
            
            dte.Events.WindowEvents.WindowActivated += OnWindowActivated;
            dte.Events.WindowEvents.WindowCreated += OnWindowOpened;
            dte.Events.WindowEvents.WindowClosing += OnWindowClosed;
        }

        

        private void OnWindowActivated(Window gotFocus, Window lostFocus)
        {
            if(gotFocus.Document?.Language == "XML")
            {
//                var nodeTitle = new Label{Content = gotFocus.Document.Name};
//                ((OutlineWindowControl) OutlineWindow.Instance.Content).StackPanel.Children.Add(nodeTitle);
                Debug.WriteLine("It's an xml file!");

                var doc = xmlDocuments.Single(x => x.FullPath == gotFocus.Document.FullName);
                doc.UpdateXml();
                var tree = doc.Tree;
                ((OutlineWindowControl)OutlineWindow.Instance.Content).Grid.Children.Clear();
                ((OutlineWindowControl) OutlineWindow.Instance.Content).Grid.Children.Add(tree);
            }
            else
            {
                Debug.WriteLine("Set window to empty");
            }
        }

        private void OnWindowOpened(Window window)
        {
            Debug.WriteLine("If it's an xml document add it to the list");
            if (window.Document?.Language == "XML")
            {
                var xmlDoc = new DocumentModel(window.Document.FullName);
                xmlDocuments.Add(xmlDoc);
            }
        }

        private void OnWindowClosed(Window window)
        {
            Debug.WriteLine("Remove xml document");
            if (window?.Document?.Language == "XML")
            {
                var selectedDoc = xmlDocuments.Single(x => x.FullPath == window.Document.FullName);
                if (selectedDoc != null)
                    xmlDocuments.Remove(selectedDoc);
            }
        }

        public void TreeElementSelected(int lineNumber)
        {
            Debug.WriteLine("Go to line number : " + lineNumber);
            //Go to document thingy
        }
    }
}

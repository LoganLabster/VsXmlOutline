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

        private Window codeWindow ;

        private OutlineWindowControl _outlineWindow;
        public OutlineWindowControl OutlineWindowInstance => _outlineWindow ?? (_outlineWindow = (OutlineWindowControl) OutlineWindow.Instance.Content);

        
        public OutlineManager()
        {
            Instance = this;
            

            dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            if (dte == null) throw new Exception("dte could not be found");
            
            dte.Events.WindowEvents.WindowActivated += OnWindowActivated;
            dte.Events.WindowEvents.WindowCreated += OnWindowOpened;
            dte.Events.WindowEvents.WindowClosing += OnWindowClosed;
            dte.Events.TextEditorEvents.LineChanged += OnLineChanged;
        }

        private void OnWindowActivated(Window gotFocus, Window lostFocus)
        {
            if(gotFocus.Document?.Language == "XML")
            {
                if(gotFocus == codeWindow) return;
                
                var doc = xmlDocuments.Single(x => x.FullPath == gotFocus.Document.FullName);
                var tree = doc.UpdateXml();
                OutlineWindowInstance.Grid.Children.Clear();
                OutlineWindowInstance.Grid.Children.Add(tree);
                
                if (codeWindow == null || 
                    codeWindow != gotFocus)
                    codeWindow = gotFocus;
            }
            else if (gotFocus.Kind != "Tool")
            {
                codeWindow = null;
                OutlineWindowInstance.Grid.Children.Clear();
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

        private void OnLineChanged(TextPoint startpoint, TextPoint endpoint, int hint)
        {
            Debug.WriteLine("did stuff");
        }
        
        public void TreeElementSelected(int lineNumber)
        {
            Debug.WriteLine("Go to line number : " + lineNumber);
            if (codeWindow != null)
            {
                var doc = (EnvDTE.TextDocument) dte.ActiveDocument.Object();
                doc.Selection.GotoLine(lineNumber);
            }
        }

    }
}

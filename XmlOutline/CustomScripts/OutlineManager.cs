using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using XmlOutline.CustomScripts;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace XmlOutline
{
    
    public class OutlineManager
    {
        public static OutlineManager Instance;

        private List<DocumentModel> xmlDocuments = new List<DocumentModel>();

        private DTE dte;
        private Events events;
        private WindowEvents windowEvents;
        private TextEditorEvents textEditorEvents;
        private DocumentEvents documentEvents;
        private Window codeWindow;

        private int _changeCount = 0;

        private OutlineWindowControl _outlineWindow;
        public OutlineWindowControl OutlineWindowInstance => _outlineWindow ?? (_outlineWindow = (OutlineWindowControl) OutlineWindow.Instance.Content);

        
        public OutlineManager()
        {
            Instance = this;
            

            dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            if (dte == null) throw new Exception("dte could not be found");
            events = dte.Events;
            windowEvents = events.WindowEvents;
            textEditorEvents = events.TextEditorEvents;
            documentEvents = events.DocumentEvents;

            windowEvents.WindowActivated += OnWindowActivated;
            windowEvents.WindowCreated += OnWindowOpened;
            windowEvents.WindowClosing += OnWindowClosed;
            
            textEditorEvents.LineChanged += OnLineChange;
            documentEvents.DocumentSaved += DocumentChanged;
        }

        private void DocumentChanged(Document document)
        {
//            throw new NotImplementedException();
        }

        private void OnLineChange(TextPoint startpoint, TextPoint endpoint, int hint)
        {
            if (dte.ActiveWindow.Document.Language != "XML") return;

            _changeCount++;
            if (_changeCount > 20)
            {
                _changeCount = 0;
                var doc = xmlDocuments.Single(x => x.FullPath == dte.ActiveWindow.Document.FullName);
                var tree = doc.UpdateTree();
                OutlineWindowInstance.Grid.Children.Clear();
                OutlineWindowInstance.Grid.Children.Add(tree);
                throw new Exception("Should take time into account too");
            }
        }

        private void OnWindowActivated(Window gotFocus, Window lostFocus)
        {
            _changeCount = 0;
            if(gotFocus.Document?.Language == "XML")
            {
                if(gotFocus == codeWindow) return;
                
                var doc = xmlDocuments.Single(x => x.FullPath == gotFocus.Document.FullName);
                var tree = doc.UpdateTree();
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
            if (codeWindow != null)
            {
                var doc = (EnvDTE.TextDocument) dte.ActiveDocument.Object();
                doc.Selection.GotoLine(lineNumber);
            }
        }

    }
}

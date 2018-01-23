using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using XmlOutline.CustomScripts;

namespace XmlOutline
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    
    public class OutlineManager
    {
        private DTE dte;
        private List<DocumentModel> xmlDocuments = new List<DocumentModel>();
        
        public OutlineManager()
        {
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
                Debug.WriteLine("It's an xml file!");
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
    }
}

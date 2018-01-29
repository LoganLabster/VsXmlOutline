using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using XmlOutline.CustomScripts;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Thread = System.Threading.Thread;
using Window = EnvDTE.Window;

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
            windowEvents.WindowActivated += FirstRender;
            
//            textEditorEvents.LineChanged += OnLineChange;
            documentEvents.DocumentSaved += DocumentChanged;
        }

        private void FirstRender(Window gotfocus, Window lostfocus)
        {
            windowEvents.WindowActivated -= FirstRender;
            ClearTree();
        }

        private void DocumentChanged(Document document)
        {
            if (document.Language != "XML") return;
            RePaint();
        }

//        private void OnLineChange(TextPoint startpoint, TextPoint endpoint, int hint)
//        {
//            if (dte.ActiveWindow.Document.Language != "XML") return;
//            {
//                RePaint();
//            }
//        }


        private void RePaint()
        {
            var doc = xmlDocuments.FirstOrDefault(x => x.FullPath == dte.ActiveWindow.Document.FullName);
            if (doc == null)
            {
                OnWindowOpened(dte.ActiveWindow);
                doc = xmlDocuments.Single(x => x.FullPath == dte.ActiveWindow.Document.FullName);
            }

            var tree = doc.UpdateTree();
            if (tree == null) return;
            
            OutlineWindowInstance.TreeGrid.Children.Clear();
            OutlineWindowInstance.TreeGrid.Children.Add(tree);
//            tree.Items.Refresh();
//            tree.UpdateLayout();
        }

        private void OnWindowActivated(Window gotFocus, Window lostFocus)
        {
            if(gotFocus.Document?.Language == "XML")
            {
                if(gotFocus == codeWindow) return;
                
                RePaint();
                
                if (codeWindow == null || 
                    codeWindow != gotFocus)
                    codeWindow = gotFocus;
            }
            else if (gotFocus.Kind != "Tool")
            {
                codeWindow = null;
                ClearTree();
            }
        }

        private void OnWindowOpened(Window window)
        {
            if (window.Document?.Language == "XML")
            {
                var xmlDoc = new DocumentModel(window.Document.FullName);
                xmlDocuments.Add(xmlDoc);
            }
        }

        private void OnWindowClosed(Window window)
        {
            if (window == codeWindow) ClearTree();
            
            if (window?.Document?.Language == "XML")
            {
                
                var selectedDoc = xmlDocuments.Single(x => x.FullPath == window.Document.FullName);
                if (selectedDoc != null)
                {
                    xmlDocuments.Remove(selectedDoc);
                }
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
        

        public void ClearTree()
        {
            OutlineWindowInstance.TreeGrid.Children.Clear();
            OutlineWindowInstance.TreeGrid.Children.Add(CreateDecal());
        }

        private StackPanel CreateDecal()
        {
            var stackP = new StackPanel { Orientation = Orientation.Vertical };
            stackP.HorizontalAlignment = HorizontalAlignment.Center;
            stackP.VerticalAlignment = VerticalAlignment.Center;

            var title = new TextBlock
            {
                Text = "XML Outliner",
                FontSize = 30,
                Foreground = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                FontWeight = FontWeights.Bold,
                FontStyle = FontStyles.Italic,
                FontFamily = new FontFamily("Century Gothic"),
                Effect = new DropShadowEffect
                {
                    ShadowDepth = 4,
                    Color = Color.FromRgb(30, 30, 30),
                    Direction = 315,
                    Opacity = 0.5,
                    BlurRadius = 4
                }
            };

            var createdBy = new TextBlock
            {
                Text = "-Created by-",
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(55, 55, 55)),
                FontStyle = FontStyles.Italic,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var myName = new TextBlock
            {
                Text = "Tim K. Logan",
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(55, 55, 55)),
                FontStyle = FontStyles.Italic,
                HorizontalAlignment = HorizontalAlignment.Center
            };


            stackP.Children.Add(title);
            stackP.Children.Add(createdBy);
            stackP.Children.Add(myName);

            return stackP;
        }
    }
}

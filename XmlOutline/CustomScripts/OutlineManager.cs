using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using XmlOutline.CustomScripts;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
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
            windowEvents.WindowActivated += FirstRender;
            
            textEditorEvents.LineChanged += OnLineChange;
            documentEvents.DocumentSaved += DocumentChanged;
        }

        private void FirstRender(Window gotfocus, Window lostfocus)
        {
            windowEvents.WindowActivated -= FirstRender;
            ClearTree();
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

                //OutlineWindowInstance.TreeGrid.Children.Clear();
                ClearTree();
                OutlineWindowInstance.TreeGrid.Children.Add(tree);
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

                //OutlineWindowInstance.TreeGrid.Children.Clear();
                ClearTree();
                OutlineWindowInstance.TreeGrid.Children.Add(tree);
                
                if (codeWindow == null || 
                    codeWindow != gotFocus)
                    codeWindow = gotFocus;
            }
            else if (gotFocus.Kind != "Tool")
            {
                codeWindow = null;
                //OutlineWindowInstance.TreeGrid.Children.Clear();
                ClearTree();
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
            if (window == codeWindow)
            {
                //OutlineWindowInstance.TreeGrid.Children.Clear();
                ClearTree();
            }

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
            
            //Create name decal
            var stackP = new StackPanel{Orientation = Orientation.Vertical};
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
            
            OutlineWindowInstance.TreeGrid.Children.Add(stackP);
        }
    }
}

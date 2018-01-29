﻿using System;
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
            windowEvents.WindowCreated += OnWindowOpened;
            windowEvents.WindowClosing += OnWindowClosed;
            windowEvents.WindowActivated += FirstRender;
            
            documentEvents.DocumentSaved += DocumentChanged;
        }

        /// <summary>
        /// First time the document is rendered the toolbox is given the logo
        /// </summary>
        /// <param name="gotfocus"></param>
        /// <param name="lostfocus"></param>
        private void FirstRender(Window gotfocus, Window lostfocus)
        {
            windowEvents.WindowActivated -= FirstRender;
            ClearTree();
        }
        
        /// <summary>
        /// Every time the document is saved it will update the layout
        /// </summary>
        /// <param name="document"></param>
        private void DocumentChanged(Document document)
        {
            if (document.Language != "XML") return;
            RePaint();
        }

        /// <summary>
        /// Updates the layout
        /// </summary>
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
        }

        /// <summary>
        /// Is called when focus goes from one window to another, checks if we should redo the XML document
        /// </summary>
        /// <param name="gotFocus"></param>
        /// <param name="lostFocus"></param>
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

        /// <summary>
        /// Registers new windows if they are XML documents
        /// </summary>
        /// <param name="window"></param>
        private void OnWindowOpened(Window window)
        {
            if (window.Document?.Language == "XML")
            {
                var xmlDoc = new DocumentModel(window.Document.FullName);
                xmlDocuments.Add(xmlDoc);
            }
        }

        /// <summary>
        /// Removes the closed window from the list if it's an XML document
        /// </summary>
        /// <param name="window"></param>
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

        /// <summary>
        /// Called when the user selects a tree element
        /// </summary>
        /// <param name="lineNumber"></param>
        public void TreeElementSelected(int lineNumber)
        {
            Debug.WriteLine("Go to line number : " + lineNumber);
            if (codeWindow != null)
            {
                var doc = (EnvDTE.TextDocument) dte.ActiveDocument.Object();
                doc.Selection.GotoLine(lineNumber);
            }
        }
        

        /// <summary>
        /// Removes the UI content and replaces it with the logo
        /// </summary>
        public void ClearTree()
        {
            OutlineWindowInstance.TreeGrid.Children.Clear();
            OutlineWindowInstance.TreeGrid.Children.Add(CreateDecal());
        }

        /// <summary>
        /// Creates the logo decal
        /// </summary>
        /// <returns></returns>
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

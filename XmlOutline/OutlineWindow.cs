using System;
using System.Windows.Controls;

namespace XmlOutline
{
    using System.Runtime.InteropServices;
    using EnvDTE;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using System.Diagnostics;


    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("bc4c5e8f-a492-4a44-9e57-ec9ad945140e")]
    public class OutlineWindow : ToolWindowPane
    {
        public static OutlineWindow Instance;


        /// <summary>
        /// Initializes a new instance of the <see cref="OutlineWindow"/> class.
        /// </summary>
        public OutlineWindow()
        {
            this.Caption = "OutlineWindow";

            Instance = this;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new OutlineWindowControl();
//            ((OutlineWindowControl) Content).StackPanel.Children.Add()
        }

    }
}

using System;
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
    public class OutlineWindow : ToolWindowPane //, IVsRunningDocTableEvents
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="OutlineWindow"/> class.
        /// </summary>
        public OutlineWindow()
        {
            this.Caption = "OutlineWindow";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new OutlineWindowControl();


        }

        //public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        //{
        //    Debug.WriteLine("Logan : 1");
        //    return VSConstants.S_OK;
        //}

        //public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        //{
        //    Debug.WriteLine("Logan : 2");
        //    return VSConstants.S_OK;
        //}

        //public int OnAfterSave(uint docCookie)
        //{
        //    Debug.WriteLine("Logan : 3");
        //    return VSConstants.S_OK;
        //}

        //public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        //{
        //    Debug.WriteLine("Logan : 4");
        //    return VSConstants.S_OK;
        //}

        //public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        //{
        //    Debug.WriteLine("Logan : 5");
        //    return VSConstants.S_OK;
        //}

        //public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        //{
        //    Debug.WriteLine("Logan : 6");
        //    return VSConstants.S_OK;
        //}
    }
}

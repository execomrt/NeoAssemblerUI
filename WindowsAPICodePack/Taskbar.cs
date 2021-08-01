using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace V3X.WindowsAPICodePack.Taskbar
{
    public enum TaskbarProgressBarState
    {
        NoProgress = 0,
        Indeterminate = 0x1,
        Normal = 0x2,
        Error = 0x4,
        Paused = 0x8
    }
    public class TaskbarManager
    {
        public static class TaskbarProgress
        {
            [ComImport()]
            [Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
            [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            private interface ITaskbarList3
            {
                // ITaskbarList
                [PreserveSig]
                void HrInit();
                [PreserveSig]
                void AddTab(IntPtr hwnd);
                [PreserveSig]
                void DeleteTab(IntPtr hwnd);
                [PreserveSig]
                void ActivateTab(IntPtr hwnd);
                [PreserveSig]
                void SetActiveAlt(IntPtr hwnd);
                // ITaskbarList2
                [PreserveSig]
                void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);
                // ITaskbarList3
                [PreserveSig]
                void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
                [PreserveSig]
                void SetProgressState(IntPtr hwnd, int state);
            }
            [ComImport()]
            [Guid("56fdf344-fd6d-11d0-958a-006097c9a090")]
            [ClassInterface(ClassInterfaceType.None)]
            private class TaskbarInstance
            {
            }
            private static ITaskbarList3 taskbarInstance = (ITaskbarList3)new TaskbarInstance();
            public static void SetState(IntPtr windowHandle, int taskbarState)
            {
                taskbarInstance.SetProgressState(windowHandle, taskbarState);
            }
            public static void SetValue(IntPtr windowHandle, double progressValue, double progressMax)
            {
                taskbarInstance.SetProgressValue(windowHandle, (ulong)progressValue, (ulong)progressMax);
            }
        }
        // Best practice recommends defining a private object to lock on
        private static object _syncLock = new object();
        private static TaskbarManager _instance;
        
        private IntPtr _ownerHandle;
        /// <summary>
        /// Sets the handle of the window whose taskbar button will be used
        /// to display progress.
        /// </summary>
        internal IntPtr OwnerHandle
        {
            get
            {
                if (_ownerHandle == IntPtr.Zero)
                {
                    var currentProcess = Process.GetCurrentProcess();
                    if (currentProcess == null || currentProcess.MainWindowHandle == IntPtr.Zero)
                    {
                    }
                    _ownerHandle = currentProcess.MainWindowHandle;
                }
                return _ownerHandle;
            }
        }
        /// <summary>
        /// Represents an instance of the Windows Taskbar
        /// </summary>
        public static TaskbarManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new TaskbarManager();
                        }
                    }
                }
                return _instance;
            }
        }
        public void SetProgressState(TaskbarProgressBarState state)
        {
            
            TaskbarProgress.SetState(this.OwnerHandle, (int) state);
        }
        public void SetProgressValue(int value, int maxValue)
        {
            
            TaskbarProgress.SetValue(this.OwnerHandle, value, maxValue);
        }
    }
}
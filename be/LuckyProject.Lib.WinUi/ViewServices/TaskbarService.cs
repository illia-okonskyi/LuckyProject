using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LuckyProject.Lib.WinUi.ViewServices
{
    public class TaskbarService : ITaskbarService
    {
        #region ComImported
        private class ComImports
        {
            private readonly ITaskbarList3 taskbarInstance =
                (ITaskbarList3)new TaskbarInstance();
            private readonly bool isTaskbarSupported =
                Environment.OSVersion.Version >= new Version(6, 1);

            [ComImport()]
            [Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
            [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface ITaskbarList3
            {
                // ITaskbarList
                [PreserveSig]
                void HrInit();
                [PreserveSig]
                void AddTab(nint hwnd);
                [PreserveSig]
                void DeleteTab(nint hwnd);
                [PreserveSig]
                void ActivateTab(nint hwnd);
                [PreserveSig]
                void SetActiveAlt(nint hwnd);

                // ITaskbarList2
                [PreserveSig]
                void MarkFullscreenWindow(nint hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

                // ITaskbarList3
                [PreserveSig]
                void SetProgressValue(nint hwnd, ulong ullCompleted, ulong ullTotal);

                [PreserveSig]
                void SetProgressState(nint hwnd, TaskbarState state);
            }

            [ComImport()]
            [Guid("56fdf344-fd6d-11d0-958a-006097c9a090")]
            [ClassInterface(ClassInterfaceType.None)]
            public class TaskbarInstance { }

            public void SetState(TaskbarState taskbarState)
            {
                if (isTaskbarSupported)
                {
                    var handle = Process.GetCurrentProcess().MainWindowHandle;
                    taskbarInstance.SetProgressState(handle, taskbarState);
                }
            }

            public void SetValue(double progressValue, double progressMax)
            {
                if (isTaskbarSupported)
                {
                    var handle = Process.GetCurrentProcess().MainWindowHandle;
                    taskbarInstance.SetProgressValue(
                        handle,
                        (ulong)progressValue,
                        (ulong)progressMax);
                }
            }
        }
        #endregion

        #region Class impl
        private readonly ComImports imports;
        public TaskbarService()
        {
            imports = new ComImports();
        }

        public void SetState(TaskbarState taskbarState) => imports?.SetState(taskbarState);
        public void SetValue(double progressValue, double progressMax) =>
            imports?.SetValue(progressValue, progressMax);
        #endregion
    }
}

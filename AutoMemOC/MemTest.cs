using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using AutoIt;

namespace AutoMemOC
{
    /**
     * This code is a heavily modified version of:
     * https://github.com/integralfx/MemTestHelper/blob/master/MemTestHelper2/MemTest.cs
     **/
    public abstract class MemTest : ProcessBased
    {
        public bool Minimised
        {
            get { return process != null ? WinAPI.IsIconic(process.MainWindowHandle) : false; }
            set
            {
                if (process != null)
                {
                    var hwnd = process.MainWindowHandle;

                    if (value)
                        WinAPI.PostMessage(hwnd, WinAPI.WM_SYSCOMMAND, new IntPtr(WinAPI.SC_MINIMIZE), IntPtr.Zero);
                    else
                    {
                        if (WinAPI.IsIconic(hwnd))
                            WinAPI.ShowWindow(hwnd, WinAPI.SW_RESTORE);
                        else
                            WinAPI.SetForegroundWindow(hwnd);
                    }
                }
            }
        }

        public MemTest(string exePath) : base(exePath)
        {
        }

        public abstract string ExeName();

        public abstract string TestName();

        public override bool Start()
        {
            return Start(false);
        }
        
        public bool Start(bool startMinimised)
        {
            Process[] processes = Process.GetProcessesByName(ExeName());
            if (processes != null && processes.Length > 0)
            {
                foreach (Process process in processes)
                {
                    if (process.HasExited) continue;
                    try
                    {
                        AutoItX.WinKill(process.MainWindowHandle);
                    }
                    catch (Exception) { }
                }
            }

            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = ExeName();
                startInfo.WorkingDirectory = exePath;
                process = Process.Start(startInfo);
            } catch (Exception e)
            {
                if (Program.verboseLogging)
                {
                    Console.Out.WriteLine(e);
                    Console.Out.WriteLine();
                }
                return false;
            }

            if (Program.verboseLogging)
            {
                Console.Out.WriteLine($"Started {TestName()} {PID,5}, start minimised: {startMinimised}, timeout: {Timeout}");
            }

            var end = DateTime.Now + Timeout;
            // Wait for process to start.
            while (true)
            {
                if (DateTime.Now > end)
                {
                    if (Program.verboseLogging)
                    {
                        Console.Error.WriteLine($"Process {process.Id,5}: Failed to wait for process");
                    }
                    process.Kill();
                    return false;
                }

                if (process.HasExited)
                {
                    if (Program.verboseLogging)
                    {
                        Console.Error.WriteLine($"Process {TestName()} exited before it started!");
                    }
                    return false;
                }
                if (!string.IsNullOrEmpty(process.MainWindowTitle))
                    break;

                Thread.Sleep(100);
                process.Refresh();
            }

            Started = true;

            if (startMinimised)
            {
                end = DateTime.Now + Timeout;
                while (true)
                {
                    if (DateTime.Now > end)
                    {
                        if (Program.verboseLogging)
                        {
                            Console.Error.WriteLine($"Failed to minimise {ExeName()} {PID}");
                            break;
                        }
                    }

                    Minimised = true;
                    if (Minimised) break;
                    Thread.Sleep(500);
                }
            }
            return true;
        }

        public void Stop()
        {
            if (process != null && !process.HasExited && Started && !IsFinished())
            {
                if (Program.verboseLogging)
                {
                    Console.Out.WriteLine($"Stopping {TestName()} {PID}");
                }
                StopInternal();
                Started = false;
            }
        }

        protected abstract void StopInternal();

        public override void Close()
        {
            try
            {
                if (process != null && !process.HasExited)
                    process.Kill();
            }
            catch (Exception) { }

            process = null;
            Started = false;

            CloseInternal();
        }

        protected void CloseInternal()
        {
            List<IntPtr> windows = WinAPI.FindAllWindows(TestName());
            foreach (var window in windows)
            {
                AutoItX.WinKill(window);
            }
        }

        public void CloseSafely()
        {
            if (process != null && !process.HasExited && IsFinished())
            {
                if (Program.verboseLogging) Console.Out.WriteLine($"Closing {TestName()} {PID}");
                Started = false;

                CloseSafelyInternal();
            }
        }

        protected abstract void CloseSafelyInternal();

        public bool IsFinished()
        {
            if (process == null || process.HasExited)
                return false;
            return IsFinishedInternal();
        }

        public abstract bool IsFinishedInternal();

        public bool HasErrors()
        {
            if (process == null || process.HasExited)
                return false;
            return HasErrorsInternal();
        }

        protected abstract bool HasErrorsInternal();
    }

    public class TestMem5 : MemTest
    {
        private const string LOG_LIST = "ListBox1";

        public TestMem5(string exePath) : base(exePath) { }

        public override string ExeName()
        {
            return "TM5.exe";
        }

        public override string TestName()
        {
            return "TestMem5";
        }

        public override bool IsFinishedInternal()
        {
            var output = WinAPI.ControlGetList(process.MainWindowHandle, LOG_LIST);
            return output.Count > 1 && output[output.Count - 2].Contains("Testing completed");
        }

        protected override bool HasErrorsInternal()
        {
            string error = GetValue("Static", 59);
            return !string.IsNullOrWhiteSpace(error);
        }

        protected override void StopInternal()
        {
            AutoItX.ControlClick(process.MainWindowHandle, GetHandle("Button", 6));
        }

        protected override void CloseSafelyInternal()
        {
            List<IntPtr> windows = WinAPI.FindAllWindows("TestMem5");
            foreach (var window in windows)
            {
                WinAPI.ControlClick(window, "Button1");
            }
        }
    }
}

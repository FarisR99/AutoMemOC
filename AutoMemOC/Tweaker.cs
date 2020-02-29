using AutoIt;
using System;
using System.Diagnostics;
using System.Threading;

namespace AutoMemOC
{

    public abstract class Tweaker : ProcessBased
    {
        public bool Ready
        {
            get
            {
                if (process == null || process.HasExited)
                    return false;

                return !WinAPI.IsHungAppWindow(process.MainWindowHandle);
            }
            private set { }
        }

        public Tweaker(string exePath) : base(exePath)
        {
        }

        protected abstract string ExeName();

        public abstract string TweakerName();

        public override bool Start()
        {
            var processes = Process.GetProcessesByName(TweakerName());
            if (processes.Length == 1)
            {
                process = processes[0];
                Thread.Sleep(1000);
                Started = true;
                return true;
            }

            try
            {
                process = Process.Start(exePath + "/" + ExeName());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine();
                return false;
            }

            if (Program.verboseLogging)
            {
                Console.WriteLine($"Started {TweakerName()} {PID,5}, timeout: {Timeout}");
            }

            // Wait for process to start.
            var end = DateTime.Now + TimeSpan.FromSeconds(5);
            while (!process.HasExited && AutoItX.WinExists(process.MainWindowHandle) != 1)
            {
                if (DateTime.Now > end)
                {
                    if (Program.verboseLogging) Console.WriteLine(TweakerName() + " took too long to respond.");
                    process.Kill();
                    return false;
                }
                Thread.Sleep(100);
            }
            if (process.HasExited)
            {
                return false;
            }

            Started = true;
            return true;
        }

        public void Apply()
        {
            if (process != null && !process.HasExited && Started)
            {
                ApplyInternal();
            }
        }

        protected abstract void ApplyInternal();

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
        }

        public string Frequency()
        {
            if (process != null && !process.HasExited)
            {
                return FrequencyInternal();
            }
            throw new NullReferenceException("Tweaker process has terminated");
        }

        protected abstract string FrequencyInternal();

        public string Primaries()
        {
            if (process != null && !process.HasExited)
            {
                return $"{GetTiming(Timing.tCL)}-{GetTiming(Timing.tRCD)}-{GetTiming(Timing.tRP)}-{GetTiming(Timing.tRAS)}";
            }
            throw new NullReferenceException("Tweaker process has terminated");
        }

        public int GetTiming(Timing timing)
        {
            if (process == null || process.HasExited)
            {
                throw new NullReferenceException("Tweaker process has terminated");
            }
            return GetTimingInternal(timing);
        }

        protected abstract int GetTimingInternal(Timing timing);

        public bool SetTiming(Timing timing, string value)
        {
            if (process == null || process.HasExited)
            {
                throw new NullReferenceException("Tweaker process has terminated");
            }
            return SetTimingInternal(timing, value);
        }

        protected abstract bool SetTimingInternal(Timing timing, string value);

        public bool SetTiming(Timing timing, int value)
        {
            if (process == null || process.HasExited)
            {
                throw new NullReferenceException("Tweaker process has terminated");
            }
            if (timing == Timing.tRCD)
            {
                SetTimingWithoutChecks(Timing.tRP, value);
            }
            else if (timing == Timing.tRP)
            {
                SetTimingWithoutChecks(Timing.tRCD, value);
            }
            else if (timing == Timing.tRRD_S)
            {
                SetTimingWithoutChecks(Timing.tRRD_L, value);
                SetTimingWithoutChecks(Timing.tFAW, value * 4);
            }
            else if (timing == Timing.tRRD_L)
            {
                SetTimingWithoutChecks(Timing.tRRD_S, value);
                SetTimingWithoutChecks(Timing.tFAW, value * 4);
            }
            return SetTimingWithoutChecks(timing, value);
        }

        private bool SetTimingWithoutChecks(Timing timing, int value)
        {
            if (timing == Timing.tRFC)
            {
                return SetTiming(timing, $"{value}");
            }
            return SetTimingInternal(timing, value);
        }

        protected abstract bool SetTimingInternal(Timing timing, int value);
    }

    public class MemTweakIt : Tweaker
    {
        public MemTweakIt(string exePath) : base(exePath)
        {
        }

        protected override string ExeName()
        {
            return "MemTweakIt.exe";
        }

        public override string TweakerName()
        {
            return "MemTweakIt";
        }

        protected override void ApplyInternal()
        {
            AutoItX.ControlClick(process.MainWindowHandle, GetHandle("Button", 3));
        }

        protected override string FrequencyInternal()
        {
            return GetValue("Static", 127).Trim();
        }

        protected override int GetTimingInternal(Timing timing)
        {
            int timingValue;
            if (int.TryParse(GetValue("ComboBox", GetInstance(timing)).Trim(), out timingValue))
            {
                return timingValue;
            }
            else
            {
                return -1;
            }
        }

        protected override bool SetTimingInternal(Timing timing, string value)
        {
            IntPtr hwnd = GetHandle("ComboBox", GetInstance(timing));
            if (hwnd == IntPtr.Zero) return false;
            AutoItX.WinActivate(process.MainWindowHandle);
            AutoItX.ControlClick(process.MainWindowHandle, hwnd);
            AutoItX.ControlSend(process.MainWindowHandle, hwnd, "{BACKSPACE}");
            Thread.Sleep(50);
            foreach (var character in value.ToCharArray())
            {
                AutoItX.ControlSend(process.MainWindowHandle, hwnd, $"{character}", 1);
                Thread.Sleep(20);
            }
            return true;
        }

        protected override bool SetTimingInternal(Timing timing, int value)
        {
            try
            {
                IntPtr hwnd = GetHandle("ComboBox", GetInstance(timing));

                int currentTiming = GetTiming(timing);

                if (value < currentTiming)
                {
                    while (value < currentTiming)
                    {
                        AutoItX.ControlSend(process.MainWindowHandle, hwnd, "{UP}");
                        currentTiming--;
                        Thread.Sleep(20);
                    }
                }
                else if (value > currentTiming)
                {
                    while (value > currentTiming)
                    {
                        AutoItX.ControlSend(process.MainWindowHandle, hwnd, "{DOWN}");
                        currentTiming++;
                        Thread.Sleep(20);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                if (Program.verboseLogging)
                {
                    Console.WriteLine();
                    Console.Error.WriteLine(e);
                    Console.WriteLine();
                }
                return false;
            }
        }

        private static int GetInstance(Timing timing)
        {
            if (timing == Timing.tCL)
            {
                return 73;
            }
            else if (timing == Timing.tRCD)
            {
                return 74;
            }
            else if (timing == Timing.tRP)
            {
                return 75;
            }
            else if (timing == Timing.tRAS)
            {
                return 76;
            }
            else if (timing == Timing.tRRD_S)
            {
                return 79;
            }
            else if (timing == Timing.tRRD_L)
            {
                return 78;
            }
            else if (timing == Timing.tFAW)
            {
                return 84;
            }
            else if (timing == Timing.tRFC)
            {
                return 80;
            }
            else if (timing == Timing.tRDRD_sg)
            {
                return 87;
            }
            else if (timing == Timing.tRDWR_sg)
            {
                return 89;
            }
            else if (timing == Timing.tRDWR_dg)
            {
                return 90;
            }
            else if (timing == Timing.tWRRD_sg)
            {
                return 93;
            }
            else if (timing == Timing.tWRRD_dg)
            {
                return 94;
            }
            else if (timing == Timing.tWRWR_sg)
            {
                return 91;
            }
            return -1;
        }
    }
}

using AutoIt;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;

namespace AutoMemOC
{
    class Program
    {
        public static readonly TimeSpan DEFAULT_TIMEOUT = TimeSpan.FromSeconds(10);

        private static JProperties properties = null;
        public static bool verboseLogging = false;

        private static Tweaker tweaker = null;
        private static MemTest memTest = null;

        static void Main(string[] args)
        {
            // Check if the program is running in administrator mode.
            if (!IsAdministrator)
            {
                Console.Error.WriteLine("Please run this program in administrator mode.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            // Load the config
            try
            {
                properties = new JProperties("config.txt");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return;
            }
            Thread.Sleep(50);

            string tweakPath = properties.get("TweakerPath");
            string testPath = properties.get("MemTestPath");
            bool tweakPathOverriden = false; // Only store paths in the config if they are entered manually.
            bool testPathOverriden = false;
            // Fetch paths from command line arguments.
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg.Equals("--tweak"))
                {
                    if (i < arg.Length - 2)
                    {
                        tweakPath = args[i + 1];
                        tweakPathOverriden = true;
                    }
                }
                else if (arg.Equals("--test"))
                {
                    if (i < arg.Length - 2)
                    {
                        testPath = args[i + 1];
                        testPathOverriden = true;
                    }
                }
                else if (arg.Equals("-v") || arg.Equals("--verbose"))
                {
                    verboseLogging = true;
                }
            }

            var stableSettings = StableSettings;
            if (CurrentTiming == null)
            {
                // If the auto memory overclock has completed,
                // print all the stable settings.
                if (stableSettings.Count > 0)
                {
                    Console.WriteLine("Current Stable Settings:");
                    foreach (KeyValuePair<Timing, int> stableSetting in stableSettings)
                    {
                        Console.WriteLine($"{stableSetting.Key}: {stableSetting.Value}");
                    }

                    Console.WriteLine();
                    Console.WriteLine("Press 'R' to reset config and exit. Press any other key to exit...");
                    if (Console.ReadKey(true).Key == ConsoleKey.R)
                    {
                        ResetProgress();
                    }
                    return;
                }
            }

            // Set up tweaker
            if (tweakPath != null && tweakPath.Contains("MemTweakIt"))
            {
                tweaker = new MemTweakIt(tweakPath);
            }
            while (tweaker == null || !tweaker.Start())
            {
                Console.WriteLine("Please enter the directory path for the memory tweaker:");
                tweakPath = Console.In.ReadLine();
                if (tweakPath.Contains("MemTweakIt"))
                {
                    tweaker = new MemTweakIt(tweakPath);
                    tweakPathOverriden = false;
                }
                else
                {
                    Console.WriteLine("Unknown memory tweaker.");
                    tweaker = null;
                }
                Console.WriteLine();
            }
            if (!tweakPathOverriden)
            {
                properties.set("TweakerPath", tweakPath);
                properties.save();
            }

            // Set up memory tester.
            if (testPath != null && (testPath.Contains("TestMem5") || testPath.Contains("TM5")))
            {
                memTest = new TestMem5(testPath);
            }

            Dictionary<Timing, int> initialTimings = new Dictionary<Timing, int>();
            foreach (Timing timing in Timing.All())
            {
                int timingValue = tweaker.GetTiming(timing);
                if (timingValue == -1)
                {
                    FatalError();
                    Console.Error.WriteLine("Failed to fetch current " + timing + "!");

                    tweaker.Close();
                    Console.ReadKey();
                    return;
                }
                initialTimings[timing] = timingValue;
                Thread.Sleep(10);
            }

            if (CurrentTiming == null)
            {
                // First time running.
                Console.WriteLine("Welcome!");
                Console.WriteLine("Please note this program will not tighten all timings including tCL and tRAS.");
                Console.WriteLine("As this is the first time you are running this application, an initial memory test will be run to ensure stock settings are stable.");
                Console.WriteLine();
                Console.WriteLine($"Frequency: {tweaker.Frequency()}");
                Console.WriteLine($"Primary Timings: {tweaker.Primaries()}");
                Console.WriteLine();
                Console.WriteLine("Key binds:");
                Console.WriteLine("Q - Quit testing and exit the program");
                Console.WriteLine("S - Skip a memory test");
                Console.WriteLine("N - Skip a memory test, undo most recent tightening of current timing and move on to the next timing");
                Console.WriteLine("Press 'Q' to exit or any other key to continue...");
                if (Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    tweaker.Close();
                    return;
                }
                Console.WriteLine();

                // Set up initial memory test.
                while (memTest == null || !memTest.Start(false))
                {
                    Console.WriteLine("Please enter the directory path for a memory tester:");
                    testPath = Console.In.ReadLine();
                    if (testPath.Contains("TestMem5") || testPath.Contains("TM5"))
                    {
                        memTest = new TestMem5(testPath);
                        testPathOverriden = false;
                    }
                    else
                    {
                        Console.WriteLine("Unknown memory tester.");
                    }
                    Console.WriteLine();
                }
                if (!testPathOverriden)
                {
                    properties.set("MemTestPath", testPath);
                    properties.save();
                }

                // Run initial memory test.
                while (!memTest.IsFinished())
                {
                    if (!memTest.Running)
                    {
                        FatalError();
                        Console.WriteLine($"{memTest.TestName()} closed!");
                        return;
                    }
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true).Key;
                        if (key == ConsoleKey.Q)
                        {
                            Console.WriteLine("Stop key detected! Closing test...");
                            memTest.Stop();
                            Thread.Sleep(500);
                            memTest.Close();

                            tweaker.Close();
                            return;
                        }
                        else if (key == ConsoleKey.S)
                        {
                            Console.WriteLine("Skip key detected! Skipping test...");
                            memTest.Stop();
                            Thread.Sleep(500);
                            memTest.Close();

                            memTest.Reset();
                            break;
                        }
                    }
                    if (memTest.HasErrors())
                    {
                        Console.WriteLine("Found a memory-related error!");
                        memTest.Stop();
                        memTest.Close();
                        tweaker.Close();

                        Console.WriteLine("Your stock settings are unstable! Please correct them by loosening the error-causing timings before running this program again!");
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey();
                        return;
                    }
                }
                if (memTest.IsFinished())
                {
                    Console.WriteLine("Successfully passed the memory test with stock settings!");
                    memTest.CloseSafely();
                }
                Console.WriteLine();

                // Set up the starting point for tweaking.
                // Currently, this program tweaks tRCD-tRP first.
                Timing initialTiming = Timing.tRCD;
                CurrentTiming = initialTiming;
                CurrentTimingValue = initialTimings[initialTiming];
                Failed = false;
                properties.save();
            }
            else
            {
                // Print out a summary of the RAM speed.
                Console.WriteLine($"Frequency: {tweaker.Frequency()}");
                Console.WriteLine($"Primary Timings: {tweaker.Primaries()}");
                Console.WriteLine();

                if (stableSettings.Count > 0)
                {
                    // Print out the current stable settings.
                    Console.WriteLine("Current Stable Settings:");
                    foreach (KeyValuePair<Timing, int> stableSetting in stableSettings)
                    {
                        Console.WriteLine($"{stableSetting.Key}: {stableSetting.Value}");
                    }

                    Console.WriteLine();
                    Console.WriteLine("Press 'R' to reset current progress and exit, otherwise press any key to continue...");
                    if (Console.ReadKey(true).Key == ConsoleKey.R)
                    {
                        ResetProgress();
                        tweaker.Close();
                        return;
                    }

                    Console.WriteLine();
                }

                while (memTest == null)
                {
                    Console.WriteLine("Please enter the directory path for a memory tester:");
                    testPath = Console.In.ReadLine();
                    if (testPath.Contains("TestMem5") || testPath.Contains("TM5"))
                    {
                        memTest = new TestMem5(testPath);
                    }
                    else
                    {
                        Console.WriteLine("Unknown memory tester.");
                    }
                    Console.WriteLine();
                }
            }

            // Reapply the current stable settings that has been discovered by this program.
            // This step is important as tightening new settings may be affected by other previously tightened timings.
            Console.WriteLine("Reapplying current stable settings...");
            Console.WriteLine();
            foreach (KeyValuePair<Timing, int> stableSetting in stableSettings)
            {
                tweaker.SetTiming(stableSetting.Key, stableSetting.Value);
                Thread.Sleep(10);
            }
            Thread.Sleep(50);
            tweaker.Apply();

            var end = DateTime.Now + TimeSpan.FromSeconds(10);
            while (!tweaker.Ready)
            {
                if (DateTime.Now > end)
                {
                    Console.Error.WriteLine("Failed to apply current stable settings.");
                    tweaker.Close();
                    return;
                }
                Thread.Sleep(100);
            }

            // Start the program loop.
            bool exitLoop = false;
            while (!exitLoop)
            {
                try
                {
                    Timing currentTiming = CurrentTiming;
                    int currentTimingValue = CurrentTimingValue;
                    if (currentTiming == null)
                    {
                        FatalError();
                        Console.Error.WriteLine("Invalid settings; unknown current timing set.");
                        Console.WriteLine();
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey();

                        tweaker.Close();
                        return;
                    }
                    if (!Failed) // If the current timing value did not fail the memory test...
                    {
                        int lowerValue = currentTiming.Lower(currentTimingValue);
                        if (lowerValue != -1) // If there is a tighter value for the current timing...
                        {
                            int biosValue = tweaker.GetTiming(currentTiming); // Get the current value of the timing in the BIOS
                            if (biosValue == -1 || lowerValue < biosValue) // Ensure the tighter value is tighter than the value in the BIOS
                            {
                                SetCurrentTimingValue(lowerValue);
                            }
                            else
                            {
                                if (!TightenNextTiming(stableSettings))
                                {
                                    return;
                                }
                                currentTiming = CurrentTiming;
                            }
                        }
                        else
                        {
                            stableSettings[currentTiming] = currentTimingValue;
                            StableSettings = stableSettings;

                            KeyValuePair<Timing, int> nextTimingPair = FindNextTiming(stableSettings, currentTiming);
                            if (nextTimingPair.Key == null)
                            {
                                return;
                            }

                            UpdateNextTiming(nextTimingPair, stableSettings);
                            currentTiming = CurrentTiming;
                        }
                    }
                    else // If the current timing value failed the memory test...
                    {
                        Console.WriteLine($"Found unstable timing {currentTiming} at value {CurrentTimingValue}.");

                        int higherValue = currentTiming.Higher(currentTimingValue);
                        if (higherValue != -1) // Try finding a looser timing
                        {
                            int biosValue = tweaker.GetTiming(currentTiming);
                            if (biosValue == -1 || higherValue < biosValue) // Do not set the timing looser than the timing in BIOS.
                                                                            // The disadvantage to this is that tightening one timing may
                                                                            // require another timing to be looser and this could prevent that;
                                                                            // however we will ignore such a case and assume the best.
                            {
                                SetCurrentTimingValue(higherValue);
                            }
                            else
                            {
                                if (!TightenNextTiming(stableSettings))
                                {
                                    return;
                                }
                                currentTiming = CurrentTiming;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Failed to find a stable timing for {currentTiming}, skipping...");

                            stableSettings.Remove(currentTiming); // Remove timing from stable settings as the BIOS value is preferred.
                            StableSettings = stableSettings;

                            KeyValuePair<Timing, int> nextTimingPair = FindNextTiming(stableSettings, currentTiming);
                            if (nextTimingPair.Key == null)
                            {
                                return;
                            }

                            UpdateNextTiming(nextTimingPair, stableSettings);
                            currentTiming = CurrentTiming;
                        }
                    }

                    // Start the memory test.
                    if (!memTest.Start(false))
                    {
                        FatalError();
                        Console.Error.WriteLine("Failed to start the memory test.");
                        return;
                    }

                    bool hadErrors = false;
                    int skipMode = -1;
                    // Perform the test.
                    while (!memTest.IsFinished())
                    {
                        if (!memTest.Running)
                        {
                            FatalError();
                            Console.WriteLine($"{memTest.TestName()} closed!");

                            tweaker.Close();
                            return;
                        }
                        if (Console.KeyAvailable)
                        {
                            var pressedKey = Console.ReadKey(true).Key;
                            if (pressedKey == ConsoleKey.Q)
                            {
                                try
                                {
                                    memTest.Stop();
                                }
                                catch (Exception) { }
                                memTest.Close();
                                exitLoop = true;
                                break;
                            }
                            else if (pressedKey == ConsoleKey.S)
                            {
                                Console.WriteLine();
                                Console.WriteLine("Skipped memory test!");
                                skipMode = 1;
                                memTest.Stop();
                                Thread.Sleep(200);
                                memTest.Close();
                                Thread.Sleep(200);
                                break;
                            }
                            else if (pressedKey == ConsoleKey.N)
                            {
                                Console.WriteLine();
                                Console.WriteLine("Skipped memory test, untightening and moving on to next timing...");
                                skipMode = 2;
                                memTest.Stop();
                                Thread.Sleep(200);
                                memTest.Close();
                                Thread.Sleep(200);
                                break;
                            }
                        }
                        if (memTest.HasErrors())
                        {
                            Console.WriteLine("Found a memory-related error!");
                            memTest.Stop();
                            Thread.Sleep(100);
                            memTest.Close();
                            hadErrors = true;
                            break;
                        }
                    }
                    if (exitLoop) break;
                    Thread.Sleep(100);
                    if (!hadErrors)
                    {
                        if (skipMode == 2)
                        {
                            Console.WriteLine();

                            tweaker.SetTiming(currentTiming, stableSettings.ContainsKey(currentTiming) ? stableSettings[currentTiming] : initialTimings[currentTiming]);
                            Thread.Sleep(100);
                            tweaker.Apply();
                            Thread.Sleep(100);

                            KeyValuePair<Timing, int> nextTimingPair = FindNextTiming(stableSettings, currentTiming);
                            if (nextTimingPair.Key == null)
                            {
                                return;
                            }

                            UpdateNextTiming(nextTimingPair, stableSettings);
                            currentTiming = CurrentTiming;
                        } else
                        {
                            if (skipMode == -1)
                            {
                                memTest.CloseSafely();
                            }

                            stableSettings = StableSettings;
                            stableSettings[CurrentTiming] = CurrentTimingValue;
                            StableSettings = stableSettings;
                            Failed = false;
                            properties.save();

                            if (skipMode == -1)
                            {
                                Console.WriteLine($"Successfully passed memory test! New {CurrentTiming}: {CurrentTimingValue}");
                            }
                        }
                    } else
                    {
                        Failed = true;
                    }

                    if (testPath.Contains("TestMem5") || testPath.Contains("TM5"))
                    {
                        memTest = new TestMem5(testPath);
                    }
                    else
                    {
                        memTest.Reset();
                    }

                    Console.WriteLine();
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    Console.ReadKey();
                    break;
                }
            }

            tweaker.Close();
        }

        private static void FatalError()
        {
            Console.Out.WriteLine();
            Console.Out.WriteLine("==========================");
            Console.Out.WriteLine("FATAL ERROR");
            Console.Out.WriteLine("==========================");
        }

        private static void ResetProgress()
        {
            CurrentTiming = null;
            CurrentTimingValue = -1;
            StableSettings = new Dictionary<Timing, int>();
            Failed = false;
            properties.save();
        }

        private static void SetCurrentTimingValue(int newValue)
        {
            CurrentTimingValue = newValue;
            Failed = true;
            properties.save();

            Console.WriteLine($"Set {CurrentTiming} to {CurrentTimingValue}.");
            tweaker.SetTiming(CurrentTiming, CurrentTimingValue);
            Thread.Sleep(100);
            tweaker.Apply();
            Thread.Sleep(100);
        }

        private static bool TightenNextTiming(Dictionary<Timing, int> stableSettings)
        {
            Timing currentTiming = CurrentTiming;
            Console.WriteLine($"Failed to find a tighter timing for {currentTiming}, skipping...");

            KeyValuePair<Timing, int> nextTimingPair = FindNextTiming(stableSettings, currentTiming);
            if (nextTimingPair.Key == null)
            {
                return false;
            }
            UpdateNextTiming(nextTimingPair, stableSettings);
            return true;
        }

        private static void UpdateNextTiming(KeyValuePair<Timing, int> nextTimingPair, Dictionary<Timing, int> stableSettings)
        {
            var currentTiming = nextTimingPair.Key;
            Failed = true;
            CurrentTiming = nextTimingPair.Key;
            CurrentTimingValue = nextTimingPair.Value;
            StableSettings = stableSettings;
            properties.save();

            Console.WriteLine($"Set {currentTiming} to {CurrentTimingValue}.");
            tweaker.SetTiming(currentTiming, CurrentTimingValue);
            Thread.Sleep(100);
            tweaker.Apply();
            Thread.Sleep(100);
        }

        private static KeyValuePair<Timing, int> FindNextTiming(Dictionary<Timing, int> stableSettings, Timing currentTiming)
        {
            Timing nextTiming = currentTiming;
            int nextTimingValue = -1;
            int nextLowerValue;
            while (nextTiming == currentTiming || (nextLowerValue = nextTiming.Lower(nextTimingValue)) == -1)
            {
                nextTiming = GetNextTiming(nextTiming);
                if (nextTiming == null)
                {
                    StableSettings = stableSettings;
                    CurrentTiming = null;
                    properties.save();

                    Console.WriteLine();
                    Console.WriteLine("========================================");
                    Console.WriteLine("Auto memory overclock has completed!");
                    Console.WriteLine("========================================");
                    Console.WriteLine();
                    Console.WriteLine("Stable Settings:");
                    foreach (KeyValuePair<Timing, int> stableSetting in stableSettings)
                    {
                        Console.WriteLine($"{stableSetting.Key}: {stableSetting.Value}");
                    }
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    return new KeyValuePair<Timing, int>(null, -1);
                }

                nextTimingValue = tweaker.GetTiming(nextTiming);
                if (nextTimingValue == -1)
                {
                    Failed = false;
                    properties.save();

                    FatalError();
                    Console.Error.WriteLine("Failed to fetch current " + nextTiming + "!");
                    Console.Error.WriteLine("Cannot proceed...");

                    tweaker.Close();
                    Console.Out.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    return new KeyValuePair<Timing, int>(null, -1);
                }
            }
            return new KeyValuePair<Timing, int>(nextTiming, nextLowerValue);
        }

        private static Timing GetNextTiming(Timing currentTiming)
        {
            if (currentTiming == Timing.tRCD)
            {
                return Timing.tRRD_S;
            }
            else if (currentTiming == Timing.tRRD_S || currentTiming == Timing.tRRD_L || currentTiming == Timing.tFAW)
            {
                return Timing.tRFC;
            }
            else if (currentTiming == Timing.tRFC)
            {
                return Timing.tRDRD_sg;
            }
            else if (currentTiming == Timing.tRDRD_sg)
            {
                return Timing.tWRWR_sg;
            }
            else if (currentTiming == Timing.tWRWR_sg)
            {
                return Timing.tWRRD_sg;
            }
            else if (currentTiming == Timing.tWRRD_sg)
            {
                return Timing.tWRRD_dg;
            }
            else if (currentTiming == Timing.tWRRD_dg)
            {
                return Timing.tRDWR_sg;
            }
            else if (currentTiming == Timing.tRDWR_sg)
            {
                return Timing.tRDWR_dg;
            }
            return null;
        }

        public static Timing CurrentTiming
        {
            get
            {
                if (properties == null) return null;
                string property = properties.get("CurrentTiming");
                return !string.IsNullOrEmpty(property) ? Timing.FromName(property) : null;
            }
            set
            {
                if (properties == null) return;
                properties.set("CurrentTiming", value != null ? value.displayName : null);
            }
        }

        public static int CurrentTimingValue
        {
            get
            {
                if (properties == null) return -1;
                return properties.getInt("CurrentTimingValue");
            }
            set
            {
                if (properties == null) return;
                properties.set("CurrentTimingValue", value);
            }
        }

        public static bool Failed
        {
            get
            {
                if (properties == null) return false;
                return properties.getBool("Failed");
            }
            set
            {
                if (properties == null) return;
                properties.set("Failed", value);
            }
        }

        public static Dictionary<Timing, int> StableSettings
        {
            get
            {
                if (properties == null) return new Dictionary<Timing, int>();
                StringCollection configStableSettings = properties.getStringCollection("StableSettings");
                if (configStableSettings == null)
                {
                    configStableSettings = new StringCollection();
                }
                Dictionary<Timing, int> stableSettings = new Dictionary<Timing, int>(configStableSettings.Count / 2);
                for (int i = 0; i < configStableSettings.Count; i += 2)
                {
                    Timing timing = Timing.FromName(configStableSettings[i]);
                    if (timing == null) continue;
                    int value;
                    if (!int.TryParse(configStableSettings[i + 1], out value))
                    {
                        continue;
                    }
                    stableSettings.Add(timing, value);
                }
                return stableSettings;
            }
            set
            {
                if (properties == null) return;
                if (value != null && value.Count > 0)
                {
                    StringCollection stableSettings = new StringCollection();
                    foreach (KeyValuePair<Timing, int> stableSetting in value)
                    {
                        stableSettings.Add(stableSetting.Key.displayName);
                        stableSettings.Add($"{stableSetting.Value}");
                    }
                    properties.set("StableSettings", stableSettings);
                }
                else
                {
                    properties.set("StableSettings", null);
                }
            }
        }

        public static bool IsAdministrator => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }


    public abstract class ProcessBased
    {
        protected string exePath = null;
        protected Process process = null;

        public TimeSpan Timeout { get; set; } = Program.DEFAULT_TIMEOUT;

        public bool Started { get; protected set; } = false;

        public bool Running
        {
            get
            {
                return process != null && !process.HasExited;
            }
            private set { }
        }

        public int PID
        {
            get { return process != null ? process.Id : 0; }
        }

        public ProcessBased(string exeDirectory)
        {
            exePath = exeDirectory;
        }

        public abstract bool Start();

        public abstract void Close();

        public void Reset()
        {
            if (process != null)
            {
                if (!process.HasExited) process.Kill();
                process = null;
            }
            Started = false;
        }

        protected string GetValue(string clazz, int instance)
        {
            if (!Running) return null;
            IntPtr hwnd = GetHandle(clazz, instance);
            if (hwnd == IntPtr.Zero) return "";
            return AutoItX.ControlGetText(process.MainWindowHandle, hwnd);
        }

        protected bool SetValue(string clazz, int instance, string value)
        {
            if (!Running) return false;
            IntPtr hwnd = GetHandle(clazz, instance);
            if (hwnd == IntPtr.Zero) return false;
            return AutoItX.ControlSetText(process.MainWindowHandle, hwnd, value) == 1;
        }

        protected IntPtr GetHandle(string clazz, int instance)
        {
            if (!Running) return IntPtr.Zero;
            return AutoItX.ControlGetHandle(process.MainWindowHandle, $"[CLASS:{clazz}; INSTANCE:{instance}]");
        }
    }
}

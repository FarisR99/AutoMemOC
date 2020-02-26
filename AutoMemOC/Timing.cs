using System.Collections.Generic;

namespace AutoMemOC
{
    public class Timing
    {
        private static List<Timing> timings = new List<Timing>();

        public static Timing tCL = CreateTiming("tCL");
        public static Timing tRCD = CreateTiming("tRCD", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28 });
        public static Timing tRP = CreateTiming("tRP");
        public static Timing tRAS = CreateTiming("tRAS");

        public static Timing tRRD_S = CreateTiming("tRRD_S", new int[] { 4, 5, 6 });
        public static Timing tRRD_L = CreateTiming("tRRD_L", new int[] { 4, 5, 6 });
        public static Timing tFAW = CreateTiming("tFAW");
        public static Timing tRFC = CreateTiming("tRFC", new int[] { 200, 220, 240, 260, 280, 300, 320, 340, 360, 380, 400, 420, 440, 460, 480, 600, 620, 640, 660, 680, 700, 720, 740, 760, 780, 800 });

        public static Timing tRDRD_sg = CreateTiming("tRDRD_sg", new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });

        public static Timing tRDWR_sg = CreateTiming("tRDWR_sg", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
        public static Timing tRDWR_dg = CreateTiming("tRDWR_dg", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });

        public static Timing tWRRD_sg = CreateTiming("tWRRD_sg", new int[] { 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35 });
        public static Timing tWRRD_dg = CreateTiming("tWRRD_dg", new int[] { 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35 });

        public static Timing tWRWR_sg = CreateTiming("tWRWR_sg", new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });

        private static Timing CreateTiming(string displayName)
        {
            if (timings == null) timings = new List<Timing>();

            Timing timing = new Timing(displayName);
            timings.Add(timing);
            return timing;
        }

        private static Timing CreateTiming(string displayName, int[] values)
        {
            if (timings == null) timings = new List<Timing>();

            Timing timing = new Timing(displayName, values);
            timings.Add(timing);
            return timing;
        }

        public string displayName;
        public int[] potentialValues;

        public Timing(string dispName) : this(dispName, new int[0])
        {
        }

        public Timing(string dispName, int[] values)
        {
            this.displayName = dispName;
            this.potentialValues = values;
        }

        public int Lower(int currentValue)
        {
            for (int i = this.potentialValues.Length - 1; i >= 0; --i)
            {
                if (this.potentialValues[i] < currentValue)
                {
                    return this.potentialValues[i];
                }
            }
            return -1;
        }

        public int Higher(int currentValue)
        {
            for (int i = 0; i < this.potentialValues.Length; ++i)
            {
                if (this.potentialValues[i] > currentValue)
                {
                    return this.potentialValues[i];
                }
            }
            return -1;
        }

        public override string ToString()
        {
            return displayName;
        }

        public static Timing FromName(string name)
        {
            if (name == null) return null;
            foreach (Timing timing in timings)
            {
                if (name.Equals(timing.displayName))
                {
                    return timing;
                }
            }
            return null;
        }
    }
}

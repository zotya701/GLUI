using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GLUI.Application
{
    public static class PerformanceInfo
    {
        private static PerformanceCounter mCpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private static PerformanceCounter mRamCounter = mRamCounter = new PerformanceCounter("Memory", "Available Bytes");

        public static float TotalRAM { get { return Helper.GetTotalMemory(); } }
        public static float AvailableRAM { get { return mRamCounter.NextValue() / 1024.0f / 1024.0f; } }
        public static float TotalUsedRAM { get { return TotalRAM - AvailableRAM; } }
        public static float UsedRam { get { return Environment.WorkingSet / 1024.0f / 1024.0f; } }
        public static float CPUUtilization { get { return mCpuCounter.NextValue(); } }

        private static class Helper
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct PerformanceInformation
            {
                public int Size;
                public IntPtr CommitTotal;
                public IntPtr CommitLimit;
                public IntPtr CommitPeak;
                public IntPtr PhysicalTotal;
                public IntPtr PhysicalAvailable;
                public IntPtr SystemCache;
                public IntPtr KernelTotal;
                public IntPtr KernelPaged;
                public IntPtr KernelNonPaged;
                public IntPtr PageSize;
                public int HandlesCount;
                public int ProcessCount;
                public int ThreadCount;
            }

            [DllImport("psapi.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

            public static float GetTotalMemory()
            {
                var wInfo = new PerformanceInformation();
                if (GetPerformanceInfo(out wInfo, Marshal.SizeOf(wInfo)))
                {
                    return wInfo.PhysicalTotal.ToInt64() * wInfo.PageSize.ToInt64() / 1024.0f / 1024.0f;
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}

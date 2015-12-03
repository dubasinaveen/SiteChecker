using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace IISHealthCheckApi.Helpers
{
    public static class MiniDump
    {
        [DllImport("DbgHelp.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern Boolean MiniDumpWriteDump(
                                    IntPtr hProcess,
                                    Int32 processId,
                                    IntPtr fileHandle,
                                    MiniDumpType dumpType,
                                    ref MinidumpExceptionInfo excepInfo,
                                    IntPtr userInfo,
                                    IntPtr extInfo);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern int GetCurrentThreadId();


        struct MinidumpExceptionInfo
        {
            public Int32 ThreadId;
            public IntPtr ExceptionPointers;
            public Boolean ClientPointers;
        }

        public static Boolean TryDump(Process process, String dmpPath, MiniDumpType dmpType)
        {

            if (File.Exists(dmpPath))
                File.Delete(dmpPath);
            using (FileStream stream = new FileStream(dmpPath, FileMode.Create))
            {
                //Process process = Process.GetProcessById(PID);
                MinidumpExceptionInfo mei = new MinidumpExceptionInfo();
                ProcessThreadCollection pt = process.Threads;
                //foreach (ProcessThread th in pt)
                //{
                //    if (th.ThreadState.Equals(System.Diagnostics.ThreadState.Wait))
                //    {
                //        mei.ThreadId = th.Id;
                //    }
                //}

                mei.ExceptionPointers = Marshal.GetExceptionPointers();
                mei.ClientPointers = true;

                Boolean res = MiniDumpWriteDump(
                                    process.Handle,
                                    process.Id,
                                    stream.SafeFileHandle.DangerousGetHandle(),
                                    dmpType,
                                    ref mei,
                                    IntPtr.Zero,
                                    IntPtr.Zero);

                stream.Flush();
                stream.Close();

                return res;
            }
        }
    }

    public enum MiniDumpType
    {
        None = 0x00010000,
        Normal = 0x00000000,
        WithDataSegs = 0x00000001,
        WithFullMemory = 0x00000002,
        WithHandleData = 0x00000004,
        FilterMemory = 0x00000008,
        ScanMemory = 0x00000010,
        WithUnloadedModules = 0x00000020,
        WithIndirectlyReferencedMemory = 0x00000040,
        FilterModulePaths = 0x00000080,
        WithProcessThreadData = 0x00000100,
        WithPrivateReadWriteMemory = 0x00000200,
        WithoutOptionalData = 0x00000400,
        WithFullMemoryInfo = 0x00000800,
        WithThreadInfo = 0x00001000,
        WithCodeSegs = 0x00002000
    }

}

﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Runtime.InteropServices;
using System.IO;
using LibreLancer;
using LibreLancer.Dialogs;

namespace SystemViewer
{
    class MainClass
    {
        [DllImport("kernel32.dll")]
        static extern bool SetDllDirectory(string directory);

        [STAThread]
        public static void Main(string[] args)
        {
            if (Platform.RunningOS == OS.Windows)
            {
                string bindir = Path.GetDirectoryName(typeof(MainClass).Assembly.Location);
                var fullpath = Path.Combine(bindir, IntPtr.Size == 8 ? "x64" : "x86");
                SetDllDirectory(fullpath);
            }
            if (!Platform.CheckDependencies()) return;
#if !DEBUG
            try {
#endif
            new MainWindow().Run();
#if !DEBUG
            }
            catch (Exception ex)
            {
                CrashWindow.Run("Uh-oh!", "Librelancer has crashed. See the log for more information.", ex.Message + "\n" + ex.StackTrace);
            }
#endif
        }
    }
}
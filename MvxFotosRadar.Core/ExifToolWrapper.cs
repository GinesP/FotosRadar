﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace BBCSharp
{
    public class ExifToolWrapper : IDisposable
    {
        private readonly string exe;
        private const string ExeName = "exiftool(-k).exe";
        private const string Arguments = "-fast -m -q -q -stay_open True -@ - -common_args -d \"%Y.%m.%d %H:%M:%S\" -t";   //-g for groups

        public enum Statuses { Stopped, Starting, Ready, Stopping };
        public Statuses Status { get; private set; }

        private int cmdCnt = 1;
        private readonly StringBuilder output = new StringBuilder();
        private readonly ProcessStartInfo psi;
        private Process proc = null;
        private readonly ManualResetEvent waitHandle = new ManualResetEvent(true);

        public string ExiftoolVersion { get; private set; }

        public ExifToolWrapper(string path = null)
        {
            exe = string.IsNullOrEmpty(path) ? Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), ExeName) : path;
            if (!File.Exists(exe))
                throw new FileNotFoundException(ExeName + " not found");

            psi = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = Arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            };

            Status = Statuses.Stopped;
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
                return;

            if (Status == Statuses.Starting)
            {
                ExiftoolVersion = e.Data;
                waitHandle.Set();
            }
            else
            {
                if (e.Data.ToLower() == string.Format("{{ready{0}}}", cmdCnt))
                    waitHandle.Set();
                else
                    output.AppendLine(e.Data);
            }
        }

        public void Start()
        {
            if (Status != Statuses.Stopped)
                throw new InvalidOperationException("Process is not stopped");

            Status = Statuses.Starting;

            proc = new Process { StartInfo = psi, EnableRaisingEvents = true };
            proc.OutputDataReceived += OutputDataReceived;
            proc.Exited += proc_Exited;
            proc.Start();

            proc.BeginOutputReadLine();

            waitHandle.Reset();
            proc.StandardInput.WriteLine("-ver\n-execute0000");
            waitHandle.WaitOne();

            Status = Statuses.Ready;
        }

        //detect if process is killed
        void proc_Exited(object sender, EventArgs e)
        {
            if (proc != null)
            {
                proc.Dispose();
                proc = null;
            }

            Status = Statuses.Stopped;

            waitHandle.Set();
        }

        public void Stop()
        {
            try
            {
                if (Status != Statuses.Ready)
                    throw new InvalidOperationException("Process must be ready");

                Status = Statuses.Stopping;

                waitHandle.Reset();
                proc.StandardInput.WriteLine("-stay_open\nFalse\n");
                if (!waitHandle.WaitOne(5000))
                {
                    if (proc != null)
                    {
                        //silently swallow an eventual exception
                        try
                        {
                            proc.Kill();
                            proc.WaitForExit(2000);
                            proc.Dispose();
                        }
                        catch (Exception ex) {
                            Console.WriteLine(ex.Message);
                        }

                        proc = null;
                    }

                    Status = Statuses.Stopped;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string SendCommand(string cmd)
        {
            if (Status != Statuses.Ready)
                throw new InvalidOperationException("Process must be ready");

            waitHandle.Reset();
            proc.StandardInput.WriteLine("{0}\n-execute{1}", cmd, cmdCnt);
            waitHandle.WaitOne();

            cmdCnt++;

            string r = output.ToString();
            output.Clear();

            return r;
        }

        public Dictionary<string, string> FetchExifFrom(string path)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();

            string sRes = SendCommand(path);
            foreach (string s in sRes.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] kv = s.Split('\t');
                if (kv.Length == 2)
                    res[kv[0]] = kv[1];

                //Debug.Assert(kv.Length == 2, "Can not parse line :'" + s + "'");
            }

            return res;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Debug.Assert(Status == Statuses.Ready || Status == Statuses.Stopped, "Invalid state");

            if (proc != null && Status == Statuses.Ready)
                Stop();

            waitHandle.Dispose();
        }

        #endregion
    }
}

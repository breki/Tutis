using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using LibroLib;
using NUnit.Framework;

namespace SrtmPlaying.Tests
{
    public static class PngValidator
    {
        public static void ValidatePng(string outputFileName)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(
                Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\tools\pngcheck\pngcheck.exe"),
                "-vt {0}".Fmt(outputFileName))
            {
                CreateNoWindow = true,
                ErrorDialog = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            StringBuilder outputText = new StringBuilder();
            using (Process process = Process.Start(processStartInfo))
            {
                // ReSharper disable once PossibleNullReferenceException
                process.ErrorDataReceived += (o, e) => { outputText.Append(e.Data); };
                process.OutputDataReceived += (o, e) => { outputText.Append(e.Data); };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.Start();
                process.WaitForExit(20*1000);

                Assert.AreEqual(0, process.ExitCode, outputText.ToString());

                Console.Write(outputText);
            }
        }
    }
}
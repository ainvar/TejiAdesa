using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Controls;
using System.Net.NetworkInformation;
using System.Windows.Media;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace TejiAdesa.MainLibs
{
    public delegate void ExecScript(Statement statement, ProgressBar bar, String ServerName = "", String serverUser = "", String serverPass = "");
    public delegate void ExecScriptWithLog(Statement statement, String tailPars, ProgressBar bar, TextBox txtBox, String ServerName = "", String serverUser = "", String serverPass = "");
    public delegate void StartProcess(Statement statement, String tailPars, TextBox txtBlock, ProgressBar bar);
    public delegate void ExecScriptWithLog2(Statement statement, ProgressBar bar, TextBlock txtBlock, String ServerName = "", String serverUser = "", String serverPass = "");
    public delegate void WriteFSLog(String txtToLog, String logPath);

    public static class Behaviours
    {
        public static ExecScript ExecScript()
        {
            return (Statement statement, ProgressBar bar, String ServerName, String serverUser, String serverPass) =>
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += (o, workerE) =>
                {
                    worker.ReportProgress(50);
                    try
                    {
                        SSHCommander.RunCommand(ServerName, serverUser, serverPass, statement.ScriptToLaunch);
                    }
                    catch (Exception ex)
                    {

                    }
                    worker.ReportProgress(50);
                };

                worker.ProgressChanged += (o, e) =>
                {
                    bar.Value += e.ProgressPercentage;
                };

                worker.RunWorkerCompleted += (o, e) =>
                {
                    bar.Value = 0;
                    bar.Visibility = System.Windows.Visibility.Hidden;
                };

                worker.RunWorkerAsync();
            };
        }

        public static ExecScriptWithLog2 ExecScriptWithLog2()
        {
            return (Statement statement, ProgressBar bar, TextBlock txtBlock, String ServerName, String serverUser, String serverPass) =>
            {
                bar.Value = 0;
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += (o, workerE) =>
                {
                    String feedback = "";
                    worker.ReportProgress(50);
                    try
                    {
                        feedback = SSHCommander.RunCommandWithFeedback(ServerName, serverUser, serverPass, statement.ScriptToLaunch);
                        worker.ReportProgress(10);
                        workerE.Result = feedback;
                        worker.ReportProgress(10);
                    }
                    catch (Exception ex)
                    {

                    }
                    worker.ReportProgress(30);
                };

                worker.ProgressChanged += (o, e) =>
                {
                    bar.Value += e.ProgressPercentage;
                };

                worker.RunWorkerCompleted += (o, e) =>
                {
                    txtBlock.Text = e.Result.ToSafeString();
                    Task.Factory.StartNew(() => {
                        WriteFSLog()(e.Result.ToSafeString(), Path.Combine(Environment.CurrentDirectory,statement.ScriptToLaunch +  DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Millisecond.ToString() + ".log"));
                    });
                };

                worker.RunWorkerAsync();
            };
        }

        public static ExecScriptWithLog ExecScriptWithLog()
        {
            return (Statement statement, String tailPars, ProgressBar bar, TextBox txtBox, String ServerName, String serverUser, String serverPass) =>
            {
                bar.Value = 0;
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += (o, workerE) =>
                {
                    String feedback = "";
                    worker.ReportProgress(50);
                    try
                    {
                        feedback = SSHCommander.RunCommandWithFeedback(ServerName, serverUser, serverPass, statement.ScriptToLaunch + tailPars.ToSafeString());
                        worker.ReportProgress(10);
                        workerE.Result = feedback;
                        worker.ReportProgress(10);
                    }
                    catch (Exception ex)
                    {

                    }
                    worker.ReportProgress(30);
                };

                worker.ProgressChanged += (o, e) =>
                {
                    bar.Value += e.ProgressPercentage;
                };

                worker.RunWorkerCompleted += (o, e) =>
                {
                    txtBox.Text = e.Result.ToSafeString();
                    Task.Factory.StartNew(() =>
                    {
                        WriteFSLog()(e.Result.ToSafeString(), Path.Combine(Environment.CurrentDirectory, statement.ScriptToLaunch + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Millisecond.ToString() + ".log"));
                    });
                };

                worker.RunWorkerAsync();
            };
        }

        public static StartProcess StartProcess()
        {
            return (Statement statement, String tailPars, TextBox txtBox, ProgressBar bar) =>
            {
                Action cancel = () =>
                            {
                                txtBox.Text ="";
                            };

                txtBox.Dispatcher.Invoke(cancel);

                if (statement.ScriptToLaunch.IndexOf(".exe") == -1 && statement.ScriptToLaunch.IndexOf(".jar") == -1)
                {
                    ProcessStartInfo procStartInfo =
                        new System.Diagnostics.ProcessStartInfo("cmd", "/c " + statement.ScriptToLaunch + " " + tailPars);

                    procStartInfo.RedirectStandardOutput = true;
                    procStartInfo.UseShellExecute = false;
                    procStartInfo.CreateNoWindow = true;
                    Process proc = new Process();
                    proc.StartInfo = procStartInfo;
                    proc.Start();
                    string result = proc.StandardOutput.ReadToEnd();
                    txtBox.Text = result;
                }
                else
                {
                    // se è un exe lo lancio
                    Process process = new Process();

                    var res = new StringBuilder();
                    var resError = new StringBuilder();

                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = statement.ScriptToLaunch;
                    process.StartInfo.Arguments = statement.Parameters + " " + tailPars;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    try
                    {
                        if(process.Start())
                        {
                            process.OutputDataReceived += new DataReceivedEventHandler
                            (
                                delegate(object sender, DataReceivedEventArgs e)
                                {
                                    Action pr = () =>
                                    {
                                        txtBox.Text += e.Data + "\n";
                                    };

                                    txtBox.Dispatcher.Invoke(pr);
                                    res.AppendLine(e.Data);
                                }
                            );

                            process.ErrorDataReceived += new DataReceivedEventHandler
                            (
                                (ob, ev) =>
                                {
                                    resError.AppendLine("Rilevato un errore: " + ev.Data);    
                                }
                            );

                            process.Exited += new EventHandler
                            (
                                (ob, ev) =>
                                {
                                    txtBox.Text += "Processo terminato";

                                    Task.Factory.StartNew(() =>
                                    {
                                        WriteFSLog()(res.ToString() + " \n\nErrori:\n " + resError.ToString(), Path.Combine(Environment.CurrentDirectory, (statement.ScriptToLaunch + statement.Parameters).Replace(" ", "_").Replace(".", "_") + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Millisecond.ToString() + ".log"));
                                    });
                                }
                            );

                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();
                        }
                    }
                    catch (Exception ex)
                    {
                        txtBox.Text = ex.Message;
                    }
                }
            };
        }

        public static WriteFSLog WriteFSLog()
        {
            return (String txtToLog, String logPath) =>
            {
                File.WriteAllText(logPath, txtToLog);
            };
        }

        //public static void TrapStreamFromConsole()
        //{
        //    Process _cmd;

        //delegate void SetTextCallback(string text);

        //private void SetText(string text)
        //{
        //    if (this.richTextBox1.InvokeRequired)
        //    {
        //        SetTextCallback d = new SetTextCallback(SetText);
        //        this.Invoke(d, new object[] { text });
        //    }
        //    else
        //    {
        //        this.richTextBox1.Text += text + Environment.NewLine;
        //    }
        //}

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    ProcessStartInfo cmdStartInfo = new ProcessStartInfo("tracert.exe");
        //    cmdStartInfo.Arguments = "google.com";
        //    cmdStartInfo.CreateNoWindow = true;
        //    cmdStartInfo.RedirectStandardInput = true;
        //    cmdStartInfo.RedirectStandardOutput = true;
        //    cmdStartInfo.RedirectStandardError = true;
        //    cmdStartInfo.UseShellExecute = false;
        //    cmdStartInfo.WindowStyle = ProcessWindowStyle.Hidden;

        //    _cmd = new Process();
        //    _cmd.StartInfo = cmdStartInfo;

        //    if (_cmd.Start())
        //    {
        //        _cmd.OutputDataReceived += new DataReceivedEventHandler(_cmd_OutputDataReceived);
        //        _cmd.ErrorDataReceived += new DataReceivedEventHandler(_cmd_ErrorDataReceived);
        //        _cmd.Exited += new EventHandler(_cmd_Exited);

        //        _cmd.BeginOutputReadLine();
        //        _cmd.BeginErrorReadLine();
        //    }
        //    else
        //    {
        //        _cmd = null;
        //    }
        //}

        //void _cmd_OutputDataReceived(object sender, DataReceivedEventArgs e)
        //{
        //    UpdateConsole(e.Data);
        //}

        //void _cmd_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        //{
        //    UpdateConsole(e.Data, Brushes.Red);
        //}

        //void _cmd_Exited(object sender, EventArgs e)
        //{
        //    _cmd.OutputDataReceived -= new DataReceivedEventHandler(_cmd_OutputDataReceived);
        //    _cmd.Exited -= new EventHandler(_cmd_Exited);
        //}

        //private void UpdateConsole(string text)
        //{
        //    UpdateConsole(text, null);
        //}
        //private void UpdateConsole(string text, Brush color)
        //{
        //    WriteLine(text, color);
        //}

        //private void WriteLine(string text, Brush color)
        //{
        //    if (text != null)
        //    {    
        //        SetText(text);
        //    }
        //}
        //}
    }
}

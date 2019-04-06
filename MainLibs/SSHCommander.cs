using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.SshNet;

namespace TejiAdesa.MainLibs
{
    class SSHCommander
    {
       /* public static void RunCommand(String host, String user, String passwd, String singleCommand)
        {
            try
            {
                SshExec sshExec = new SshExec(host, user, passwd);
                sshExec.Connect();
                Console.Write(sshExec.RunCommand(singleCommand));
                Console.WriteLine("disconnessione in corso");
                Console.Write(sshExec.RunCommand("logout"));
                sshExec.Close();
                Console.WriteLine("disconnesso");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
            }
        }*/

        public static void RunCommand(String host, String user, String passwd, String singleCommand)
        {
            try
            {
                SshClient client = new SshClient(host, 22, user, passwd);
                client.Connect();
                Console.Write(client.RunCommand(singleCommand));
                Console.WriteLine("disconnessione in corso");
                Console.Write(client.RunCommand("logout"));
                client.Disconnect();
                Console.WriteLine("disconnesso");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
            }
        }

        public static String RunCommandWithFeedback(String host, String user, String passwd, String singleCommand)
        {
            StringBuilder feedback = new StringBuilder();
            try
            {
                feedback.AppendLine("Inizia esecuzione script:");
                PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo(host, 22, user, passwd);
                connectionInfo.Timeout = TimeSpan.FromMilliseconds(40000);
                SshClient client = new SshClient(connectionInfo);
                client.Connect();
                SshCommand command = client.RunCommand(singleCommand);
                command.CommandTimeout = new TimeSpan(2000000000);
                feedback.AppendLine(command.Execute());
                feedback.AppendLine("disconnessione in corso");
                feedback.AppendLine(command.Execute("logout"));
                client.Disconnect();
                client.Dispose();

                feedback.AppendLine("disconnesso");
                
            }
            catch (Exception e)
            {
                feedback.AppendLine(Util.GetFormattedExceptionInfo(ref e));
            }
            return feedback.ToString();
        }
    }
}

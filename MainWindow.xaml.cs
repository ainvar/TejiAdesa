using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TejiAdesa.MainLibs;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace TejiAdesa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if(File.Exists("config.json"))
            {
                File.Decrypt("config.json");
                StartLight light = new StartLight(new List<Statement> { });
                light = JsonConvert.DeserializeObject<StartLight>(File.ReadAllText("config.json"));
                //File.Encrypt("config.json");
                BuildButtons(light);
            }
        }

        private void btnGenTemplate_Click(object sender, RoutedEventArgs e)
        {
            StartLight light = new StartLight(new List<Statement> { 
                new Statement
                    { 
                        Identity = "0",
                        Description="descrizione primo comando",
                        Label="label primo comando",
                        ScriptToLaunch="path completo dello script o primo comando",
                        ServerName = "clio",
                        ServerUser = "root",
                        ServerPassword = "password"
                    },
                new Statement
                    { 
                        Identity = "1",
                        Description="descrizione secondo comando",
                        Label="label secondo comando",
                        ScriptToLaunch="path completo dello script o secondo comando",
                        ServerName = "clio",
                        ServerUser = "root",
                        ServerPassword = "password"
                    } ,
                new Statement
                    { 
                        Identity = "2",
                        Description="descrizione terzo comando",
                        Label="label terzo comando",
                        ScriptToLaunch="path completo dello script o terzo comando",
                        ServerName = "clio",
                        ServerUser = "root",
                        ServerPassword = "password"
                    } 
            });

            light.ServerName = "server Name";
            light.ServerPassword = "ssh user password";
            light.ServerUser = "server ssh user";

            String JsonTemplate = JsonConvert.SerializeObject(light);

            var myWindow = new ShowJsonTemplate();
            myWindow.txtArea.Text = JsonTemplate;
            myWindow.Show();
            /*WebClient client = new WebClient();
            Stream stream = client.OpenRead("http://openlibrary.org/api/get?key=/b/OL1001932M");
            StreamReader reader = new StreamReader(stream);

            List<dynamic> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(reader.Read().ToString());*/
        }

        private void btnOpenShell_Click(object sender, RoutedEventArgs e)
        {
            var myShell = new WinShell();
            myShell.Show();
        }

        private void BuildButtons(StartLight light)
        {
            foreach (Statement statement in light.ScriptsList)
            {
                Button tempBtn = new Button();
                Button tempBtn2 = new Button();
                tempBtn.Content = statement.Label;
                tempBtn.Name = "exec_" + statement.Identity;
                tempBtn.ToolTip = statement.Description;
                switch(statement.OperationType.ToLowerInvariant())
                {
                    case "ssh":
                        if (statement.ServerName.ToSafeString() != "")
                            tempBtn.Click += new RoutedEventHandler((e, o) => 
                                Behaviours.ExecScriptWithLog()(
                                light.ScriptsList[Convert.ToInt16(tempBtn.Name.Split('_')[1])], 
                                txtTailPars.Text, 
                                execBar, 
                                txtBox, 
                                light.ScriptsList[Convert.ToInt16(tempBtn.Name.Split('_')[1])].ServerName, 
                                light.ScriptsList[Convert.ToInt16(tempBtn.Name.Split('_')[1])].ServerUser, 
                                light.ScriptsList[Convert.ToInt16(tempBtn.Name.Split('_')[1])].ServerPassword));
                        else
                            tempBtn.Click += new RoutedEventHandler((e, o) => 
                                Behaviours.ExecScriptWithLog()(
                                light.ScriptsList[Convert.ToInt16(tempBtn.Name.Split('_')[1])], 
                                txtTailPars.Text, 
                                execBar, 
                                txtBox, 
                                light.ServerName, 
                                light.ServerUser, 
                                light.ServerPassword));
                        break;

                    case "localosstatement":
                        tempBtn.Click += new RoutedEventHandler((e, o) =>
                                Behaviours.StartProcess()(statement, txtTailPars.Text, txtBox, this.execBar)); 
                        break;

                    default:
                        break;
                }

                btnMain.Children.Add(tempBtn);

                tempBtn2.Content = "script";
                tempBtn2.Name = "script_" + statement.Identity;
                tempBtn2.Click += new RoutedEventHandler(
                    (e, o) => MessageBox.Show(light.ScriptsList[Convert.ToInt16(tempBtn.Name.Split('_')[1])].ScriptToLaunch + " " + light.ScriptsList[Convert.ToInt16(tempBtn.Name.Split('_')[1])].Parameters + txtTailPars.Text)

                );
                btnMainScripts.Children.Add(tempBtn2);

            }
        }

        private void imgReload_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace MinecraftServerEditor
{
    public partial class frmMain : Form
    {
        DirectoryInfo DataDir;
        DirectoryInfo ServerDir;
        FileInfo DataFile;

        public frmMain()
        {
            InitializeComponent();
            InitializeVars();
            Startup();
        }

        private void InitializeVars()
        {
            DataDir = new DirectoryInfo(Environment.CurrentDirectory + "\\Data");
            ServerDir = new DirectoryInfo("C:\\Minecraft Servers");
            DataFile = new FileInfo(DataDir.FullName + "\\Data.txt");
        }

        private void Startup()
        {
            CheckDirs();
            CheckFiles();
            LoadFiles();
            UpdateServerList();
        }

        private void LoadFiles()
        {
            StreamReader sr = new StreamReader(DataFile.FullName);
            List<string> fc = new List<string>();
            while (sr.Peek() != -1)
            {
                fc.Add(sr.ReadLine());
            }
            sr.Close();
            foreach (string s in fc)
            {
                if (s.Contains("server_directory:"))
                {
                    ServerDir = new DirectoryInfo(s.Replace("server_directory:", "").Trim());
                }
            }


        }

        private void CheckFiles()
        {
            if (!DataFile.Exists)
            {
                FirstTimeSetup();
                SaveData();
            }
        }

        private void FirstTimeSetup()
        {
            MessageBox.Show("It appears that this is the first time the program has run. We need some basic information " +
                "before you can start using the application.", "Minecraft Server Editor Setup");
            MessageBox.Show("Please select a location for the servers to be stored./r/nThe default location is " + ServerDir.FullName, "Minecraft Server Editor Setup");
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.SelectedPath = ServerDir.FullName;
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ServerDir = new DirectoryInfo(fb.SelectedPath);
            }
        }

        private void SaveData()
        {
            StreamWriter sw = new StreamWriter(DataFile.FullName);
            sw.WriteLine("**Minecraft Server Editor**");
            sw.WriteLine("version:              0.0.0.1");
            sw.WriteLine("server_directory:     " + ServerDir.FullName);

            sw.Close();
        }

        private void CheckDirs()
        {
            if (!DataDir.Exists)
            {
                DataDir.Create();
            }
            if (!ServerDir.Exists)
            {
                ServerDir.Create();
            }
        }

        private void webServerPage_FileDownload(object sender, EventArgs e)
        {

        }

        private void webServerPage_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.Segments[e.Url.Segments.Length - 1].EndsWith(".jar"))
            {
                e.Cancel = true;
                //MessageBox.Show("Cancel");
                string filepath = null;
                string name = e.Url.Segments[e.Url.Segments.Length - 1];
                if (name.Contains("minecraft_server"))
                {
                    DirectoryInfo DI = new DirectoryInfo(ServerDir.FullName + "\\" + name.Replace("minecraft_server.", "").Replace(".jar", ""));
                    if (!DI.Exists)
                    {
                        DI.Create();
                    }
                    filepath = DI.FullName + "\\" + name;

                    WebClient client = new WebClient();
                    
                    client.DownloadProgressChanged += client_DownloadProgressChanged;

                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                    client.DownloadFileAsync(e.Url, filepath);
                    lblStatus.Text = "Downloading " + e.Url.Segments[e.Url.Segments.Length - 1];
                    pbarDownload.Visible = true;
                }
                else
                {
                    MessageBox.Show("Only Minecraft Server Executables (.jar) may be downloaded using this program!", "WRONG FILE");
                }
            }
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            pbarDownload.Value = e.ProgressPercentage;
            lblPercentage.Text = e.ProgressPercentage + "%";
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            lblStatus.Text = "File downloaded";
            lblPercentage.Text = "";
            pbarDownload.Value = 0;
            pbarDownload.Visible = false;
            UpdateServerList();
        }

        private void UpdateServerList()
        {
            TreeNode root = treeServers.Nodes[0];
            root.ToolTipText = ServerDir.FullName;
            foreach (DirectoryInfo di in ServerDir.GetDirectories())
            {
                bool canAdd = true;
                foreach (TreeNode t in root.Nodes)
                {
                    if(t.Text == di.Name)
                    {
                        canAdd = false;
                        break;
                    }
                }
                if (canAdd)
                {
                    TreeNode tr = new TreeNode(di.Name);
                    tr.Tag = "Open Dir";
                    root.Nodes.Add(tr);
                }
            }
        }

        private void treeServers_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode t = e.Node;
            string s = "";
            if (t.Tag == null || t.Tag.GetType() != typeof(string))
            {
                return;
            }
            s = (string)t.Tag;
            switch (s)
            {
                case "Open Dir":
                    try
                    {
                        Process.Start(ServerDir.FullName + "\\" + t.Text + "\\");
                    }
                    catch
                    {
                        MessageBox.Show("Directory " + ServerDir.FullName + "\\" + t.Text + "\\ was not found. The application will now remove the server from the listing.", "DIRECTORY NOT FOUND ERROR");
                        TreeNode tn = t.Parent;
                        tn.Nodes.Remove(t);
                    }
                    break;
            }
        }

        private void treeServers_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }
    }
}

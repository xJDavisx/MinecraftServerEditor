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

        }

        private void LoadFiles()
        {
            StreamReader sr = new StreamReader(DataFile.FullName);
            string s = sr.ReadToEnd();
            sr.Close();


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
    }
}

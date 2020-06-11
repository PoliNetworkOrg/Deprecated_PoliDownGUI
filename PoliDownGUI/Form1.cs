using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoliDownGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string appdata_path;
        public string polidown_js_path;
        public string output_folder_path;
        public int? codice_persona;
        public string password;
        public List<string> videos;

        public const string const_polidown_js_path = "polidown_js.txt";
        public const string const_codice_persona_path = "codicepersona.txt";
        public const string const_password_path = "password.txt";
        public const string const_output_folder_path = "output_folder.txt";

        private void Form1_Load(object sender, EventArgs e)
        {
            appdata_path = SetAppdataPath();
            LoadValues();
        }


        private void LoadValues()
        {
            polidown_js_path = LoadValue(const_polidown_js_path);
            codice_persona = TryGetInt(LoadValue(const_codice_persona_path));
            password = LoadValue(const_password_path);
            output_folder_path = LoadValue(const_output_folder_path);

            if (!string.IsNullOrEmpty(polidown_js_path))
            {
                label4.Text = polidown_js_path;
            }

            if (codice_persona != null)
            {
                textBox_codice_persona.Text = codice_persona.Value.ToString();
            }

            if (!string.IsNullOrEmpty(password))
            {
                textBox_password.Text = password;
            }
            
        }

        private int? TryGetInt(string v)
        {
            if (string.IsNullOrEmpty(v))
                return null;


            try
            {
                return Convert.ToInt32(v);
            }
            catch
            {
                return null;
            }
        }

        private string LoadValue(string v)
        {
            try
            {
                return File.ReadAllText(this.appdata_path + v);
            }
            catch
            {
                return null;
            }
        }

        private string SetAppdataPath()
        {
            string s = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            s += "/PoliDownGUI/";

            try
            {
                Directory.CreateDirectory(s);
            }
            catch
            {
                ;
            }

            return s;
        }

        private void Button_download_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty( textBox_codice_persona.Text))
            {
                MessageBox.Show("Devi inserire un codice persona!");
                return;
            }

            if (string.IsNullOrEmpty(textBox_password.Text))
            {
                MessageBox.Show("Devi inserire la password!");
                return;
            }

            codice_persona = TryGetInt( textBox_codice_persona.Text);

            if (codice_persona == null)
            {
                MessageBox.Show("Devi inserire un codice persona valido!");
                return;
            }

            password = textBox_password.Text;

            if (string.IsNullOrEmpty(textBox_link.Text))
            {
                MessageBox.Show("Devi inserire dei video!");
                return;
            }

            if (string.IsNullOrEmpty(output_folder_path))
            {
                MessageBox.Show("Devi selezionare una cartella di destinazione!");
                return;
            }

            videos = GetList(textBox_link.Lines);

            string strCmdText = "/K node "; 
               
            strCmdText+= polidown_js_path +" -u ";
            strCmdText += codice_persona.Value.ToString();
            strCmdText += " ";
            strCmdText += "-p";
            strCmdText += " ";
            strCmdText += password;
            strCmdText += " ";
            strCmdText += "-o";
            strCmdText += " ";
            strCmdText += output_folder_path;
            strCmdText += " ";
            strCmdText += "-v";
            strCmdText += " ";
            foreach (var video in videos)
            {
                strCmdText += "\"";
                strCmdText += video;
                strCmdText += "\"";
                strCmdText += " ";
            }

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = strCmdText;
            cmd.Start();
            cmd.WaitForExit();
        }

        private List<string> GetList(string[] lines)
        {
            return (lines).ToList();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var r = openFileDialog.ShowDialog();
            if (r == DialogResult.OK)
            {
                string f = openFileDialog.FileName;
                if (f.EndsWith(".js"))
                {
                    polidown_js_path = f;
                    label4.Text = polidown_js_path;
                    return;
                }

                MessageBox.Show("Devi selezionare il file polidown.js corretto!");
            }


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Save(TryGetInt(textBox_codice_persona.Text), const_codice_persona_path);
            Save(textBox_password.Text, const_password_path);
            Save(label4.Text, const_polidown_js_path);
            Save(label5.Text, const_output_folder_path);
        }

        private void Save(object v, string path)
        {
            string s = TryGetString(v);
            if (string.IsNullOrEmpty(s))
                return;

            File.WriteAllText(this.appdata_path + path, s);
        }

        private string TryGetString(object v)
        {
            try
            {
                return v.ToString();
            }
            catch
            {
                return null;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string s = System.Reflection.Assembly.GetEntryAssembly().Location;
                s = s.Replace('\\', '/');
                var s2 = s.Split('/');
                string s3 = "";
                for (int i=0; i<s2.Length-1; i++)
                {
                    s3 += s2[i] + "/";
                }

                s3 += "videos";

                try
                {
                    Directory.CreateDirectory(s3);
                }
                catch
                {
                    ;
                }

                Process.Start(s3);
            }
            catch
            {
                ;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            var r = folderBrowserDialog.ShowDialog();
            if (r == DialogResult.OK)
            {
                string f = folderBrowserDialog.SelectedPath;
                if (!string.IsNullOrEmpty(f))
                {
                    output_folder_path = f;
                    label5.Text = output_folder_path;
                    return;
                }

                MessageBox.Show("Devi selezionare una cartella di destinazione valida!");
            }
        }
    }
}

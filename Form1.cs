using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ScannerSonar
{
    public partial class Form1 : Form
    {
        private Process cmdProcess;
        public Form1()
        {
            InitializeComponent();
        }
        private void AppendTextToRichTextBox(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendTextToRichTextBox(text)));
            }
            else
            {
                richTextBox0.AppendText(text);
            }
        }
        private void SendCommandToCmd(string command)
        {
            try
            {
                if (cmdProcess != null && !cmdProcess.HasExited)
                {
                    cmdProcess.StandardInput.WriteLine(command);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ProcessStartInfo cmdStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            cmdProcess = new Process
            {
                StartInfo = cmdStartInfo,
                EnableRaisingEvents = true
            };
            cmdProcess.OutputDataReceived += (s, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    AppendTextToRichTextBox(args.Data + Environment.NewLine);
                }
            };
            cmdProcess.Start();
            cmdProcess.BeginOutputReadLine();

            
            string startupFolder = textBox0.Text;
            string command = $@"cd ""{startupFolder}""";
            SendCommandToCmd(command);
        }
        public void button2_Click(object sender, EventArgs e)
        {
            string command = $@"SonarScanner.MSBuild.exe begin /k:""{textBox1.Text}"" /d:sonar.host.url=""{textBox2.Text}"" /d:sonar.login=""{textBox3.Text}""";
            SendCommandToCmd(command);
        }

        public void button3_Click(object sender, EventArgs e)
        {
            string startupFolder = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "MsBuild", "MsBuild.exe");
            string msBuildPath = startupFolder;
            string targetDirectory = textBox0.Text;
            string command = $@"""{msBuildPath}"" /t:Rebuild /p:WorkingDirectory=""{targetDirectory}""";
            SendCommandToCmd(command);

        }

        public void button4_Click(object sender, EventArgs e)
        {
            string command = $@"SonarScanner.MSBuild.exe end /d:sonar.login=""{textBox3.Text}""";
            SendCommandToCmd(command);
        }
    }
}

using System;
using System.Windows.Forms;
//using Word = Microsoft.Office.Tools.Word;
using System.Threading;
using System.IO;

namespace AozoraIntoWord
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            btnRun.Enabled = false;
            progressBar1.Style = ProgressBarStyle.Marquee;

            ThreadStart threadStart = new ThreadStart(delegate()
                {
                    try
                    {
                        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "template.dot");
                        AozoraDocument aozora = new AozoraDocument(tbFilePath.Text, templatePath);
                        aozora.GenerateWordDocument();
                    }
                    catch (Exception ex)
                    {
                        NotifyErrorMessage(ex);
                    }
                    finally
                    {
                        GenerationFinished(this, null);
                    }
                });

            Thread thread = new Thread(threadStart);

            thread.Start();
        }
        
        delegate void NotifyErrorDelegate(Exception ex);

        private void NotifyErrorMessage(Exception ex)
        {
            if (InvokeRequired)
            {
                Invoke(new NotifyErrorDelegate(NotifyErrorMessage), ex);
                return;
            }
            MessageBox.Show(Control.FromHandle(this.Handle), ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void GenerationFinished(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(GenerationFinished));
                return;
            }
            progressBar1.Style = ProgressBarStyle.Blocks;
            btnRun.Enabled = true;
        }


        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                 Multiselect = false,
                 Filter = "HTML ファイル|*.html|すべてのファイル|*.*",
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbFilePath.Text = dialog.FileName;
            }

        }



        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop, false);
                tbFilePath.Text = files[0];
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                tbFilePath.Text = (string) e.Data.GetData(DataFormats.Text);
            }      
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
    }
}

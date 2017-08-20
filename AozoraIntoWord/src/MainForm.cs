using System;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;

namespace AozoraIntoWord
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Icon = global::AozoraIntoWord.Properties.Resources.ProgramIcon;
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            btnRun.Enabled = false;

            ProgressDialog progressDialog;

            using (BackgroundWorker bw = new BackgroundWorker())
            {
                progressDialog = new ProgressDialog(bw);
                progressDialog.Text = "変換";
                progressDialog.Message = "変換処理中...";

                bw.WorkerReportsProgress = true;
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += new DoWorkEventHandler(DoProcessBkgnd);
                bw.RunWorkerAsync();
            }

            ProgressDialogResult result = progressDialog.ShowDialog();

            if (result == ProgressDialogResult.Success)
            {
                MessageBox.Show("変換に成功しました", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (result == ProgressDialogResult.Failure)
            {
                MessageBox.Show(progressDialog.Error.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (result == ProgressDialogResult.Canceled)
            {
                MessageBox.Show("ユーザーにより処理が中断されました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            btnRun.Enabled = true;
            btnRun.Focus();
        }

        void DoProcessBkgnd(object sender, DoWorkEventArgs eventArgs)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            try
            {
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "template.dot");
                AozoraDocument aozora = new AozoraDocument(tbFilePath.Text, templatePath);
                aozora.GenerateWordDocument(bw);
            }
            catch (OperationCanceledException)
            {
                eventArgs.Cancel = true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = "HTML ファイル|*.html|すべてのファイル|*.*",
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                tbFilePath.Text = dialog.FileName;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                tbFilePath.Text = files[0];
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                tbFilePath.Text = (string)e.Data.GetData(DataFormats.Text);
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

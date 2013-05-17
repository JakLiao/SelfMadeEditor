using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Text;

namespace SelfMadeEditor
{
    public partial class MainForm : Form
    {
        #region 定义变量
        private bool isChanged; // 文档是否被修改
        private bool isSaved;	// 文档是否被保存过
        private bool isFontChanged; //下拉框使字体改变
        private Encoding TextEncoding = Encoding.GetEncoding("GB2312");
        private SearchAndReplaceForm searchForm;
        private string fileName;// 文档的文件名
        public string FileName
        {
            get
            {
                if (this.fileName == null)
                {
                    return "无标题";
                }
                return this.fileName;
            }
            set
            {
                this.fileName = value;
            }
        }
        #endregion

        public System.Windows.Forms.RichTextBox txtMain;

        public MainForm()
        {
            this.InitializeComponent();
            this.isSaved = false;
            this.saveStatus.Text = "未保存";
            this.Text = "无标题 ---- Editor"; 
            this.isChanged = false;
            this.isFontChanged = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //获得系统字体列表
            InstalledFontCollection fonts = new InstalledFontCollection();
            foreach (FontFamily family in fonts.Families)
            {
                fontFamilyToolStripComboBox.Items.Add(family.Name);
            }
        }

        private bool AlertSaveFile() // 提示用户保存文件
        {
            if (isChanged) // 表示文档有改动并且未保存
            {
                DialogResult result =
                    MessageBox.Show(this, "存在文件尚未保存，要保存改动吗？", "文本编辑器", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                // 保存文档
                if (result == DialogResult.Yes)
                {
                    if (this.isSaved)
                    {
                        this.txtMain.SaveFile(FileName);
                    }
                    else
                    {
                        另存为toolStripMenuItem_Click(null, null);
                    }
                }// 不保存文档
                else if (result == DialogResult.No)
                {
                }// 取消
                else
                {
                    return false; // 表示点击取消
                }
            }
            return true; // 表示点击其他
        }

        // 用于打开文件和改变文本编码
        private void OpenFile(string fileName, System.Text.Encoding encoding, bool SetEncoding)
        {
            FileInfo finfo = new FileInfo(fileName);
            if (encoding == null) // 如果不需要设置编码，则设置编码为GB2312
            {
                this.TextEncoding = Encoding.GetEncoding("GB2312");
            }
            else // 否则设置为指定的编码
            {
                this.TextEncoding = encoding;
            }
            if (SetEncoding) // 如果需要设置编码并且编码不为空
            {

                if (encoding != null)
                {
                    FileStream fs = finfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);// 打开文件并将内容读入控件
                    StreamReader sr = new StreamReader(fs, encoding);
                    this.txtMain.Text = sr.ReadToEnd();
                    sr.Close();
                    fs.Close();
                    return;
                }
            }
            // rtf文件
            if (finfo.Extension == ".rtf")
            {
                this.txtMain.LoadFile(finfo.FullName, RichTextBoxStreamType.RichText);
            }// 文本文件
            else if (finfo.Extension == ".txt")
            {
                this.txtMain.LoadFile(finfo.FullName, RichTextBoxStreamType.PlainText);
            }// Unicode Text
            else if (finfo.Extension == ".uni")
            {
                this.txtMain.LoadFile(finfo.FullName, RichTextBoxStreamType.UnicodePlainText);
            }// 其他文件
            else
            {
            }
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog(this);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AlertSaveFile(); // 提示用户保存文件
            this.txtMain.Clear();
            this.isSaved = false;
            this.saveStatus.Text = "未保存";
            this.Text = "无标题 ---- Editor"; 
            this.isChanged = false;
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.isSaved)
            {
                this.txtMain.SaveFile(FileName);
            }
            else
            {
                另存为toolStripMenuItem_Click(null, null);
            }
            this.saveStatus.Text = "已保存";
        }

        private void 另存为toolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog2.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileInfo finfo = new FileInfo(this.saveFileDialog2.FileName);
                    if (finfo.Extension == ".rtf")
                    {
                        this.txtMain.SaveFile(finfo.FullName, RichTextBoxStreamType.RichText);
                    }
                    else
                    {
                        this.txtMain.SaveFile(finfo.FullName, RichTextBoxStreamType.PlainText);
                    }
                    this.FileName = this.saveFileDialog2.FileName;

                    this.isSaved = true;
                    this.saveStatus.Text = "已保存于" + DateTime.Now.ToShortTimeString();
                    this.isChanged = false;
                    this.Text = this.FileName;
                }
                catch (ArgumentException ex)
                {
                    
                } 
            }
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AlertSaveFile(); // 提示用户保存文件
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                OpenFile(this.openFileDialog1.FileName, null, false);
                this.FileName = this.openFileDialog1.FileName;
                this.isSaved = true;
                this.saveStatus.Text = "已保存";
                this.isChanged = false;
                this.Text = this.openFileDialog1.FileName;
                this.statusBarSaveTime.Text = "文件打开于" + DateTime.Now.ToShortTimeString();
            }
        }

        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.txtMain.Copy();
        }

        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.txtMain.Cut();
        }

        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.txtMain.Paste();
        }

        private void 全选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.txtMain.SelectAll();
        }

        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.txtMain.Text = "";
        }

        private void 查找ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 替换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.searchForm = new SearchAndReplaceForm(this);
            this.searchForm.Show();
            this.AddOwnedForm(this.searchForm);
        }

        private void 字体ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.fontDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtMain.SelectionFont = this.fontDialog1.Font;
            }
            this.isChanged = true;
        }

        private void 颜色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.colorDialog1.Color = this.txtMain.SelectionColor;
            if (this.colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                this.txtMain.SelectionColor = this.colorDialog1.Color;
            }
            this.isChanged = true;
        }

        private void txtMain_TextChanged(object sender, EventArgs e)
        {
            this.isChanged = true;
            this.saveStatus.Text = "未保存";
        }

        private void 撤销toolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.txtMain.CanUndo)
            {
                this.txtMain.Undo();
            }
        }

        private void 恢复toolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.txtMain.CanRedo)
            {
                this.txtMain.Redo();
            }
        }

        private void cursor()
        {
            int ln =this.txtMain.GetLineFromCharIndex(this.txtMain.SelectionStart) + 1;
            int col = (this.txtMain.SelectionStart -this.txtMain.GetFirstCharIndexOfCurrentLine()) + 1;
            this.positionStatus.Text = string.Concat(new object[] { "Ln:", ln, " Col:", col });
            if (this.txtMain.SelectionFont != null)
            {
                this.fontFamilyToolStripComboBox.Text = this.txtMain.SelectionFont.FontFamily.Name;
                this.fontSizeToolStripComboBox.Text = this.txtMain.SelectionFont.Size.ToString();
                this.boldButton.Checked = this.txtMain.SelectionFont.Bold;
                this.italicButton.Checked = this.txtMain.SelectionFont.Italic;
                this.underlineButton.Checked = this.txtMain.SelectionFont.Underline;
            }
            else
            {
                this.fontFamilyToolStripComboBox.Text = "";
                this.fontSizeToolStripComboBox.Text = "";
            }
            if (this.txtMain.SelectionAlignment == HorizontalAlignment.Left)
            {
                this.alignLeftButton.Checked = true;
                this.alignCenterButton.Checked = false;
                this.alignRightButton.Checked = false;
            }
            if (this.txtMain.SelectionAlignment == HorizontalAlignment.Center)
            {
                this.alignLeftButton.Checked = false;
                this.alignCenterButton.Checked = true;
                this.alignRightButton.Checked = false;
            }
            if (this.txtMain.SelectionAlignment == HorizontalAlignment.Right)
            {
                this.alignLeftButton.Checked = false;
                this.alignCenterButton.Checked = false;
                this.alignRightButton.Checked = true;
            }
            Color selectionColor = this.txtMain.SelectionColor;
            this.colorButton.ForeColor = this.txtMain.SelectionColor;
        }

        private void alignLeftButton_Click(object sender, EventArgs e)
        {
            this.txtMain.SelectionAlignment = HorizontalAlignment.Left;
            this.alignLeftButton.Checked = true;
            this.alignCenterButton.Checked = false;
            this.alignRightButton.Checked = false;
        }

        private void alignCenterButton_Click(object sender, EventArgs e)
        {
            this.txtMain.SelectionAlignment = HorizontalAlignment.Center;
            this.alignLeftButton.Checked = false;
            this.alignCenterButton.Checked = true;
            this.alignRightButton.Checked = false;
        }

        private void alignRightButton_Click(object sender, EventArgs e)
        {
            this.txtMain.SelectionAlignment = HorizontalAlignment.Right;
            this.alignLeftButton.Checked = false;
            this.alignCenterButton.Checked = false;
            this.alignRightButton.Checked = true;
        }

        private void boldButton_Click(object sender, EventArgs e)
        {
            if (this.txtMain.SelectionFont != null)
            {
                this.txtMain.SelectionFont = new System.Drawing.Font(this.txtMain.SelectionFont.FontFamily.Name, this.txtMain.SelectionFont.Size, this.txtMain.SelectionFont.Style ^ FontStyle.Bold);
                this.boldButton.Checked = this.txtMain.SelectionFont.Bold;
            }
            else
            {
                int selectionStart = this.txtMain.SelectionStart;
                int lastEdit = selectionStart + this.txtMain.SelectionLength;
                bool flag = true;
                for (int i = selectionStart; i < lastEdit; i++)
                {
                    this.txtMain.Select(i, 1);
                    this.txtMain.SelectionFont = new System.Drawing.Font(this.txtMain.SelectionFont.FontFamily.Name, this.txtMain.SelectionFont.Size, this.txtMain.SelectionFont.Style ^ FontStyle.Bold);
                    if (!this.txtMain.SelectionFont.Bold)
                    {
                        flag = false;
                    }
                }
                this.boldButton.Checked = flag;
                this.txtMain.Select(selectionStart, lastEdit - selectionStart);
            }
            this.isChanged = true;
        }

        private void italcButton_Click(object sender, EventArgs e)
        {
            if (this.txtMain.SelectionFont != null)
            {
                this.txtMain.SelectionFont = new System.Drawing.Font(this.txtMain.SelectionFont.FontFamily.Name, this.txtMain.SelectionFont.Size, this.txtMain.SelectionFont.Style ^ FontStyle.Italic);
                this.italicButton.Checked = this.txtMain.SelectionFont.Italic;
            }
            else
            {
                int selectionStart = this.txtMain.SelectionStart;
                int lastEdit = selectionStart + this.txtMain.SelectionLength;
                bool flag = true;
                for (int i = selectionStart; i < lastEdit; i++)
                {
                    this.txtMain.Select(i, 1);
                    this.txtMain.SelectionFont = new System.Drawing.Font(this.txtMain.SelectionFont.FontFamily.Name, this.txtMain.SelectionFont.Size, this.txtMain.SelectionFont.Style ^ FontStyle.Italic);
                    if (!this.txtMain.SelectionFont.Italic)
                    {
                        flag = false;
                    }
                }
                this.italicButton.Checked = flag;
                this.txtMain.Select(selectionStart, lastEdit - selectionStart);
            }
            this.isChanged = true;
        }

        private void underlineButton_Click(object sender, EventArgs e)
        {
            if (this.txtMain.SelectionFont != null)
            {
                this.txtMain.SelectionFont = new System.Drawing.Font(this.txtMain.SelectionFont.FontFamily.Name, this.txtMain.SelectionFont.Size, this.txtMain.SelectionFont.Style ^ FontStyle.Underline);
                this.underlineButton.Checked = this.txtMain.SelectionFont.Underline;
            }
            else
            {
                int selectionStart = this.txtMain.SelectionStart;
                int lastEdit = selectionStart + this.txtMain.SelectionLength;
                bool flag = true;
                for (int i = selectionStart; i < lastEdit; i++)
                {
                    this.txtMain.Select(i, 1);
                    this.txtMain.SelectionFont = new System.Drawing.Font(this.txtMain.SelectionFont.FontFamily.Name, this.txtMain.SelectionFont.Size, this.txtMain.SelectionFont.Style ^ FontStyle.Underline);
                    if (!this.txtMain.SelectionFont.Underline)
                    {
                        flag = false;
                    }
                }
                this.underlineButton.Checked = flag;
                this.txtMain.Select(selectionStart, lastEdit - selectionStart);
            }
            this.isChanged = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isChanged)
                AlertSaveFile();
        }

        private void 编辑器背景色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.colorDialog1.Color = this.txtMain.SelectionColor;
            if (this.colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                this.txtMain.BackColor = this.colorDialog1.Color;
            }
        }

        private void txtMain_SelectionChanged(object sender, EventArgs e)
        {
            this.cursor();
        }

        private void toolStripComboBox1_DropDownClosed(object sender, EventArgs e)
        {
            this.isFontChanged = true;
        }

        private void toolStripComboBox2_DropDownClosed(object sender, EventArgs e)
        {
            this.isFontChanged = true;
        }

        private void Font_TextChanged(object sender, EventArgs e)
        {
            if (isFontChanged)
            {
                if ((this.fontFamilyToolStripComboBox.Text != null) && (this.fontFamilyToolStripComboBox.Text != ""))
                {
                    this.txtMain.SelectionFont = new System.Drawing.Font(this.fontFamilyToolStripComboBox.Text, float.Parse(this.fontSizeToolStripComboBox.Text), this.txtMain.SelectionFont.Style);
                }
                else
                {
                    string text = this.fontFamilyToolStripComboBox.Text;
                    int selectionStart = this.txtMain.SelectionStart;
                    int lastEdit = selectionStart + this.txtMain.SelectionLength;
                    for (int i = selectionStart; i < lastEdit; i++)
                    {
                        this.txtMain.Select(i, 1);
                        this.txtMain.SelectionFont = new System.Drawing.Font(text, this.txtMain.SelectionFont.Size, this.txtMain.SelectionFont.Style);
                    }
                    this.txtMain.Select(selectionStart, lastEdit - selectionStart);
                }
                isFontChanged = false;
            }
            this.isChanged = true;
        }

        private void 字数统计ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int totalNum = 0;
            string[] strArray = this.txtMain.Text.Split(" \t\n\0".ToCharArray());
            foreach (string str in strArray)
            {
                totalNum += str.Length;
            }
            MessageBox.Show(this, string.Concat(new object[] { "当前编辑文件:\n", this.fileName, "\n字符数(计空格)            ", this.txtMain.TextLength, "\n字符数(不计空格)        ", totalNum, "\n" }), "字数统计", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
    }
}

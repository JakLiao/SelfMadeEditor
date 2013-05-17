using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SelfMadeEditor
{
    public partial class SearchAndReplaceForm : Form
    {
        private MainForm parentForm;
        private string strSearch = ""; // 表示要查找的字符串
        private string strReplace = "";// 表示要替换成的字符串
        private int searchPos = 0, lastSearchPos = 0;// 前者表示当前的查找位置，后者表示上次的查找位置
        public SearchAndReplaceForm(MainForm parent)
        {
            InitializeComponent();
            this.parentForm = parent;
        }

         //函数完成文本的查找功能，参数表示是否在未找到指定文本时显示消息，返回值为是否找到指定文本
        private bool SearchText(bool isShowFound)
        {
            bool isFound = true;
            if (this.caseCheckBox.Checked)// 表示大小写匹配查找
            {
                if (this.downRadioButton.Checked) // 表示向下查找
                {
                    this.searchPos = this.parentForm.txtMain.Find(this.strSearch, searchPos,this.parentForm.txtMain.Text.Length, RichTextBoxFinds.MatchCase);
                }
                else
                {
                    this.searchPos = this.parentForm.txtMain.Find(this.strSearch, 0,searchPos, RichTextBoxFinds.MatchCase | RichTextBoxFinds.Reverse);
                }
            }
            else
            {
                if (this.downRadioButton.Checked)
                {
                    this.searchPos = this.parentForm.txtMain.Find(this.strSearch, searchPos,this.parentForm.txtMain.Text.Length, RichTextBoxFinds.None);
                }
                else
                {
                    this.searchPos = this.parentForm.txtMain.Find(this.strSearch, 0,searchPos, RichTextBoxFinds.Reverse);
                }
            }
            if (this.searchPos < 0)//如果未找到，则显示信息，将上次查找位置复原，置标志位为未找到
            {
                if (isShowFound)
                {
                    MessageBox.Show("已完成对文档的搜索，未找到搜索项", "Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.searchPos = this.lastSearchPos;
                isFound = false;
            }
            else
            {
                if (this.downRadioButton.Checked)
                {
                    this.searchPos += this.strSearch.Length;
                }
                else
                {
                    this.searchPos -= this.strSearch.Length;
                }
                this.parentForm.Focus();// 使主窗体获得焦点
            }
            this.lastSearchPos = this.searchPos;
            return isFound;
        }

        private void replaceButton_Click(object sender, EventArgs e)
        {
            this.strSearch = this.searchText.Text;
            this.strReplace = this.replaceText.Text;
            SearchText(true);
            if (this.parentForm.txtMain.SelectedText.Length > 0)
            {
                this.parentForm.txtMain.SelectedText = this.strReplace;
            }
        }

        private void replaceAllButton_Click(object sender, EventArgs e)
        {
            this.strSearch = this.searchText.Text;
            this.strReplace = this.replaceText.Text;
            while (SearchText(false))
            {
                if (this.parentForm.txtMain.SelectedText.Length > 0)
                {
                    this.parentForm.txtMain.SelectedText = this.strReplace;
                }
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            this.strSearch = this.searchText.Text;
            SearchText(true);
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void searchText_TextChanged(object sender, EventArgs e)
        {
            if (this.searchText.Text.Length == 0)
                this.SearchButton.Enabled = false;
            else
                this.SearchButton.Enabled = true;
        }

        private void replaceText_TextChanged(object sender, EventArgs e)
        {
            if (this.replaceText.Text.Length == 0)
            {
                this.replaceButton.Enabled = false;
                this.replaceAllButton.Enabled = false;
            }
            else
            {
                this.replaceButton.Enabled = true;
                this.replaceAllButton.Enabled = true;
            }
        }
    }
}

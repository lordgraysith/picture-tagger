using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PictureTagger
{
    public partial class Form1 : Form
    {
        private TaggingDataStore _dataStore = null;
        private string _currentLocation = null;

        public Form1()
        {
            InitializeComponent();
            textBox1.KeyUp += textBox1_KeyUp;
            this.KeyUp += Form1_KeyUp;
        }

        void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                nextButton_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Left)
            {
                previousButton_Click(sender, e);
            }
            else if (e.Alt)
            {
                if (e.KeyCode == Keys.V)
                {
                    AddTag("Velvet");
                }
                if (e.KeyCode == Keys.M)
                {
                    AddTag("Michael");
                }
                if (e.KeyCode == Keys.J)
                {
                    AddTag("Jackson");
                }
                if (e.KeyCode == Keys.K)
                {
                    AddTag("Karli");
                }
            }
        }

        void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            string text = textBox1.Text;
            if (e.KeyCode == Keys.Enter &&
                !String.IsNullOrWhiteSpace(text))
            {
                AddTag(text);
            }
            if (e.KeyCode == Keys.Enter)
            {
                textBox1.Text = String.Empty;
            }
        }

        private void AddTag(string text)
        {
            if (_dataStore != null &&
                !_dataStore.ContainsTag(text))
            {
                _dataStore.AddTag(text);
                tagListBox.Items.Add(text);
            }
            tagListBox.SelectedItems.Add(text);
            SaveTagsToCurrentImage();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            _dataStore.WriteToFile(_currentLocation);
            this.Close();
        }

        private void folderSelectButton_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                _currentLocation = folderBrowserDialog1.SelectedPath;
                _dataStore = TaggingDataStore.LoadFromFile(_currentLocation);
                LoadTags();
                if (_dataStore.ImageCount > 0)
                {
                    LoadFile(_dataStore.CurrentImage);
                    previousButton.Enabled = true;
                    nextButton.Enabled = true;
                }
                else
                {
                    previousButton.Enabled = false;
                    nextButton.Enabled = false;
                }
            }
        }

        private void LoadTags()
        {
            tagListBox.Items.Clear();
            foreach (string tag in _dataStore.Tags)
            {
                tagListBox.Items.Add(tag);
            }
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            LoadFile(_dataStore.Previous);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            LoadFile(_dataStore.Next);
        }

        private void tagListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveTagsToCurrentImage();
        }

        private void SaveTagsToCurrentImage()
        {
            IList<string> selected = new List<string>();
            foreach (var item in tagListBox.SelectedItems)
            {
                selected.Add(item as string);
            }
            _dataStore.CurrentImage.Tags = selected;
            _dataStore.WriteToFile(_currentLocation);
        }

        private void LoadFile(ImageFile file)
        {
            pictureBox1.Load(file.FullPath);

            tagListBox.ClearSelected();
            List<object> selectedItems = new List<object>();
            foreach (var item in tagListBox.Items)
            {
                string tag = item as string;
                if (file.Tags.Contains(tag))
                {
                    selectedItems.Add(item);
                }
            }
            foreach (var item in selectedItems)
            {
                tagListBox.SelectedItems.Add(item);
            }
        }
    }
}

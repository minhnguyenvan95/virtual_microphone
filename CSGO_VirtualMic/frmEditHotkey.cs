using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace CSGO_VirtualMic
{
    public partial class frmEditHotkey : Form
    {
        string audioPath = Form1.appPath + "\\audio";
        List<string> list_hotkey = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "NumPad0", "NumPad1", "NumPad2", "NumPad3", "NumPad4", "NumPad5", "NumPad6", "NumPad7", "NumPad8", "NumPad9", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "Up", "Down", "Left", "Right", "Tab", "LWin" }; 
        private int current_edit_id = -1;
        private String current_audio_file = "";
        public frmEditHotkey()
        {
            InitializeComponent();
            InitHotkeyComboBox();
            ShowListView();
        }

        private void InitHotkeyComboBox() 
        {           
            cbKey.Items.AddRange(list_hotkey.ToArray());
            cbKey.SelectedIndex = 0;
        }

        private void btnSelectAudio_Click(object sender, EventArgs e)
        {
            btnPlay.Enabled = false;
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Audio File (*.mp3;*.wav)|*.mp3;*.wav;";
            if (open.ShowDialog() != DialogResult.OK)
                return;
            String test_sound = CSGO_VirtualMic.Audio.playSound(0, open.FileName);
            if (test_sound == "true")
            {
                string open_file_path = System.IO.Path.GetDirectoryName(open.FileName);
                string file_name = open.SafeFileName;

                if (!open_file_path.ToUpper().Contains(audioPath.ToUpper())) //audio file is not in appPath
                {
                    if (!System.IO.File.Exists(audioPath + "\\" + file_name))
                    {
                       
                        System.IO.File.Copy(open.FileName, audioPath + "\\" + file_name);
                    }
                    else 
                    {
                        System.IO.File.Copy(open.FileName, audioPath + "\\" + "copy_" + file_name);
                    }
                }
                current_audio_file = @".\audio\" + file_name;
                CSGO_VirtualMic.Audio.disposeWave();
                btnPlay.Enabled = true;
            }
            else
            {
                CSGO_VirtualMic.Form1.ShowMessage(test_sound);
                btnSelectAudio_Click(sender, e);
            }
        }

        public void ShowListView() 
        {
            listView1.Items.Clear();
            ArrayList arr = Model.LoadData();

            for(int i=0;i<arr.Count;i++)
            {
                HotkeyItem a = (HotkeyItem)arr[i];
                listView1.Items.Add(i.ToString());
                int j = listView1.Items.Count - 1;
                listView1.Items[j].SubItems.Add(a.getHotkey());
                listView1.Items[j].SubItems.Add(a.Description);
                listView1.Items[j].SubItems.Add(a.AudioFile.Substring(a.AudioFile.LastIndexOf('\\') + 1));
            }
        }

        private HotkeyItem getHotkeyItemInForm() 
        {
            HotkeyItem a = new HotkeyItem();
            a.Ctr = chkCtr.Checked;
            a.Alt = chkAlt.Checked;
            a.Shift = chkShift.Checked;
            a.Key = cbKey.Text;
            a.Description = txtDescription.Text;
            a.AudioFile = current_audio_file;
            return a;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            HotkeyItem a = getHotkeyItemInForm();

            String validate_hotkey = ValidateHotkeyItem(a);

            if (validate_hotkey != "true")
                Form1.ShowMessage(validate_hotkey);
            else if (Model.Add(a))
            {
                ShowListView();
                btnEdit.Enabled = false;
                btnRemove.Enabled = false;
            }
            else
                Form1.ShowMessage("Hotkey " + a.getHotkey() + " is already exist");
        }

        private String ValidateHotkeyItem(HotkeyItem a) 
        {
            List<string> restrict_key = new List<string>() { "Q", "W", "E", "R", "Y", "U", "A", "S", "D", "G", "1", "2", "3", "4", "5" };
            if (!a.Ctr && !a.Alt && !a.Shift)
            {
                if (restrict_key.Contains(a.Key))
                    return "You can not only this key.";
            }

            if (!list_hotkey.Contains(a.Key))
                return "Hotkey is not in HotkeyList.";

            if (a.Description == "")
                return "Please fill Description field.";

            String test_sound = Audio.playSound(Form1.AudioDevice,a.AudioFile);
            if (test_sound == "true")
                Audio.disposeWave();

            return test_sound;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                btnEdit.Enabled = false;
                current_edit_id = -1;
                return;
            }

            btnEdit.Enabled = true;
            btnRemove.Enabled = true;
            int item = listView1.SelectedItems[0].Index;
            current_edit_id = item;
            ArrayList arr = Model.LoadData();
            HotkeyItem a = (HotkeyItem)arr[item];
            chkCtr.Checked = a.Ctr;
            chkAlt.Checked = a.Alt;
            chkShift.Checked = a.Shift;
            cbKey.Text = a.Key;
            txtDescription.Text = a.Description;
            current_audio_file = a.AudioFile;

            if (Audio.playSound(0, a.AudioFile) == "true")
            {
                Audio.disposeWave();
                btnPlay.Enabled = true;
            }
            else 
            {
                btnPlay.Enabled = false;

            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (btnPlay.Text == "Play")
            {
                String audio_file = current_audio_file;
                Audio.playSound(0, audio_file, playComplete);
                btnPlay.Text = "Pause";
            }
            else
            {
                Audio.disposeWave();
                btnPlay.Text = "Play";
            }
        }

        private void playComplete(object sender, EventArgs e)
        {
            btnPlay.Text = "Play";
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                btnRemove.Enabled = false;
                return;
            }
            int pos = listView1.SelectedItems[0].Index;
            listView1.Items.RemoveAt(pos);
            Model.Remove(pos);
            clearInputData();
            /*
            if (listView1.Items.Count > 0)
            {
                listView1.Items[0].Selected = true;
                listView1.Select();
            }*/
        }

        private void clearInputData() 
        {
            chkAlt.Checked = false;
            chkShift.Checked = false;
            chkCtr.Checked = false;
            cbKey.Text = "0";
            txtDescription.Text = "";
            current_audio_file = "";
            btnEdit.Enabled = false;
            btnRemove.Enabled = false;
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
                btnRemove_Click(sender, e);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (current_edit_id < 0)
            {
                btnAdd_Click(sender, e);
                return;
            }
            HotkeyItem a = getHotkeyItemInForm();
            String test_item = ValidateHotkeyItem(a);
            if (test_item != "true")
            {
                Form1.ShowMessage(test_item);
                return;
            }

            Model.Edit(current_edit_id, a);
            current_edit_id = -1;
            ShowListView();
            clearInputData();
        }
    }
}

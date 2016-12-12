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
    public partial class Form1 : Form
    {
        public static string appPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
        public static Form f1 = null;
        public static ArrayList arrHotkeyRegistered = new ArrayList();
        public static int AudioDevice = -1;
        public static bool toggleHotkeys = true;
        public Form1()
        {
            InitializeComponent();
            LoadAudioDevice();
            f1 = this;
            setArrayHotkey();

            //Set toggle hotkeys ALT + ESC
            Hotkey hk1 = createHotkey(false, true, false, "Escape");
            hk1.Pressed += delegate
            {
                if (toggleHotkeys)
                {
                    Audio.disposeWave();
                    clearArrayHotkey();
                }
                else
                    setArrayHotkey();
                toggleHotkeys = !toggleHotkeys;
            };
            hk1.Register(f1);
        }

        public static Hotkey createHotkey(bool Ctr, bool Alt, bool Shift, String Key)
        {
            switch (Key) {
                case "0": 
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    Key = "D" + Key;
                    break;
            }
            Hotkey hk = new Hotkey();
            hk.KeyCode = (Keys)Enum.Parse(typeof(Keys), Key, true);
            hk.Control = Ctr;
            hk.Alt = Alt;
            hk.Shift = Shift;
            return hk;
        }

        public static void clearArrayHotkey() 
        {
            foreach (Hotkey hk in arrHotkeyRegistered)
                if(hk.Registered)
                    hk.Unregister();                
        }

        public static void SingleStepAddHotkey(int i, ArrayList arr) 
        {
            if (i == arr.Count)
                return;

            HotkeyItem a = (HotkeyItem)arr[i];
            Hotkey hk = createHotkey(a.Ctr, a.Alt, a.Shift, a.Key);
            hk.Pressed += delegate
            {
                Audio.playSound(AudioDevice, a.AudioFile);
            };
            hk.Register(f1);
            arrHotkeyRegistered.Add(hk);

            i++;
            SingleStepAddHotkey(i, arr);
        }

        public static void setArrayHotkey()
        {
            ArrayList arr = Model.LoadData();
            SingleStepAddHotkey(0,arr);

            /*
            ArrayList arr = Model.LoadData();

            foreach (HotkeyItem a in arr)
            {
                Hotkey hk = createHotkey(a.Ctr, a.Alt, a.Shift, a.Key);
                hk.Pressed += delegate
                {
                    Audio.playSound(AudioDevice, a.AudioFile);
                };
                hk.Register(f1);
                arrHotkeyRegistered.Add(hk);
            }     
            */
        }

        private void LoadAudioDevice()
        {
            ArrayList arr = Audio.getAudioDevice();
            if (arr.Count == 0)
            {
                ShowMessage("You don't have any Output Device");
                return;
            }

            foreach (String DeviceName in arr)
            {
                cbAudioDevice.Items.Add(DeviceName);
            }
            cbAudioDevice.SelectedIndex = 0;
        }

        public static void ShowMessage(String msg,String title = "Error") {
            MessageBox.Show(msg, title);
        }

        public static void ShowErrorExit(String msg) {
            ShowMessage(msg, "Error");
            Application.Exit();
        }

        private void btnTestOutput_Click(object sender, EventArgs e)
        {
            if (btnTestOutput.Text == "Test Output")
            {
                int selectedDevice = cbAudioDevice.SelectedIndex;
                String test = CSGO_VirtualMic.Audio.playSound(selectedDevice, @".\audio\demo.mp3", DemoComplete);
                if (test == "true")
                    btnTestOutput.Text = "Stop Test";
                else
                    ShowMessage(test);
            }
            else {
                btnTestOutput.Text = "Test Output";
                CSGO_VirtualMic.Audio.disposeWave();
            }
            
        }

        private void DemoComplete(object sender, EventArgs e)
        {
            btnTestOutput.Text = "Test Output";
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Form frmEditHotkey = new frmEditHotkey();
            frmEditHotkey.ShowDialog();
        }

        private void cbAudioDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            AudioDevice = cbAudioDevice.SelectedIndex;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://goo.gl/q36jpe");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://goo.gl/YGrhe3");
        }

        private void btnShowSoundPanel_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mmsys.cpl");
        }

    }
}

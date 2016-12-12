using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CSGO_VirtualMic
{
    [Serializable]
    class HotkeyItem : IComparer
    {
        private bool m_Ctr;
        private bool m_Alt;
        private bool m_Shift;
        private String m_key;
        private String m_Description;
        private String m_AudioFile;

        public bool Ctr 
        {
            get { return m_Ctr; }
            set { m_Ctr = value; }
        }

        public bool Alt
        {
            get { return m_Alt; }
            set { m_Alt = value; }
        }

        public bool Shift 
        {
            get { return m_Shift; }
            set { m_Shift = value; }
        }

        public String Key
        {
            get { return m_key; }
            set { m_key = value; }
        }

        public String Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        public String AudioFile
        {
            get { return m_AudioFile; }
            set { m_AudioFile = value; }
        }

        public String getHotkey ()
        {
            return (m_Ctr ? "Ctrl + " : "") + (m_Alt ? "Alt + ":"") + (m_Shift ? "Shift + " : "") + m_key;
        }

        public override string ToString() {
            return getHotkey();
        }

        public int Compare(object x, object y)
        {
            if (x is HotkeyItem && y is HotkeyItem)
                return String.Compare((x as HotkeyItem).getHotkey(), (y as HotkeyItem).getHotkey());

            if (x is IComparable)
                return (x as IComparable).CompareTo(y);

            return 0;
        }
    }
}

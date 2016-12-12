using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CSGO_VirtualMic
{
    class Model
    {
        public static ArrayList arrList = new ArrayList();
        
        public static ArrayList LoadData()
        {
            try
            {
                FileStream fs = new FileStream("config.dat", FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                arrList = (ArrayList)bf.Deserialize(fs);
                fs.Close();
            }
            catch (FileNotFoundException e1)
            {
                SaveData();
            }
            catch (Exception ex) 
            {
                Form1.ShowMessage(ex.Message);
            }
            return arrList;
        }

        public static bool SaveData() 
        {
            arrList.Sort(new HotkeyItem());
            FileStream str = File.Create("config.dat");
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(str, arrList);
            str.Close();
            Form1.clearArrayHotkey();
            Form1.setArrayHotkey();
            return true;
        }

        public static bool Add(HotkeyItem a) 
        {
            if (CheckDuplicate(a))
                return false;
            arrList.Add(a);
            SaveData();
            return true;
        }

        public static void Remove(int index)
        {
            if (arrList == null)
                return ;
            arrList.RemoveAt(index);
            SaveData();
        }

        public static void Edit(int index, HotkeyItem a) 
        {
            if (index < 0)
            {
                Add(a);
                return;
            }
            arrList[index] = a;
            SaveData();
        }

        public static bool CheckDuplicate(HotkeyItem a)
        {
            if (arrList == null)
                return false;
            foreach(HotkeyItem b in arrList)
            {
                if (a.Ctr == b.Ctr && a.Alt == b.Alt && a.Shift == b.Shift && a.Key == b.Key)
                    return true;
            }
            return false;
        }

    }
}

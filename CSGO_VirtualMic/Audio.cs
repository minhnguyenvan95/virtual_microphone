using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using System.Collections;

namespace CSGO_VirtualMic
{
    [Serializable]
    class Audio
    {
        static NAudio.Wave.BlockAlignReductionStream stream = null;
        static NAudio.Wave.WaveOut output = null;
        public static void disposeWave()
        {
            if (output != null)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                    output.Stop();
                output.Dispose();
            }
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
        }

        public static String playSound(int deviceNumber, String audioPatch, EventHandler Stopped_Event = null)
        {
            disposeWave();

            try
            {
                if (audioPatch.EndsWith(".mp3"))
                {
                    NAudio.Wave.WaveStream pcm = NAudio.Wave.WaveFormatConversionStream.CreatePcmStream(new NAudio.Wave.Mp3FileReader(audioPatch));
                    stream = new NAudio.Wave.BlockAlignReductionStream(pcm);
                }
                else if (audioPatch.EndsWith(".wav"))
                {
                    NAudio.Wave.WaveChannel32 pcm = new NAudio.Wave.WaveChannel32(new NAudio.Wave.WaveFileReader(audioPatch));
                    stream = new NAudio.Wave.BlockAlignReductionStream(pcm);
                    pcm.PadWithZeroes = false;
                }
                else
                {
                    return "Not a valid audio file";
                }

                output = new NAudio.Wave.WaveOut();
                output.DeviceNumber = deviceNumber;
                output.Init(stream);
                output.Play();

                if (Stopped_Event != null)
                    output.PlaybackStopped += new EventHandler<StoppedEventArgs>(Stopped_Event);
            }
            catch (Exception ex)
            {
                 return ex.Message;
            }

            return "true";
        }


        public static ArrayList getAudioDevice()
        {
            ArrayList arr = new ArrayList();
            if (WaveOut.DeviceCount == 0)
                Form1.ShowErrorExit("You don't have any Output Device");

            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                WaveOutCapabilities WOC = WaveOut.GetCapabilities(i);
                arr.Add(WOC.ProductName);
            }
            return arr;
        }
    }
}

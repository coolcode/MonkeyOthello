using System;
using System.Runtime.InteropServices;

namespace MonkeyOthello.IO
{
    public class PlaySound
    {
        [DllImport("winmm.dll", EntryPoint = "sndPlaySound")]
        public static extern bool sndPlaySound(ref  Byte snd, int fuSound);

        private static byte[] sound;

        /// <summary>
        /// ≤•∑≈…˘“Ù
        /// </summary>
        public static  void DownStone()
        {
            try
            {
                if (sound == null)
                    ReadSoundFile();
                sndPlaySound(ref   sound[0], 0x04);//≤•∑≈…˘“Ù
            }
            catch
            {
            }
        }

        public static bool ReadSoundFile()
        {
            try
            {
                System.IO.UnmanagedMemoryStream wav = global::MonkeyOthello.Properties.Resources.DownStone;
                int offset = 0;
                int readed = 1;
                long length = wav.Length;
                sound = new byte[length];
                while (readed > 0 && offset < length)
                {
                    readed = wav.Read(sound, offset, (int)length - offset);
                    offset += readed;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

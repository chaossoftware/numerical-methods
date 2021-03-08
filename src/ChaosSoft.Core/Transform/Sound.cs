using System;
using System.IO;
using System.Text;

namespace ChaosSoft.Core.Transform
{
    public class Sound
    {
        /// <summary>
        /// Create WAV file of signal "sound"
        /// </summary>
        /// <param name="filePath">output sound file name</param>
        /// <param name="yt">Y coordinates of signal</param>
        public static void CreateWavFile(string filePath, double[] yt)
        {
            long pts = yt.Length;

            double xtmin = Ext.CountMin(yt);
            double xtmax = Ext.CountMax(yt);

            File.Delete(filePath);
            FileStream wavFile = File.Create(filePath);
            byte[] info;

            info = new UTF8Encoding(true).GetBytes("RIFF");
            wavFile.Write(info, 0, info.Length);

            info = BitConverter.GetBytes((int)(pts - 36));  // as Long (4 bytes)
            wavFile.Write(info, 0, info.Length);

            info = new UTF8Encoding(true).GetBytes("WAVEfmt ");
            wavFile.Write(info, 0, info.Length);

            info = BitConverter.GetBytes((int)16);          // as Long (4 bytes)
            wavFile.Write(info, 0, info.Length);

            info = BitConverter.GetBytes((Int16)1);         // as Word (2 bytes)
            wavFile.Write(info, 0, info.Length);

            info = BitConverter.GetBytes((Int16)1);         // as Word (2 bytes)
            wavFile.Write(info, 0, info.Length);

            info = BitConverter.GetBytes((int)22050);       // as Long (4 bytes)    
            wavFile.Write(info, 0, info.Length);

            info = BitConverter.GetBytes((int)22050);       // as Long (4 bytes)
            wavFile.Write(info, 0, info.Length);

            info = BitConverter.GetBytes((Int16)1);         // as Word (2 bytes)
            wavFile.Write(info, 0, info.Length);

            info = BitConverter.GetBytes((Int16)8);         // as Word (2 bytes)
            wavFile.Write(info, 0, info.Length);

            info = new UTF8Encoding(true).GetBytes("data");
            wavFile.Write(info, 0, info.Length);

            info = BitConverter.GetBytes((int)pts);         // as Long (4 bytes)
            wavFile.Write(info, 0, info.Length);

            // as bytes array
            byte[] data = new byte[pts];

            for (int t = 0; t < pts; t++)
            {
                double _yt = 255 * (yt[t] - xtmin) / (xtmax - xtmin);
                data[t] = (byte)_yt;
            }

            wavFile.Write(data, 0, data.Length);
            wavFile.Close();
        }
    }
}

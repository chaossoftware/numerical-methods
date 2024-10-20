using System;
using System.IO;
using System.Text;
using ChaosSoft.Core.DataUtils;

namespace ChaosSoft.NumericalMethods.Transform;

/// <summary>
/// Provides methods to transform series to sound.
/// </summary>
public static class Sound
{
    /// <summary>
    /// Create WAV file of signal "sound" with specified quality params.
    /// </summary>
    /// <param name="filePath">output sound file name</param>
    /// <param name="freq">frequency of quantization</param>
    /// <param name="bits">digits of quantization</param>
    /// <param name="series">signal</param>
    public static void CreateWavFile(string filePath, int freq, int bits, double[] series)
    {
        long pts = series.Length;

        double xtmin = Vector.Min(series);
        double xtmax = Vector.Max(series);

        File.Delete(filePath);
        FileStream wavFile = File.Create(filePath);
        byte[] info;

        // (4 bytes) File description header
        info = new UTF8Encoding(true).GetBytes("RIFF");
        wavFile.Write(info, 0, info.Length);

        // (4 bytes) Size of file
        // The file size not including the "RIFF" description (4 bytes)
        // and file description (4 bytes). This is file size - 8.
        info = BitConverter.GetBytes((int)(pts - 8));
        wavFile.Write(info, 0, info.Length);

        // (4 bytes) WAV description header
        info = new UTF8Encoding(true).GetBytes("WAVE");
        wavFile.Write(info, 0, info.Length);

        // (4 bytes) Format description header
        info = new UTF8Encoding(true).GetBytes("fmt ");
        wavFile.Write(info, 0, info.Length);

        // (4 bytes) Size of WAVE section chunck	
        // The size of the WAVE type format (2 bytes) +
        // mono/stereo flag (2 bytes) +
        // sample rate (4 bytes) +
        // bytes per sec (4 bytes) +
        // block alignment (2 bytes) +
        // bits per sample (2 bytes).
        // This is usually 16.
        info = BitConverter.GetBytes(16);
        wavFile.Write(info, 0, info.Length);

        // (2 bytes) WAVE type format
        // Type of WAVE format. This is a PCM header = $01 (linear quntization).
        // Other values indicates some forms of compression.
        info = BitConverter.GetBytes((short)1);
        wavFile.Write(info, 0, info.Length);

        // (2 bytes) Number of channels
        // mono ($01) or stereo ($02)
        info = BitConverter.GetBytes((short)1);
        wavFile.Write(info, 0, info.Length);

        // (4 bytes)     Samples per second
        // The frequency of quantization (usually 44100 Hz, 22050 Hz, ...)
        info = BitConverter.GetBytes(freq);
        wavFile.Write(info, 0, info.Length);

        // (4 bytes) Bytes per second
        // Speed of data stream = Number_of_channels * Samples_per_second * Bits_per_Sample/8
        info = BitConverter.GetBytes(freq * bits / 8);
        wavFile.Write(info, 0, info.Length);

        // (2 bytes) Block alignment
        // Number of bytes in elementary quantization = Number_of_channels*Bits_per_Sample/8
        info = BitConverter.GetBytes((short)(bits / 8));
        wavFile.Write(info, 0, info.Length);

        // (2 bytes) Bits per sample 
        // Digits of quantization (usually 32, 24, 16, 8)
        info = BitConverter.GetBytes((short)bits);
        wavFile.Write(info, 0, info.Length);

        // Data description header
        info = new UTF8Encoding(true).GetBytes("data");
        wavFile.Write(info, 0, info.Length);

        // (4 bytes) Size of data
        info = BitConverter.GetBytes((int)pts);
        wavFile.Write(info, 0, info.Length);

        // as bytes array
        byte[] data = new byte[pts];

        for (int t = 0; t < pts; t++)
        {
            double _yt = 255 * (series[t] - xtmin) / (xtmax - xtmin);
            data[t] = (byte)_yt;
        }

        // Data
        wavFile.Write(data, 0, data.Length);
        wavFile.Close();
    }

    /// <summary>
    /// Create WAV file (8Khz8bit) of signal "sound"
    /// </summary>
    /// <param name="filePath">output sound file name</param>
    /// <param name="series">signal</param>
    public static void CreateWavFile(string filePath, double[] series) =>
        CreateWavFile(filePath, 8000, 8, series);
}

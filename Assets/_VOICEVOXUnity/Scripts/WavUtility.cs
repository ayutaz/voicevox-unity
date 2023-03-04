using System;
using UnityEngine;

namespace _VoiceVoxUnity.Scripts
{
    public abstract class WavUtility
    {
        public static AudioClip ToAudioClip(byte[] data)
        {
            // ヘッダー解析
            int channels = data[22];
            var frequency = BitConverter.ToInt32(data, 24);
            var length = data.Length - 44;
            var samples = new float[length / 2];

            // 波形データ解析
            for (var i = 0; i < length / 2; i++)
            {
                var value = BitConverter.ToInt16(data, i * 2 + 44);
                samples[i] = value / 32768f;
            }

            // AudioClipを作成
            var audioClip = AudioClip.Create("AudioClip", samples.Length, channels, frequency, false);
            audioClip.SetData(samples, 0);

            return audioClip;
        }
    }
}
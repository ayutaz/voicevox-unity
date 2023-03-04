using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace _VoiceVoxUnity.Scripts
{
    public class VoiceVoxConnection
    {
        private const string VoiceVoxUrl = "http://192.168.11.3:50021";
        private readonly int _speaker;

        public VoiceVoxConnection(int speaker = 3)
        {
            _speaker = speaker;
        }

        public async UniTask<AudioClip> TranslateTextToAudioClip(string text)
        {
            return await TranslateTextToAudioClip(text, _speaker);
        }

        public async UniTask<AudioClip> TranslateTextToAudioClip(string text, int speaker)
        {
            var queryJson = await SendAudioQuery(text, speaker);
            var clip = await GetAudioClip(queryJson);
            return clip;
        }

        private static async UniTask<string> SendAudioQuery(string text, int speaker)
        {
            var form = new WWWForm();
            using var request =
                UnityWebRequest.Post($"{VoiceVoxUrl}/audio_query?text={text}&speaker={speaker}", form);
            await request.SendWebRequest();

            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                var jsonString = request.downloadHandler.text;
                return jsonString;
            }

            return null;
        }

        private async UniTask<AudioClip> GetAudioClip(string queryJson)
        {
            var url = $"{VoiceVoxUrl}/synthesis?speaker={_speaker}";
            using var req = new UnityWebRequest(url, "POST");
            // Content-Type を設定
            req.SetRequestHeader("Content-Type", "application/json");

            // リクエストボディを設定
            var bodyRaw = Encoding.UTF8.GetBytes(queryJson);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            // レスポンスの取得に必要な設定を行う
            req.downloadHandler = new DownloadHandlerBuffer();

            await req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError ||
                req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(req.error);
            }
            else
            {
                var audioClip = WavUtility.ToAudioClip(req.downloadHandler.data);
                return audioClip;
            }

            return null;
        }
    }
}
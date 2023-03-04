using UnityEngine;

namespace _VoiceVoxUnity.Scripts
{
    public class VoiceVoxPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;
        private VoiceVoxConnection _voiceVoxConnection;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _voiceVoxConnection = new VoiceVoxConnection();
        }

        private async void Start()
        {
            var text = "一緒に冒険をしに行こう";
            var clip = await _voiceVoxConnection.TranslateTextToAudioClip(text);
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }
}
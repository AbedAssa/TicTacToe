using UnityEngine;
using Infra;

namespace Controllers
{
    /// <summary>
    /// Responsible of controlling the audio.
    /// </summary>
    public class AudioController : MonoBehaviour
    {
        public static AudioController Instance;
        [SerializeField] private AudioSource mainAudioSource;
        [SerializeField] private AudioClip squareScaleUpClip;
        [SerializeField] private AudioClip buttonClickClip;
        [SerializeField] private AudioClip popUpResultClip;
        private void Awake()
        {
            Instance = this;
        }

        public void PlaySound(AudioTypes audioTypes)
        {
            if (!IsSoundOn())
            {
                return;
            }
            if (mainAudioSource == null || squareScaleUpClip == null)
            {
                Debug.LogError("Audio references not set");
                return;
            }
            mainAudioSource.clip = GetAudioClip(audioTypes);
            mainAudioSource.Play();
        }

        private AudioClip GetAudioClip(AudioTypes audioTypes)
        {
            switch (audioTypes)
            {
                case AudioTypes.ButtonClick:
                    return buttonClickClip;
                case AudioTypes.GameOver:
                    return popUpResultClip;
                case AudioTypes.Symbol:
                    return squareScaleUpClip;
                default:
                    return null;
            }
        }

        private bool IsSoundOn()
        {
            return PlayerPrefs.HasKey(PlayerPrefsKeys.AudioKey) && PlayerPrefs.GetInt(PlayerPrefsKeys.AudioKey) == 1;
        }
    }
}


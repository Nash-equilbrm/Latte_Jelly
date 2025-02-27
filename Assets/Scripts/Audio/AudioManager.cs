using Commons;
using Patterns;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        public AudioSource musicSource;
        public AudioSource sfxSource;
        private Dictionary<string, AudioClip> _sfxClips = new Dictionary<string, AudioClip>();

        protected override void Awake()
        {
            base.Awake();
            LoadAllSFX();
        }

        private void LoadAllSFX()
        {
            AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/sfx");
            foreach (var clip in clips)
            {
                _sfxClips[clip.name] = clip;
            }
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (musicSource.clip == clip) return;

            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void PlaySFX(string clipName)
        {
            if (_sfxClips.TryGetValue(clipName, out AudioClip clip))
            {
                sfxSource.PlayOneShot(clip);
            }
            else
            {
                LogUtility.InvalidInfo("AudioManager", $"SFX clip '{clipName}' not found!");
            }
        }
    }


}

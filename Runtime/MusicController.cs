using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace MusicFlowController
{
    public class MusicController : MonoBehaviour
    {

        [SerializeField] private List<MusicTrack> musicTracks;
        [Range(0f, 1f)] public float volume = 1f;
        public float transitionSpeed = 0.01f;
        public MusicTransitionType transitionMode = MusicTransitionType.EqualPowerCrossfade;
        [SerializeField] AudioMixerGroup audioMixerGroup;
        
        public List<MusicTrack> MusicTracks => musicTracks;
        
        void Start()
        {
            Initialize();
        }

        public void Initialize(MusicTrack[] newMusicTracks) => Initialize(new List<MusicTrack>(newMusicTracks));
        public void Initialize(List<MusicTrack> newMusicTracks = null)
        {
            if(newMusicTracks != null)
                musicTracks = newMusicTracks;
            
            foreach (MusicTrack musicTrack in musicTracks)
                musicTrack.Initialize(gameObject, audioMixerGroup);
            
            foreach (MusicTrack musicTrack in musicTracks)
                musicTrack.Source.Play();
            
            Debug.Log("MusicController Initialized");
        }

        public void PlayTrack(int index, bool stopOtherTracks = false)
        {
            int[] indexesToStop = Array.Empty<int>();
            if(stopOtherTracks)
                indexesToStop = Enumerable.Range(0, musicTracks.Count).Where(i => i != index).ToArray();
            
            PlayTracks(new int[] {index}, indexesToStop);
        }
        public void StopTrack(int index) => PlayTracks(Array.Empty<int>(), new int[] {index});
        public void StopAllTracks() => PlayTracks(Array.Empty<int>(), Enumerable.Range(0, musicTracks.Count).ToArray());
        public void PlayTracks(int[] indexesToStart, int[] indexesToStop) => PlayTracks(
            musicTracks?.Where((x, i) => indexesToStart.Contains(i)).ToArray() ?? Array.Empty<MusicTrack>(),
            musicTracks?.Where((x, i) => indexesToStop.Contains(i)).ToArray() ?? Array.Empty<MusicTrack>()
        );
        public void PlayTracks(MusicTrack[] musicTracksToPlay, MusicTrack[] musicTracksToStop)
        {
            foreach (MusicTrack musicTrack in musicTracksToPlay)
                musicTrack.IsStarting = true;

            foreach (MusicTrack musicTrack in musicTracksToStop)
                musicTrack.IsStoping = true;
        }

        
        void Update()
        {
            switch (transitionMode)
            {
                case MusicTransitionType.Linear:
                    Linear();
                    break;
                case MusicTransitionType.Lerp:
                    Lerp();
                    break;
                case MusicTransitionType.EqualPowerCrossfade:
                    EqualsPowerCrossfade();
                    break;
            }
        }
        
        private void EqualsPowerCrossfade()
        {
            foreach (MusicTrack musicTrack in musicTracks)
            {
                if(musicTrack.IsStarting || musicTrack.IsStoping)
                    musicTrack.eTimer += Time.deltaTime;
                else
                    continue;

                float t = Mathf.Clamp01(musicTrack.eTimer / transitionSpeed);

                if (musicTrack.IsStarting)
                    musicTrack.Source.volume = Mathf.Sin(t * Mathf.PI * 0.5f) * volume;
                else if (musicTrack.IsStoping)
                    musicTrack.Source.volume = Mathf.Cos(t * Mathf.PI * 0.5f) * volume;

                if (t >= 1f)
                {
                    musicTrack.IsStarting = false;
                    musicTrack.IsStoping = false;
                    musicTrack.eTimer = 0f;
                }
            }
        }

        private void Linear()
        {
            foreach (MusicTrack musicTrack in musicTracks)
            {
                if (musicTrack.IsStarting)
                {
                    if(Mathf.Abs(musicTrack.Source.volume - volume) <= 0.001f)
                        musicTrack.IsStarting = false;
                    else
                        musicTrack.Source.volume = Mathf.MoveTowards(musicTrack.Source.volume, volume, transitionSpeed);
                } 
                else if (musicTrack.IsStoping)
                {
                    if(musicTrack.Source.volume <= 0.001f)
                        musicTrack.IsStoping = false;
                    else
                        musicTrack.Source.volume = Mathf.MoveTowards(musicTrack.Source.volume, 0f, transitionSpeed);
                }
            }
        }

        private void Lerp()
        {
            foreach (MusicTrack musicTrack in musicTracks)
            {
                if (musicTrack.IsStarting)
                {
                    if(Mathf.Abs(musicTrack.Source.volume - volume) <= 0.001f)
                        musicTrack.IsStarting = false;
                    else
                        musicTrack.Source.volume = Mathf.Lerp(musicTrack.Source.volume, volume, transitionSpeed);
                }
                else if (musicTrack.IsStoping)
                {
                    if(musicTrack.Source.volume <= 0.001f)
                        musicTrack.IsStoping = false;
                    else
                        musicTrack.Source.volume = Mathf.Lerp(musicTrack.Source.volume, 0f, transitionSpeed);
                }
                    
            }
        }
    }
}
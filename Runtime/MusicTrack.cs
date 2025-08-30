using System;
using UnityEngine;
using UnityEngine.Audio;

namespace MusicFlowController
{
    [Serializable]
    public class MusicTrack
    {
        public string name;
        public AudioClip clip;
        internal AudioSource Source;

        private bool _isStarting = false;
        internal bool IsStarting
        {
            get => _isStarting;
            set
            { 
                _isStarting = value;
                _isStopping = false;
                eTimer = 0f;
            } 
        }
        
        private bool _isStopping = false;
        internal bool IsStoping
        {
            get => _isStopping;
            set
            {
                _isStopping = value;
                _isStarting = false;
                eTimer = 0f;
            } 
        }

        internal float eTimer = 0f;

        public float Volume => Source.volume;
        
        public void Initialize(GameObject gameObject, AudioMixerGroup audioMixerGroup = null)
        {
            Source = gameObject.AddComponent<AudioSource>();
            Source.clip = clip;
            Source.loop = true;
            Source.volume = 0f;
            Source.outputAudioMixerGroup = audioMixerGroup;
            
            if(String.IsNullOrEmpty(name))
                name = clip.name;
            
            Debug.Log(name);
        }
    }
}
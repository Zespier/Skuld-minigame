/*
 * Tips for audio optimization:
 * - SPATIAL BLEND: 
 *      The spatial blend sets how much the audio is going to be affected by 3D calculations (attenuation, doppler, etc.).
 *      If the audio source gets closer to the audio listener, a good practice would be to lerp into 2D to benefit from 2D audio properties
 *      - Optimization => Set the audio clip to "Force Mono". If the spatial blend is set to 1 (3D), then the audio is being reproduced as mono, because the two audio channels originate from the same spot.
 *      _ Possible problems => If the audio gets too loud deselect Normalize.
 *      
 * - Checking the distance while is playing
 * - Always pause the audio source istead of enable, maybe this reproduces faster the audios.
 * - If an audio is too far away in the moment of instantiation, it would be cool to use the feature of the virtual voices, still reproducing and come back alive as if it was playing, but I don't know how
 */

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioOptimization : MonoBehaviour {

    public AudioSource audioSource;
    public AudioCategory audioCategory;

    private float _distanceFromPlayerSquared;
    private double _soundTime;
    private float _randomInitialTime;

    public AudioListener _audioListener => AudioListenerSingleton.instance.audioListener;

    private void Awake() {
        _randomInitialTime = Random.Range(0, 1000);
        if (_audioListener != null) {
            _distanceFromPlayerSquared = DistanceSquared(transform.position, _audioListener.transform.position);
        }

        ToggleAudioSource(_distanceFromPlayerSquared <= audioSource.maxDistance * audioSource.maxDistance);
    }

    void Update() {

        if (AudioSettings.dspTime >= _soundTime) {
            Deactivate();
        }

        if (_audioListener != null) {
            _distanceFromPlayerSquared = DistanceSquared(transform.position, _audioListener.transform.position);
        }

        ToggleAudioSource(_distanceFromPlayerSquared <= audioSource.maxDistance * audioSource.maxDistance);
    }

    void ToggleAudioSource(bool isAudible) {

        if (!isAudible && audioSource.isPlaying) {
            Deactivate();

        } else if (isAudible && !audioSource.isPlaying) {
            if ((audioCategory == AudioCategory.ConstantBrazierSound || audioCategory == AudioCategory.ConstantAmbientSound) && AudioManager.instance.CurrentActiveVoices < AudioManager.instance.maxRealVoices - AudioManager.instance.VoiceSpaceMargin(this.audioCategory)) {
                Activate();
            }
        }
    }

    public void Activate() {

        switch (audioCategory) {
            case AudioCategory.GenericPoolSoundLowPriority:
            case AudioCategory.GenericPoolSoundMaxPriority:
                if (!audioSource.enabled) { audioSource.enabled = true; }
                if (!enabled) { enabled = true; }
                break;

            case AudioCategory.ConstantAmbientSound:
            case AudioCategory.ConstantBrazierSound:
                if (!audioSource.enabled) { audioSource.enabled = true; }
                if (!enabled) { enabled = true; }   
                break;

            case AudioCategory.FrequentReactivation:
                if (!enabled) { enabled = true; }  
                break;

            case AudioCategory._0DelayNeeded:
                audioSource.time = (float)((AudioSettings.dspTime) % audioSource.clip.length);
                if (!enabled) { enabled = true; } 
                break;
        }

        if (audioCategory != AudioCategory.ConstantAmbientSound) {
            _soundTime = AudioSettings.dspTime + (double)audioSource.clip.length;
        } else {
            _soundTime = double.MaxValue;
        }

        if (_audioListener != null) {
            _distanceFromPlayerSquared = DistanceSquared(transform.position, _audioListener.transform.position);
        }
        bool canPlay = _distanceFromPlayerSquared <= audioSource.maxDistance * audioSource.maxDistance;

        if (canPlay) {

            switch (audioCategory) {
                case AudioCategory.GenericPoolSoundMaxPriority:
                case AudioCategory.GenericPoolSoundLowPriority:
                    audioSource.Play();
                    break;
                case AudioCategory.ConstantAmbientSound:
                case AudioCategory.ConstantBrazierSound:
                    audioSource.Play();
                    audioSource.Pause();
                    audioSource.time = (float)((AudioSettings.dspTime + _randomInitialTime) % audioSource.clip.length);
                    audioSource.UnPause();
                    break;
                case AudioCategory.FrequentReactivation:
                case AudioCategory._0DelayNeeded:
                    audioSource.UnPause();
                    break;
            }
        }
    }

    /// <summary>
    /// Enables this audioSource and moves it to the parameter position
    /// </summary>
    /// <param name="position"></param>
    public void Activate(Vector3 position) {
        Activate();
        transform.position = position;
    }

    /// <summary>
    /// Deactivates this audioSource
    /// </summary>
    public void Deactivate() {

        switch (audioCategory) {
            case AudioCategory.GenericPoolSoundMaxPriority:
            case AudioCategory.GenericPoolSoundLowPriority:
                audioSource.enabled = false;
                enabled = false;
                break;

            case AudioCategory.ConstantAmbientSound:
            case AudioCategory.ConstantBrazierSound:
                audioSource.enabled = false;
                break;

            case AudioCategory.FrequentReactivation:
                audioSource.Pause();
                audioSource.time = 0;
                break;

            case AudioCategory._0DelayNeeded:
                audioSource.Pause();
                break;
        }
    }

    [ContextMenu("Get Reference")]
    public void GetReference() {
        audioSource = GetComponent<AudioSource>();
    }

    public void ForceAudioLenght() {
        _soundTime = float.MaxValue;
    }

    public static float DistanceSquared(Vector3 a, Vector3 b) {
        return (a.x - b.x) * (a.x - b.x) +
               (a.y - b.y) * (a.y - b.y) +
               (a.z - b.z) * (a.z - b.z);
    }
}

public enum AudioCategory {
    GenericPoolSoundMaxPriority = 0,
    ConstantAmbientSound = 1,
    ConstantBrazierSound = 5,
    FrequentReactivation = 2,
    _0DelayNeeded = 3,

    GenericPoolSoundLowPriority = 4,
}

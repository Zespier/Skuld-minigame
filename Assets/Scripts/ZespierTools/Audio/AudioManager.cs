using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {

    public AudioMixer audioMixer;
    public AudioPool audioPool;
    public AudioSource battleMusic;
    public AudioSource lobbyMusic;
    public AudioClip towerPlacement;
    public AudioClip towerShoot;
    public AudioClip towerBreaking;
    public AudioClip hitSound;
    public AudioClip battleHorn;
    public AudioClip finalBattleHorn;
    public AudioClip[] skuldFightGrunts;
    public AudioClip dummyExplosion;
    public AudioClip brazierSound;
    public AudioClip bardSong;
    public AudioClip bardAttack;
    public AudioClip doorsOpening;
    public AudioClip surturLoop;
    public float gruntPitchRange = 0.07f;
    public int maxRealVoices = 52;
    public AudioSource defaultSettings;
    public AudioSource brazierSettings;
    public AudioSource bardSongSettings;
    public AudioSource towerShootSettings;
    public AudioSource hitSoundSettings;
    public AudioSource doorsOpeningSettings;

    [HideInInspector] public AudioCalls audioCalls;
    private string _lastSceneName;
    private int _lastIndex;
    private AudioOptimization _bardSong;
    private float _lobbyMusicDefaultVolume;

    public int CurrentActiveVoices => GetNumberOfActiveVoices();

    public static AudioManager instance;
    private void Awake() {
        if (!instance) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        audioCalls = new AudioCalls(this, audioPool);
        _lobbyMusicDefaultVolume = lobbyMusic.volume;

        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += StartMusic;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= StartMusic;
    }

    private void Update() {
        if (_bardSong != null) {
            float minDistance = bardSongSettings.minDistance + (bardSongSettings.maxDistance - bardSongSettings.minDistance) / 2f;
            float lerpValue = Mathf.Clamp((Vector3.Distance(AudioListenerSingleton.instance.transform.position, _bardSong.transform.position) - minDistance) / (bardSongSettings.maxDistance - minDistance), 0, 1);
            lobbyMusic.volume = Mathf.Lerp(0, _lobbyMusicDefaultVolume, lerpValue);
        }
    }

    public void PlayBrazierSound(Vector3 position) {
        audioCalls.PlaySound(AudioCategory.ConstantBrazierSound, brazierSound, position: position, spatialBlendSettings: brazierSettings);
    }

    public void PlayBardSong(Transform bardTransform, Vector3 position) {
        _bardSong = audioCalls.PlaySound(AudioCategory.ConstantAmbientSound, bardSong, bardTransform, position, spatialBlendSettings: bardSongSettings, volume: 0.4f);
    }

    public void PlayDummyExplosion() {
        audioCalls.PlaySound(AudioCategory.GenericPoolSoundMaxPriority, dummyExplosion, volume: 0.5f);
    }

    private void StartMusic(Scene scene, LoadSceneMode mode) {

        if (scene.name != _lastSceneName) {

            if (scene.name == "DeckBuilding" || scene.name == "LoadingScreen") {

            } else {
                battleMusic.Stop();
                lobbyMusic.Stop();


                if (scene.name == "Lobby") {
                    lobbyMusic.Play();
                } else if (scene.name == "Level 1 - Asgard" || scene.name == "Tutorial" || scene.name == "Level 2 - Jotunheim" || scene.name == "Level 3 - Yslasil") {
                    battleMusic.Play();
                }
            }
        }
        _lastSceneName = scene.name;
    }

    public void PlayGrunts(float time) {
        StartCoroutine(C_GruntsWithDelay(time));
    }

    private IEnumerator C_GruntsWithDelay(float time) {
        while (time >= 0) {
            time -= Time.deltaTime;
            yield return null;
        }

        int randomIndex;
        do {
            randomIndex = Random.Range(0, skuldFightGrunts.Length);

        } while (randomIndex == _lastIndex);

        audioCalls.PlaySound(AudioCategory.GenericPoolSoundMaxPriority, skuldFightGrunts[randomIndex], volume: 0.23f);

        _lastIndex = randomIndex;
    }

    public void PlayShortSound(ShortSound type, Vector3 position) {
        switch (type) {
            case ShortSound.TowerPlacement:
                audioCalls.PlaySound(AudioCategory.GenericPoolSoundLowPriority, towerPlacement, position: position, spatialBlendSettings: hitSoundSettings, volume: 0.4f);
                break;

            case ShortSound.TowerShoot:
                audioCalls.PlaySound(AudioCategory.GenericPoolSoundLowPriority, towerShoot, position: position, spatialBlendSettings: hitSoundSettings);
                break;

            case ShortSound.TowerBreaking:
                audioCalls.PlaySound(AudioCategory.GenericPoolSoundLowPriority, towerBreaking, position: position, spatialBlendSettings: hitSoundSettings, volume: 0.5f);
                break;

            case ShortSound.HitSound:
                audioCalls.PlaySound(AudioCategory.GenericPoolSoundLowPriority, hitSound, position: position, spatialBlendSettings: hitSoundSettings);
                break;

            case ShortSound.bardAttack:
                audioCalls.PlaySound(AudioCategory.GenericPoolSoundLowPriority, bardAttack, volume: 0.2f, position: position, spatialBlendSettings: hitSoundSettings);
                break;

            case ShortSound.doorsOpening:
                audioCalls.PlaySound(AudioCategory.GenericPoolSoundLowPriority, doorsOpening, volume: 1, position: position, spatialBlendSettings: doorsOpeningSettings);
                break;
            default:
                break;
        }
    }

    public void PlayBattleHorn(int waveNumber) {
        if (waveNumber > 10) {
            audioCalls.PlaySound(AudioCategory.GenericPoolSoundMaxPriority, finalBattleHorn, volume: 0.43f);

        } else {
            audioCalls.PlaySound(AudioCategory.GenericPoolSoundMaxPriority, battleHorn, volume: 0.77f);
        }
    }

    #region Volume management

    /// <summary>
    /// Changes the volume of master
    /// </summary>
    /// <param name="volume"></param>
    public void VolumeMaster(float volume) {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    /// <summary>
    /// Changes the volume of sounds
    /// </summary>
    /// <param name="volume"></param>
    public void VolumeSounds(float volume) {
        audioMixer.SetFloat("Sounds", Mathf.Log10(volume) * 20);
    }

    /// <summary>
    /// Changes the volume of music
    /// </summary>
    /// <param name="volume"></param>
    public void VolumeMusic(float volume) {
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }

    #endregion

    #region Maybe Useful later
    ///// <summary>
    ///// Lerps the pitch of the sound of the ship depending on the speed
    ///// </summary>
    ///// <param name="speed"></param>
    //public void ShipSound(Vector3 speed) {
    //    float max = Mathf.Abs(speed.x) > Mathf.Abs(speed.y) ? Mathf.Abs(speed.x) : Mathf.Abs(speed.y);
    //    float targetValue = Mathf.Lerp(shipSoundMinPitch, shipSoundMaxPitch, (PlayerController.instance.speed * max) / PlayerController.instance.speed);

    //    float pitchLerpSpeed = 0.1f;
    //    if (shipSoundSource.pitch > targetValue) {
    //        pitchLerpSpeed = 0.2f;
    //    }

    //    shipSoundSource.pitch = Mathf.Lerp(shipSoundSource.pitch, targetValue, Time.deltaTime / pitchLerpSpeed);
    //}

    //private IEnumerator DebugTime(AudioClip clip) {
    //    DateTime before = DateTime.Now;
    //    TimeSpan beforeTime = before.TimeOfDay;

    //    while (clip.loadState != AudioDataLoadState.Loaded) {
    //        yield return null;
    //    }

    //    DateTime after = DateTime.Now;
    //    TimeSpan afterTime = after.TimeOfDay;

    //    TimeSpan duration = afterTime - beforeTime;


    //    Debug.Log($"The audio took => {duration.TotalSeconds} to load");
    //}


    //private void OnEnable() {
    //    Events.OnNextCharacter += PlayDialogueSound;
    //}

    //private void OnDisable() {
    //    Events.OnNextCharacter -= PlayDialogueSound;
    //}

    /// <summary>
    /// Sound for the dialogue
    /// Gets the next available audio source on the pool
    /// </summary>
    /// <param name="position"></param>
    //public void PlayDialogueSound(Voice voice) {
    //    switch (voice) {
    //        case Voice.player:
    //            PlayDialogueSound_Normal(player.position);
    //            break;
    //        case Voice.director:
    //            PlayDialogueSound_Deep(player.position);
    //            break;
    //        default:
    //            PlayDialogueSound_Normal(player.position);
    //            Debug.LogError("Non identified voice");
    //            break;
    //    }
    //}
    //public void PlayDialogueSound_Normal(Vector3 position) {
    //    audioCalls.PlaySound(normalVoice, position, volume: generalVolume);
    //}
    //public void PlayDialogueSound_Deep(Vector3 position) {
    //    audioCalls.PlaySound(deepVoice, position, volume: generalVolume);
    //}

    //public void PlayStepSound() {
    //    if (playableSteps == null || playableSteps.Count <= 0) {
    //        playableSteps = new List<int>() { 0, 1, 2, 3, 4 };
    //    }

    //    int randomIndex = UnityEngine.Random.Range(0, playableSteps.Count);
    //    audioCalls.PlaySound(stepSounds[playableSteps[randomIndex]], pitch: 0.75f + UnityEngine.Random.Range(-pitchRange, pitchRange), volume: stepsVolume);
    //    playableSteps.RemoveAt(randomIndex);
    //}

    //public void PlayLockTickSound() {
    //    audioCalls.PlaySound(lockTickSound, 1 + UnityEngine.Random.Range(-pitchRange, pitchRange), volume: generalVolume);
    //}

    #endregion

    public int VoiceSpaceMargin(AudioCategory audioCategory) {
        switch (audioCategory) {
            default:
            case AudioCategory.GenericPoolSoundMaxPriority:
                return 0;
            case AudioCategory.ConstantAmbientSound:
            case AudioCategory.ConstantBrazierSound:
                return 15;
            case AudioCategory.FrequentReactivation:
                return 0;
            case AudioCategory._0DelayNeeded:
                return 0;
            case AudioCategory.GenericPoolSoundLowPriority:
                return 10;
        }
    }

    private int GetNumberOfActiveVoices() {
        int activeVoices = 0;
        if (battleMusic.isPlaying) { activeVoices++; }
        if (lobbyMusic.isPlaying) { activeVoices++; }

        activeVoices += audioPool.GetNumberOfActiveVoices();

        return activeVoices;
    }
}

public enum ShortSound {
    TowerPlacement,
    TowerShoot,
    TowerBreaking,
    HitSound,
    bardAttack,
    doorsOpening,
}
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

namespace TopManagement
{
    public class AudioManager : MonoBehaviour
    {
        static AudioManager Instance;
        public static bool IsAvailable => Instance != null;

        public enum MusicType
        {
            Music1 = 0,
        }

        [SerializeField] AudioMixer masterMixer = null;
        [SerializeField] AudioMixerGroup musicGroup = null;
        [SerializeField] AudioMixerGroup soundGroup = null;
        [SerializeField] AudioMixerGroup stgrGroup = null;

        [Space, Space, Header("Audio Clips")]
        [Header("Music")]
        [SerializeField] SerializedDictionary<MusicType, AudioClip> musics = null;
        [Header("Gameplay")]
        [SerializeField] AudioClip SFX_Applause;
        [SerializeField] AudioClip SFX_CountDown_Loop;
        [SerializeField] AudioClip SFX_CountDown_Letter;
        [SerializeField] AudioClip SFX_CountDown_Spin;
        [SerializeField] AudioClip SFX_Letter_Fail;
        [SerializeField] AudioClip SFX_Letter_Found;
        [SerializeField] AudioClip[] SFX_Letter_Money_Fillups;
        [SerializeField] AudioClip[] SFX_Letter_Money_Flies;
        [SerializeField] AudioClip SFX_Wheel_Fail;
        [SerializeField] AudioClip SFX_Wheel_Success;
        [SerializeField] AudioClip SFX_Wheel_Swipe;
        [SerializeField] AudioClip SFX_Wheel_Turn;
        [SerializeField] AudioClip STGR_Lose;
        [SerializeField] AudioClip STGR_Wheel_Big_Prize;
        [SerializeField] AudioClip STGR_Win;
        [SerializeField] AudioClip freeSoundClink1;
        [SerializeField] AudioClip freeSoundClink2;
        [SerializeField] AudioClip[] dominoSounds;
        [Header("UI")]
        [SerializeField] AudioClip SFX_UI_Click_Booster;
        [SerializeField] AudioClip SFX_UI_Click_Generic;
        [SerializeField] AudioClip SFX_UI_Click_Letter;
        [SerializeField] AudioClip SFX_UI_Click_Play;
        [SerializeField] AudioClip SFX_UI_Click_Settings;
        [SerializeField] AudioClip SFX_UI_Click_Skip;
        [SerializeField] AudioClip SFX_UI_Click_Spin;
        [SerializeField] AudioClip SFX_UI_NextOppenent;
        [SerializeField] AudioClip SFX_UI_StreakPoint;
        [SerializeField] AudioClip gemSound;
        [SerializeField] AudioClip IAP_purchasedSound;

        AudioSource audioSource = null;
        AudioSource musicSource = null;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = soundGroup;

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.clip = musics[0];
            musicSource.loop = true;
            musicSource.Play();
            musicSource.outputAudioMixerGroup = musicGroup;
        }
        private void Start()
        {
            _SetSoundSettings();
        }

        void _SetSoundSettings()
        {
            masterMixer.SetFloat("musicvol", Is_Music_ON ? -10f : -80f);
            masterMixer.SetFloat("soundvol", Is_Sound_ON ? 0f : -80f);
            masterMixer.SetFloat("stgrVol", Is_Sound_ON ? 0f : -80f);
        }

        #region MUZIEK
        public static bool Is_Music_ON
        {
            get => PlayerPrefs.GetInt("musicOn", 1) == 1;
            set
            {
                if (value == (PlayerPrefs.GetInt("musicOn", 1) == 1)) return;
                PlayerPrefs.SetInt("musicOn", value ? 1 : 0);
                Instance._SetSoundSettings();
            }
        }

        public static void MuteMusic(bool mute) => Instance.musicSource.mute = mute;

        public static void PlayMusic(MusicType music)
        {
            if (Instance.musicSource.clip == Instance.musics[music]) return;

            Instance.musicSource.Stop();
            Instance.musicSource.clip = Instance.musics[music];
            Instance.musicSource.Play();
        }
        #endregion

        #region STGR
        public static void PlayStgrWin() => Instance.PlayStgr(Instance.STGR_Win);
        public static void PlayStgrLose() => Instance.PlayStgr(Instance.STGR_Lose);
        public static void PlayStgrWheel_Big_Prize() => Instance.PlayStgr(Instance.STGR_Wheel_Big_Prize);

        AudioSource PlayStgr(AudioClip clip, bool stopOnSceneChange = false, bool loop = false, float _pitch = 1f, float _volume = 1f)
        {
            if (clip == null) return null;

            //Check freed sources
            for (int i = inUseSources.Count - 1; i >= 0; i--)
            {
                if (inUseSources[i].isPlaying == false)
                {
                    availableSources.Enqueue(inUseSources[i]);
                    if (stopAuto.Contains(inUseSources[i])) stopAuto.Remove(inUseSources[i]);
                    inUseSources.RemoveAt(i);
                }
            }

            //Check if there is no available sources
            if (availableSources.Count == 0)
            {
                var newAS = gameObject.AddComponent<AudioSource>();
                newAS.outputAudioMixerGroup = stgrGroup;
                newAS.playOnAwake = false;
                availableSources.Enqueue(newAS);
            }

            //Play new audio clip
            var targetAS = availableSources.Dequeue();
            if (stopOnSceneChange) stopAuto.Add(targetAS);
            else if (stopAuto.Contains(targetAS)) stopAuto.Remove(targetAS);
            inUseSources.Add(targetAS);
            targetAS.outputAudioMixerGroup = stgrGroup;
            targetAS.clip = clip;
            targetAS.loop = loop;
            targetAS.pitch = _pitch;
            targetAS.volume = _volume;
            targetAS.Play();
            return targetAS;
        }
        #endregion

        #region GELUIDEN
        private List<AudioSource> inUseSources = new List<AudioSource>();
        private Queue<AudioSource> availableSources = new Queue<AudioSource>();
        private HashSet<AudioSource> stopAuto = new HashSet<AudioSource>();

        public static bool Is_Sound_ON
        {
            get => PlayerPrefs.GetInt("soundOn", 1) == 1;
            set
            {                
                if (value == (PlayerPrefs.GetInt("soundOn", 1) == 1)) return;
                PlayerPrefs.SetInt("soundOn", value ? 1 : 0);
                Instance._SetSoundSettings();
            }
        }

        public static void Stop(AudioClip clip) => Instance._Stop(clip);
        public static void StopAll() => Instance._StopAll();
        public static void PlayGenericButtonSound() => Play(Instance.SFX_UI_Click_Generic);
        public static void PlayMoneySound() => Play(Instance.SFX_Letter_Money_Fillups.Random());
        public static void PlayMoneyFlySound() => Play(Instance.SFX_Letter_Money_Flies.Random());
        public static void PlayButtonSound() => Play(Instance.SFX_UI_Click_Play);
        public static void PlaySettingsButtonSound() => Play(Instance.SFX_UI_Click_Settings);
        public static void PlayBoosterSound() => Play(Instance.SFX_UI_Click_Booster);
        public static void PlayLetterButton() => Play(Instance.SFX_UI_Click_Letter);
        public static void PlaySpinButton() => Play(Instance.SFX_UI_Click_Spin);
        public static void PlayLetterFailSound() => Play(Instance.SFX_Letter_Fail);
        public static void PlayLetterFoundSound() => Play(Instance.SFX_Letter_Found);
        public static void PlayWheelFailSound() => Play(Instance.SFX_Wheel_Fail);
        public static void PlayWheelSuccesSound() => Play(Instance.SFX_Wheel_Success);
        public static void PlaySkipSound() => Play(Instance.SFX_UI_Click_Skip);
        public static void PlayTimerCountDownSound() => Play(Instance.SFX_CountDown_Loop);
        public static void PlayTimerCountDownSpin() => Play(Instance.SFX_CountDown_Spin);
        public static void PlayTimerCountDownLetter() => Play(Instance.SFX_CountDown_Letter);
        public static void PlaySwipeSound() => Play(Instance.SFX_Wheel_Swipe);
        public static void PlayStreakIncreaseSound() => Play(Instance.SFX_UI_StreakPoint);
        public static void PlayNextOpponentSound() => Play(Instance.SFX_UI_NextOppenent);
        public static void PlayApplauseSound(float _duration) => Instance.PlayApplause(_duration);
        public static void PlayDominoSound() => Play(Instance.dominoSounds.Random());
        public static (AudioSource source, float pitch) PlaySpinWheelTurnSound(float _duration) => Instance.PlaySpinwheelTurningSound(_duration);
        public static void PlayRandomSwipeSound()
        {
            int random = Random.Range(0, 2);
            Play(random == 1 ? Instance.freeSoundClink1 : Instance.freeSoundClink2);
        }
        public static void PlayGemSound() => Play(Instance.gemSound);
        public static void PlayIAPSound()
        {
            if (!IsAvailable) return;
            Play(Instance.IAP_purchasedSound);
        }

        void _StopAll()
        {
            for (int i = inUseSources.Count - 1; i >= 0; i--)
            {
                if (stopAuto.Contains(inUseSources[i]))
                {
                    stopAuto.Remove(inUseSources[i]);
                    inUseSources[i].Stop();
                    availableSources.Enqueue(inUseSources[i]);
                    inUseSources.RemoveAt(i);
                }
            }
        }

        void _Stop(AudioClip clip)
        {
            for (int i = 0; i < inUseSources.Count; i++) if (inUseSources[i].isPlaying && inUseSources[i].clip == clip) inUseSources[i].Stop();
        }

        static AudioSource Play(AudioClip clip, bool stopOnSceneChange = false, bool loop = false, float _pitch = 1f, float _volume = 1f)
        {
            if (!IsAvailable) return null;
            return Instance._Play(clip, stopOnSceneChange, loop, _pitch, _volume);
        }

        AudioSource _Play(AudioClip clip, bool stopOnSceneChange = false, bool loop = false, float _pitch = 1f, float _volume = 1f)
        {
            if (clip == null) return null;

            //Check freed sources
            for (int i = inUseSources.Count - 1; i >= 0; i--)
            {
                if (inUseSources[i].isPlaying == false)
                {
                    availableSources.Enqueue(inUseSources[i]);
                    if (stopAuto.Contains(inUseSources[i])) stopAuto.Remove(inUseSources[i]);
                    inUseSources.RemoveAt(i);
                }
            }

            //Check if there is no available sources
            if (availableSources.Count == 0)
            {
                var newAS = gameObject.AddComponent<AudioSource>();
                newAS.outputAudioMixerGroup = soundGroup;
                newAS.playOnAwake = false;
                availableSources.Enqueue(newAS);
            }

            //Play new audio clip
            var targetAS = availableSources.Dequeue();
            if (stopOnSceneChange) stopAuto.Add(targetAS);
            else if (stopAuto.Contains(targetAS)) stopAuto.Remove(targetAS);
            inUseSources.Add(targetAS);
            targetAS.outputAudioMixerGroup = soundGroup;
            targetAS.clip = clip;
            targetAS.loop = loop;
            targetAS.pitch = _pitch;
            targetAS.volume = _volume;
            targetAS.Play();
            return targetAS;
        }

        (AudioSource source, float pitch) PlaySpinwheelTurningSound(float spinDuration)
        {
            float duration = SFX_Wheel_Turn.length;
            float pitch = duration / spinDuration;
            return (Play(SFX_Wheel_Turn, _pitch: pitch), pitch);
        }

        void PlayApplause(float _duration)
        {
            _duration = Mathf.Clamp(_duration, 0f, SFX_Applause.length * 0.95f);
            var auds = Play(Instance.SFX_Applause);
            auds.DOFade(0f, 1f).SetDelay(_duration);
        }
        #endregion
    }
}

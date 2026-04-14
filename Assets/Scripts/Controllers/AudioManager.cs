using UnityEngine;

namespace Controllers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("音频源")]
        public AudioSource sfxSource; // 音效专用（移动、点击）
        public AudioSource bgmSource; // 背景音乐专用

        [Header("音效配置")]
        

        [Header("音量设置")]
        [Range(0f, 1f)] public float sfxVolume = 1f;
        [Range(0f, 1f)] public float bgmVolume = 1f;

        private void Awake()
        {
            // 单例初始化
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 初始化音量
            bgmSource.volume = bgmVolume;
        }

        // 播放音效（通用）
        public void PlaySfx(string name)
        {
            AudioClip clip = Resources.Load<AudioClip>($"Sounds/{name}");
            if (clip == null) return;
            sfxSource.clip = clip;
            sfxSource.volume = sfxVolume;
            sfxSource.Play();
        }
        
        // 播放背景音乐
        public void PlayBGM(string name, bool loop = true)
        {
            AudioClip clip = Resources.Load<AudioClip>($"Sounds/{name}");
            if (clip == null) return;
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }

        // 开关音效/背景音乐
        public void ToggleBGM(bool isOn) => bgmSource.mute = !isOn;

        // 调节音量
        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            sfxSource.volume = sfxVolume;
        }

        public void SetBgmVolume(float volume)
        {
            bgmVolume = Mathf.Clamp01(volume);
            bgmSource.volume = bgmVolume;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
    // 控制音效播放的第二个Audio Source
    public AudioSource efxSource;
    public AudioSource musicSource;
    // 音量
    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    // 采用单例模式 避免音乐在切换场景时重置
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void playSingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        efxSource.clip = clips[randomIndex];
        efxSource.pitch = randomPitch;
        efxSource.Play();
    }
}

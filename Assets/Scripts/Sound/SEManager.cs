using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SoundEffect
{
    public string key;
    public AudioClip clip;
}

public class SEManager : MonoBehaviour
{
    [SerializeField]
    List<SoundEffect> effect;
    List<AudioSource> audioPool;

    public static SEManager instance;

    Dictionary<string, AudioClip> se_map;

    // **마스터 볼륨**
    public float masterVolume = 1f;
    // Use this for initialization
    void Awake ()
    {
        if(instance == null)
        {
            instance = this;
            audioPool = new List<AudioSource>();

            for (int i = 0; i < 5; i++)
            {
                var src = gameObject.AddComponent<AudioSource>();
                src.playOnAwake = false;
                audioPool.Add(src);
            }

            se_map = new Dictionary<string, AudioClip>();
            foreach(var e in effect)
            {
                se_map[e.key] = e.clip;
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DestroySEManager()
    {
        Destroy(gameObject);
    }
	
    public void PlaySE(string key)
    {
        if (se_map.ContainsKey(key) == false)
        {
            Debug.LogError(key + " SE가 SEManager에 없습니다.");
        }
        else
        {
            PlaySound(se_map[key]);
        }
    }

    public void LoopPlaySE(string key)
    {
        if (se_map.ContainsKey(key) == false)
        {
            Debug.LogError(key + " SE가 SEManager에 없습니다.");
        }
        else
        {
            LoopPlaySound(se_map[key]);
        }
    }

    public void StopSE(string key)
    {
        if (!se_map.ContainsKey(key))
            Debug.LogError($"{key} SE가 없습니다");
        else
            StopSound(se_map[key]);
    }

    void PlaySound(AudioClip clip)
    {
        var AS = GetOrCreateSource(false);
        AS.clip = clip;
        AS.loop = false;
        AS.volume = masterVolume;
        AS.Play();
    }

    // 이쪽 문제 있는거 확인하기
    void LoopPlaySound(AudioClip clip)
    {
        var AS = GetOrCreateSource(true);
        AS.clip = clip;
        AS.loop = true;
        AS.volume = masterVolume;
        AS.Play();
    }

    void StopSound(AudioClip clip)
    {
        // clip 과 동일하게 재생 중인 AudioSource 만 찾는다.
        foreach (var AS in audioPool)
        {
            if (AS.isPlaying && AS.clip == clip)
            {
                AS.loop = false;
                AS.Stop();
                break;
            }
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        // 이미 재생 중인 AudioSource의 볼륨도 변경
        foreach (var AS in audioPool)
        {
            if (AS.isPlaying)
            {
                AS.volume = masterVolume;
            }
        }
    }

    // 재사용 혹은 새로 생성하는 헬퍼
    AudioSource GetOrCreateSource(bool shouldLoop)
    {
        // 재생 중이지 않은 소스가 있으면 그걸 쓰고
        foreach (var src in audioPool)
        {
            if (!src.isPlaying)
                return src;
        }
            
        // 없으면 새로 하나 만들어서 풀에 추가
        var extra = gameObject.AddComponent<AudioSource>();
        extra.playOnAwake = false;
        audioPool.Add(extra);
        return extra;
    }
}

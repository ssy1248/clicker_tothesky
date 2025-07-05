using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SoundEffect
{
    public string key;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
}

public class SEManager : MonoBehaviour
{
    [SerializeField]
    List<SoundEffect> effect;
    List<AudioSource> audioPool;

    public static SEManager instance;

    Dictionary<string, SoundEffect> se_map;

    // 마스터 볼륨
    [Range(0f, 1f)]
    public float masterVolume = 1f;

    void Awake ()
    {
        if (instance == null)
        {
            instance = this;
            audioPool = new List<AudioSource>();

            for (int i = 0; i < 5; i++)
            {
                var src = gameObject.AddComponent<AudioSource>();
                src.playOnAwake = false;
                audioPool.Add(src);
            }

            // --- 변경점 2: Dictionary에 SoundEffect 객체 전체를 저장 ---
            se_map = new Dictionary<string, SoundEffect>();
            foreach (var e in effect)
            {
                // AudioClip만이 아닌, SoundEffect 객체(e) 자체를 저장합니다.
                se_map[e.key] = e;
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
            // 재생 메소드에 SoundEffect 객체 전체를 전달합니다.
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
            // 재생 메소드에 SoundEffect 객체 전체를 전달합니다.
            LoopPlaySound(se_map[key]);
        }
    }

    public void StopSE(string key)
    {
        if (!se_map.ContainsKey(key))
            Debug.LogError($"{key} SE가 없습니다");
        else
            // 사운드를 멈출 때는 클립 정보만 있어도 충분합니다.
            StopSound(se_map[key].clip);
    }

    // --- 변경점 3: PlaySound가 SoundEffect 객체를 인자로 받도록 수정 ---
    void PlaySound(SoundEffect sfx)
    {
        var AS = GetOrCreateSource(false);
        AS.clip = sfx.clip;
        AS.loop = false;
        // 볼륨을 마스터 볼륨과 개별 볼륨의 곱으로 설정합니다.
        AS.volume = masterVolume * sfx.volume;
        AS.Play();
    }

    // --- 변경점 4: LoopPlaySound도 SoundEffect 객체를 인자로 받도록 수정 ---
    void LoopPlaySound(SoundEffect sfx)
    {
        var AS = GetOrCreateSource(true);
        AS.clip = sfx.clip;
        AS.loop = true;
        // 볼륨을 동일한 방식으로 계산합니다.
        AS.volume = masterVolume * sfx.volume;
        AS.Play();
    }

    void StopSound(AudioClip clip)
    {
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

    // --- 변경점 5: SetMasterVolume이 재생 중인 사운드 볼륨을 올바르게 재계산하도록 수정 ---
    public void SetMasterVolume(float volume)
    {
        // masterVolume 값이 0과 1 사이를 벗어나지 않도록 합니다.
        masterVolume = Mathf.Clamp01(volume);

        // 마스터 볼륨이 변경되면, 현재 재생 중인 모든 사운드의 볼륨을 업데이트합니다.
        foreach (var AS in audioPool)
        {
            if (AS.isPlaying)
            {
                // 현재 재생 중인 클립에 해당하는 원본 SoundEffect를 찾습니다.
                SoundEffect playingSfx = FindSfxByClip(AS.clip);
                if (playingSfx != null)
                {
                    // 새로운 마스터 볼륨을 적용하여 볼륨을 다시 계산합니다.
                    AS.volume = masterVolume * playingSfx.volume;
                }
            }
        }
    }

    // 재생 중인 클립으로 원본 SoundEffect를 찾기 위한 헬퍼 함수
    private SoundEffect FindSfxByClip(AudioClip clip)
    {
        // Linq를 사용해 간결하게 검색합니다.
        return effect.FirstOrDefault(sfx => sfx.clip == clip);
    }

    // 이 헬퍼 함수는 변경되지 않았습니다.
    AudioSource GetOrCreateSource(bool shouldLoop)
    {
        foreach (var src in audioPool)
        {
            if (!src.isPlaying)
                return src;
        }

        var extra = gameObject.AddComponent<AudioSource>();
        extra.playOnAwake = false;
        audioPool.Add(extra);
        return extra;
    }
}

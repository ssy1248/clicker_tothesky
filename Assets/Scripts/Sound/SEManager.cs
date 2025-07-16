using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// 사운드 타입을 구분하기 위한 열거형
public enum SoundType
{
    SE, // 효과음
    BGM // 배경음
}

[System.Serializable]
public class SoundEffect
{
    public string key;
    public AudioClip clip;
    public SoundType soundType; // 이 사운드의 타입을 지정 (SE 또는 BGM)
    [Range(0f, 1f)]
    public float volume = 1f;
}

public class SEManager : MonoBehaviour
{
    [SerializeField]
    List<SoundEffect> effectList;
    List<AudioSource> audioPool;

    public static SEManager instance;

    Dictionary<string, SoundEffect> se_map;

    [Range(0f, 1f)]
    public float seVolume = 1f; // 효과음 전체 볼륨
    [Range(0f, 1f)]
    public float bgmVolume = 1f; // 배경음 전체 볼륨

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

            se_map = new Dictionary<string, SoundEffect>();
            foreach (var e in effectList) // 리스트 이름 변경
            {
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

    // 3: PlaySound가 SoundEffect 객체를 인자로 받도록 수정
    void PlaySound(SoundEffect sfx)
    {
        if (sfx == null || sfx.clip == null)
        {
            // Key가 유효해도 Clip이 없으면 경고를 남기고 넘어갑니다.
            if (sfx != null) Debug.LogWarning(sfx.key + " 키의 오디오 클립이 비어있습니다.");
            return; // 여기서 함수 실행을 중단
        }

        var AS = GetOrCreateSource(false);
        AS.clip = sfx.clip;
        AS.loop = false;

        // 사운드 타입에 맞는 카테고리 볼륨을 가져옵니다.
        float categoryVolume = (sfx.soundType == SoundType.BGM) ? bgmVolume : seVolume;
        // 최종 볼륨 = 카테고리 볼륨 * 개별 사운드 볼륨
        AS.volume = categoryVolume * sfx.volume;

        AS.Play();
    }

    // 4: LoopPlaySound도 SoundEffect 객체를 인자로 받도록 수정
    void LoopPlaySound(SoundEffect sfx)
    {
        if (sfx == null || sfx.clip == null)
        {
            if (sfx != null) Debug.LogWarning(sfx.key + " 키의 오디오 클립이 비어있습니다. (루프)");
            return; // 여기서 함수 실행을 중단
        }


        var AS = GetOrCreateSource(true);
        AS.clip = sfx.clip;
        AS.loop = true;

        // ▼▼▼ 볼륨 계산 로직 수정 ▼▼▼
        float categoryVolume = (sfx.soundType == SoundType.BGM) ? bgmVolume : seVolume;
        AS.volume = categoryVolume * sfx.volume;

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

    /// <summary>
    /// 효과음(SE)의 전체 볼륨을 조절합니다. 옵션 슬라이더 등에서 호출합니다.
    /// </summary>
    public void SetSEVolume(float volume)
    {
        seVolume = Mathf.Clamp01(volume);
        UpdateAllPlayingSoundsVolume();
    }

    /// <summary>
    /// 배경음(BGM)의 전체 볼륨을 조절합니다. 옵션 슬라이더 등에서 호출합니다.
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        UpdateAllPlayingSoundsVolume();
    }

    // 현재 재생중인 모든 사운드의 볼륨을 새 설정에 맞게 업데이트하는 헬퍼 함수
    private void UpdateAllPlayingSoundsVolume()
    {
        foreach (var AS in audioPool)
        {
            if (AS.isPlaying)
            {
                SoundEffect playingSfx = FindSfxByClip(AS.clip);
                if (playingSfx != null)
                {
                    // 타입에 맞는 카테고리 볼륨을 다시 적용
                    float categoryVolume = (playingSfx.soundType == SoundType.BGM) ? bgmVolume : seVolume;
                    AS.volume = categoryVolume * playingSfx.volume;
                }
            }
        }
    }

    // 재생 중인 클립으로 원본 SoundEffect를 찾기 위한 헬퍼 함수
    private SoundEffect FindSfxByClip(AudioClip clip)
    {
        // Linq를 사용해 간결하게 검색합니다.
        return effectList.FirstOrDefault(sfx => sfx.clip == clip);
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

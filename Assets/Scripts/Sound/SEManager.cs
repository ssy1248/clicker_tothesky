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
    [SerializeField] 
    private float masterVolume = 1f; // 전체 음량
    [Range(0f, 1f)]
    [SerializeField]
    private float seVolume = 1f; // 효과음 전체 볼륨
    [Range(0f, 1f)]
    [SerializeField]
    private float bgmVolume = 1f; // 배경음 전체 볼륨

    // PlayerPrefs 키
    const string KEY_MASTER = "vol_master";
    const string KEY_SE = "vol_se";
    const string KEY_BGM = "vol_bgm";

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

            // 저장된 볼륨 불러오기
            LoadVolumes();
            // 혹시 에디터에서 변경한 경우 대비 일괄 적용
            UpdateAllPlayingSoundsVolume();

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
        if (!se_map.TryGetValue(key, out var sfx)) { 
            Debug.LogError($"{key} SE가 없습니다."); 
            return; 
        }

        PlaySound(sfx);
    }

    public void LoopPlaySE(string key)
    {
        if (!se_map.TryGetValue(key, out var sfx)) { 
            Debug.LogError($"{key} SE가 없습니다.");
            return; 
        }

        LoopPlaySound(sfx);
    }

    public void StopSE(string key)
    {
        if (!se_map.TryGetValue(key, out var sfx)) { 
            Debug.LogError($"{key} SE가 없습니다."); 
            return; 
        }

        StopSound(sfx.clip);
    }

    // 3: PlaySound가 SoundEffect 객체를 인자로 받도록 수정
    void PlaySound(SoundEffect sfx)
    {
        if (sfx == null || sfx.clip == null) { 
            if (sfx != null) 
                Debug.LogWarning($"{sfx.key} 클립 없음."); 
            return; 
        }

        var src = GetOrCreateSource(false);
        src.clip = sfx.clip;
        src.loop = false;
        src.volume = CalculateEffectiveVolume(sfx);
        src.Play();
    }

    // 4: LoopPlaySound도 SoundEffect 객체를 인자로 받도록 수정
    void LoopPlaySound(SoundEffect sfx)
    {
        if (sfx == null || sfx.clip == null) { 
            if (sfx != null) 
                Debug.LogWarning($"{sfx.key} 클립 없음(루프).");
            return; 
        }

        var src = GetOrCreateSource(true);
        src.clip = sfx.clip;
        src.loop = true;
        src.volume = CalculateEffectiveVolume(sfx);
        src.Play();
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

    // 실효 볼륨 계산: Master × Category × 개별클립
    float CalculateEffectiveVolume(SoundEffect sfx)
    {
        float category = (sfx.soundType == SoundType.BGM) ? bgmVolume : seVolume;
        return masterVolume * category * sfx.volume;
    }

    void UpdateAllPlayingSoundsVolume()
    {
        foreach (var src in audioPool)
        {
            if (!src.isPlaying || src.clip == null) 
                continue;

            var sfx = FindSfxByClip(src.clip);

            if (sfx != null) 
                src.volume = CalculateEffectiveVolume(sfx);
        }
    }

    SoundEffect FindSfxByClip(AudioClip clip)
        => effectList.FirstOrDefault(sfx => sfx.clip == clip);

    AudioSource GetOrCreateSource(bool shouldLoop)
    {
        foreach (var src in audioPool) 
            if (!src.isPlaying) 
                return src;
        var extra = gameObject.AddComponent<AudioSource>();
        extra.playOnAwake = false;
        audioPool.Add(extra);
        return extra;
    }

    /* ---------- 저장/불러오기 ---------- */
    void SaveVolumes()
    {
        PlayerPrefs.SetFloat(KEY_MASTER, masterVolume);
        PlayerPrefs.SetFloat(KEY_SE, seVolume);
        PlayerPrefs.SetFloat(KEY_BGM, bgmVolume);
        PlayerPrefs.Save();
    }

    void LoadVolumes()
    {
        if (PlayerPrefs.HasKey(KEY_MASTER)) masterVolume = PlayerPrefs.GetFloat(KEY_MASTER, 1f);
        if (PlayerPrefs.HasKey(KEY_SE)) seVolume = PlayerPrefs.GetFloat(KEY_SE, 1f);
        if (PlayerPrefs.HasKey(KEY_BGM)) bgmVolume = PlayerPrefs.GetFloat(KEY_BGM, 1f);
    }

    public void SetMasterVolume(float v)
    {
        masterVolume = Mathf.Clamp01(v);
        SaveVolumes();
        UpdateAllPlayingSoundsVolume();
    }
    public void SetSEVolume(float v)
    {
        seVolume = Mathf.Clamp01(v);
        SaveVolumes();
        UpdateAllPlayingSoundsVolume();
    }
    public void SetBGMVolume(float v)
    {
        bgmVolume = Mathf.Clamp01(v);
        SaveVolumes();
        UpdateAllPlayingSoundsVolume();
    }

    // UI 초기화용 Getter
    public float GetMasterVolume() => masterVolume;
    public float GetSEVolume() => seVolume;
    public float GetBGMVolume() => bgmVolume;
}

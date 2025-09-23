using System.Collections.Generic;
using UnityEngine;

public class SubTouchManager : MonoBehaviour
{
    public static SubTouchManager Instance { get; private set; }

    [Header("미니게임 프리팹")]
    public GameObject quickTapPrefab;
    public GameObject touchPrefab;
    public GameObject memoryPrefab;

    [Header("스폰 옵션")]
    public Transform spawnParent;
    [Min(0.05f)] public float spawnIntervalMin = 1.0f;
    [Min(0.05f)] public float spawnIntervalMax = 2.5f;
    [Min(1)] public int maxConcurrent = 1;      // 동시에 몇 개까지
    public bool randomPickAmongConfigs = true;       // 구성 중 랜덤 선택

    // 런타임 상태
    private List<MiniGameConfig> _configs;
    private float _spawnChance;              // 0~100
    private int _spawnsRemaining;            // 남은 총 스폰 횟수
    private float _nextSpawnAt;
    private int _concurrent;                 // 현재 활성 개수
    private bool _running;

    private void Awake()
    {
        if (Instance == null) { 
            Instance = this; 
        } else { 
            Destroy(gameObject); 
        }
    }

    /// <summary>
    /// 스테이지 시작 시 호출: 해당 스테이지의 스폰 정책 세팅
    /// </summary>
    public void BeginStage(StageData stage)
    {
        _configs = stage.miniGames != null ? new List<MiniGameConfig>(stage.miniGames) : new List<MiniGameConfig>();
        _spawnsRemaining = Mathf.Max(0, stage.miniGameSpawnCount);
        _spawnChance = Mathf.Clamp(stage.miniGameSpawnPercentage, 0f, 100f);
        _concurrent = 0;
        _running = _spawnsRemaining > 0 && _configs.Count > 0;
        _nextSpawnAt = Time.time + Random.Range(spawnIntervalMin, spawnIntervalMax);
    }

    /// <summary>스테이지 종료/일시정지 등</summary>
    public void StopAllMiniGames()
    {
        _running = false;
        foreach (Transform child in spawnParent)
        {
            var st = child.GetComponent<SubTouch>();
            if (st != null) Destroy(child.gameObject);
        }
        _concurrent = 0;
    }

    void Update()
    {
        if (!_running) 
            return;

        if (Time.time < _nextSpawnAt) 
            return;
        _nextSpawnAt = Time.time + Random.Range(spawnIntervalMin, spawnIntervalMax);

        // 더 이상 못 뽑거나 꽉 찼으면 리턴
        if (_spawnsRemaining <= 0) { 
            _running = false; 
            return; 
        }
        if (_concurrent >= maxConcurrent)
            return;

        // 확률 체크
        float roll = Random.Range(0f, 100f);
        if (roll > _spawnChance) 
            return;

        // 어떤 미니게임을 스폰할지 결정
        var cfg = PickConfig();
        if (cfg == null) 
            return;

        SpawnActualMiniGame(cfg.Value);
        _spawnsRemaining--;
        if (_spawnsRemaining <= 0) _running = false;
    }

    private MiniGameConfig? PickConfig()
    {
        if (_configs == null || _configs.Count == 0) 
            return null;
        if (!randomPickAmongConfigs) 
            return _configs[0]; // 첫 번째 고정

        int i = Random.Range(0, _configs.Count);
        return _configs[i];
    }

    private void SpawnActualMiniGame(MiniGameConfig cfg)
    {
        var prefab = GetPrefab(cfg.type);
        if (prefab == null) { 
            Debug.LogWarning($"프리팹 없음: {cfg.type}"); 
            return; 
        }

        var go = Instantiate(prefab, spawnParent);
        var st = go.GetComponent<SubTouch>();
        if (st == null) {
            Debug.LogWarning("SubTouch 파생 컴포넌트가 필요"); Destroy(go); 
            return; 
        }

        _concurrent++; // 활성 증가
        st.Initialize(cfg.successScore, () => { _concurrent = Mathf.Max(0, _concurrent - 1); });
    }

    private GameObject GetPrefab(SubTouchType type)
    {
        return type switch
        {
            SubTouchType.QuickTap => quickTapPrefab,
            SubTouchType.Touch => touchPrefab,
            SubTouchType.Memory => memoryPrefab,
            _ => null
        };
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 수집품 개별 정보 클래스
[System.Serializable]
public class CollectedItemInfo
{
    public int stageNumber;
    public int itemId;
}

public class GlobalVariable : MonoBehaviour
{
    // 씬이동이 있기에 게임씬에서 가져가야할 변해야하는 수 모음

    // 싱글톤 사용 이유 - 유일성 보장, 전역 접근성, 수명관리
    public static GlobalVariable Instance { get; private set; }

    [Header("체크포인트 관련 변수")]
    // 체크포인트 거리
    public int CheckPointDistance = 50;

    [Header("플레이어 관련 변수")]
    public int PlayerCurrentDistance = 0;
    public int PlayerCurrentPlayerStage = 0;

    [Header("수집품 관련 변수")]
    public List<CollectedItemInfo> collectedItems = new();
    // 진행하는 스테이지의 수집품 갯수
    public int StageMaxCollectCount = 0;
    // 획득한 수집품의 갯수
    public int TotalGetCollectCount = 0;

    [Header("게임 흐름 플래그")]
    public bool GameStarted = false;
    public int ShopCount = 0;

    [Header("클리어 챕터 관련 변수")]
    public int PlayerClearRound = 0;

    [Header("엔딩 관련 변수")]
    public int EndingParameter = 0;
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Awake()
    {
        // 같은 오브젝트가 존재한다면 파괴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 처음 생성된 인스턴스라면 등록하고 씬 전환 시 파괴되지 않도록 설정
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainScene")
            return;

        CollectManager.Instance.CollectObejctCount = StageMaxCollectCount;
    }

    // 상점에서 돌아올 때 호출
    public void EnteredFromShop() => ShopCount++;

    public void StartedGame() => GameStarted = true;

    // 중복 수집 관련 로직
    public bool HasCollected(int stage, int itemId)
    {
        return collectedItems.Exists(item => item.stageNumber == stage && item.itemId == itemId);
    }

    // 수집 했을 떄 반영할 함수
    public void CollectItem(int stage, int itemId)
    {
        if (!HasCollected(stage, itemId))
        {
            collectedItems.Add(new CollectedItemInfo { stageNumber = stage, itemId = itemId });
            TotalGetCollectCount++;
        }
    }

    // 스테이지별 수집 갯수 확인 함수
    public int GetCollectedCountByStage(int stage)
    {
        return collectedItems.FindAll(item => item.stageNumber == stage).Count;
    }
}


/*
스테이지   클리어 거리
1   110
2   162
3   214
4   266
5   318
6   370
 */
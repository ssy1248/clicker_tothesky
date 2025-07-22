using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// 수집품 개별 정보 클래스
[System.Serializable]
public class CollectedItemInfo
{
    public int stageNumber;
    public int itemId;
}

[System.Serializable]
public class GameData
{
    public int playerClearRound;
    public List<CollectedItemInfo> collectedItems;
    public bool hasStartedGameBefore;
    // 여기에 게임 종료 후에도 저장하고 싶은 다른 변수들을 추가할 수 있습니다.
}

public class GlobalVariable : MonoBehaviour
{
    // 씬이동이 있기에 게임씬에서 가져가야할 변해야하는 수 모음

    // 싱글톤 사용 이유 - 유일성 보장, 전역 접근성, 수명관리
    public static GlobalVariable Instance { get; private set; }

    [Header("체크포인트 관련 변수")]
    // 체크포인트 거리
    public int CheckPointDistance = 50;

    [Header("게임 관련 변수")]
    public float GameTime = 0;
    public float LastClearTime = 0f; // 마지막으로 클리어한 시간 저장

    [Header("플레이어 관련 변수")]
    public int PlayerCurrentDistance = 0;
    public int PlayerCurrentPlayerStage = 0;

    [Header("수집품 관련 변수")]
    public List<CollectedItemInfo> collectedItems = new();
    // 진행하는 스테이지의 수집품 갯수
    public int StageMaxCollectCount = 0;
    // 획득한 수집품의 갯수
    public int TotalGetCollectCount = 0;
    // 획득 못한 수집품의 갯수
    public int LossCollectCount = 0;

    [Header("게임 흐름 플래그")]
    public bool GameStarted = false;
    public int ShopCount = 0;

    [Header("클리어 챕터 관련 변수")]
    public int PlayerClearRound = 0;

    [Header("엔딩 관련 변수")]
    public int EndingParameter = 0;

    [Header("게임 저장 관련")]
    public bool hasStartedGameBefore = false;
    private string saveFilePath;

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

        // 마지막에 저장/로드 관련 초기화 코드 추가
        saveFilePath = Path.Combine(Application.persistentDataPath, "gamedata.json");
        LoadGame();
    }

    // 게임을 종료할 때 자동으로 저장되도록 함
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void SaveGame()
    {
        // 1. 저장할 데이터만 모아서 GameData 객체를 만듭니다.
        GameData dataToSave = new GameData();
        dataToSave.playerClearRound = this.PlayerClearRound;
        dataToSave.collectedItems = this.collectedItems;
        dataToSave.hasStartedGameBefore = this.hasStartedGameBefore;

        // 2. GameData 객체를 JSON 문자열로 변환합니다.
        string json = JsonUtility.ToJson(dataToSave, true);

        // 3. 파일로 저장합니다.
        File.WriteAllText(saveFilePath, json);
        Debug.Log("게임 데이터가 저장되었습니다: " + saveFilePath);
    }

    public void LoadGame()
    {
        // 1. 저장 파일이 존재하는지 확인합니다.
        if (File.Exists(saveFilePath))
        {
            // 2. 파일에서 JSON 문자열을 읽어옵니다.
            string json = File.ReadAllText(saveFilePath);

            // 3. JSON 문자열을 GameData 객체로 변환합니다.
            GameData loadedData = JsonUtility.FromJson<GameData>(json);

            // 4. 불러온 데이터를 현재 GlobalVariable에 적용합니다.
            this.PlayerClearRound = loadedData.playerClearRound;
            this.collectedItems = loadedData.collectedItems;
            this.hasStartedGameBefore = loadedData.hasStartedGameBefore;

            Debug.Log("게임 데이터를 불러왔습니다.");
        }
        else
        {
            // 저장 파일이 없으면 '처음 실행'입니다.
            hasStartedGameBefore = false;
            Debug.Log("저장된 데이터가 없습니다. 새 게임을 시작합니다.");
        }
    }

    public void DeleteSaveData()
    {
        // 저장 파일이 실제로 존재할 때만 삭제를 시도합니다.
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);

            // 현재 실행 중인 게임의 변수들도 초기화해줍니다.
            hasStartedGameBefore = false;
            PlayerClearRound = 0;
            collectedItems.Clear();

            Debug.LogWarning("세이브 파일이 삭제되었습니다. 게임을 재시작하면 처음부터 시작합니다.");
            // 더 확실하게 하려면 씬을 다시 로드하는 것도 좋습니다.
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Debug.Log("삭제할 세이브 파일이 존재하지 않습니다.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainScene")
            return;

        CollectManager.Instance.CollectObejctCount = StageMaxCollectCount;
    }

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

    public void SetupStage(int stageIndex, ChapterDatabase db)
    {
        // 1. 통합 인덱스를 (챕터, 스테이지) 인덱스로 변환합니다.
        var indices = GetChapterStageFromFlatIndex(stageIndex, db);

        // 2. 유효한 스테이지 데이터를 찾았는지 확인합니다.
        if (indices.chapter != -1 && indices.stage != -1)
        {
            // 3. 변환된 인덱스를 사용해 정확한 스테이지 데이터를 가져옵니다.
            StageData data = db.allChapterData[indices.chapter].stagesInChapter[indices.stage];

            // 4. 플레이할 스테이지 정보 설정
            PlayerCurrentPlayerStage = stageIndex; // 전체 인덱스는 그대로 저장
            CheckPointDistance = data.clearDistance;
            //StageMaxCollectCount = data.maxCollectibles; // 수집품 개수 설정
            GameTime = data.gameTime;

            // 5. 게임 플레이와 직접 관련된 변수 초기화
            PlayerCurrentDistance = 0;
            LastClearTime = 0f;
            TotalGetCollectCount = 0;
            LossCollectCount = 0;
        }
        else
        {
            Debug.LogError($"ChapterDatabase에서 통합 인덱스 {stageIndex}에 해당하는 스테이지를 찾을 수 없습니다!");
        }
    }

    /// <summary>
    /// 전체 스테이지 기준의 통합 인덱스를 (챕터 인덱스, 해당 챕터 내의 스테이지 인덱스)로 변환합니다.
    /// </summary>
    private (int chapter, int stage) GetChapterStageFromFlatIndex(int flatIndex, ChapterDatabase db)
    {
        if (db == null) return (-1, -1); // 데이터베이스가 없으면 에러 반환

        int accumulatedStages = 0;
        for (int i = 0; i < db.allChapterData.Length; i++)
        {
            int stagesInThisChapter = db.allChapterData[i].stagesInChapter.Length;
            if (flatIndex < accumulatedStages + stagesInThisChapter)
            {
                // flatIndex가 현재 챕터 범위 내에 있으면, 올바른 (챕터, 스테이지) 인덱스를 반환
                return (i, flatIndex - accumulatedStages);
            }
            accumulatedStages += stagesInThisChapter;
        }

        return (-1, -1); // 모든 챕터를 찾아도 인덱스를 찾지 못하면 에러 반환
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
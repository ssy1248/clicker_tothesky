using UnityEngine;

public class SubTouchManager : MonoBehaviour
{
    public static SubTouchManager Instance { get; private set; }

    [Header("미니게임 프리팹")]
    public GameObject quickTapPrefab;
    public GameObject touchPrefab;
    public GameObject memoryPrefab;

    [Header("생성 위치")]
    public Transform spawnParent; // 미니게임이 생성될 부모 (Canvas 등)

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    /// <summary>
    /// 지정된 타입의 미니게임을 생성하고 시작합니다.
    /// </summary>
    public void SpawnMiniGame(SubTouchType type, int successScore)
    {
        GameObject prefabToSpawn = null;
        switch (type)
        {
            case SubTouchType.QuickTap:
                prefabToSpawn = quickTapPrefab;
                break;
            case SubTouchType.Touch:
                prefabToSpawn = touchPrefab;
                break;
            case SubTouchType.Memory:
                prefabToSpawn = memoryPrefab;
                break;
        }

        if (prefabToSpawn != null)
        {
            // 프리팹을 생성하고 부모를 지정
            GameObject miniGameObj = Instantiate(prefabToSpawn, spawnParent);

            // 생성된 오브젝트의 SubTouch 스크립트를 가져와 초기화
            SubTouch subTouch = miniGameObj.GetComponent<SubTouch>();
            if (subTouch != null)
            {
                subTouch.Initialize(successScore);
            }
        }
    }
}

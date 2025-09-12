using UnityEngine;

public class SubTouchManager : MonoBehaviour
{
    public static SubTouchManager Instance { get; private set; }

    [Header("미니게임 프리팹")]
    public GameObject quickTapPrefab;
    public GameObject touchPrefab;
    public GameObject memoryPrefab;

    [Header("활성화 오브젝트 프리팹")]
    public GameObject miniGameActivatorPrefab; 

    [Header("생성 위치")]
    public Transform spawnParent; // 미니게임이 생성될 부모 (Canvas 등)

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    /// <summary>
    /// 미니게임을 시작할 터치 오브젝트를 생성합니다. (GameModeManager가 호출)
    /// </summary>
    public void SpawnMiniGameActivator(SubTouchType type, int successScore)
    {
        if (miniGameActivatorPrefab != null)
        {
            // '계기' 오브젝트 프리팹을 생성합니다.
            GameObject activatorObj = Instantiate(miniGameActivatorPrefab, spawnParent);

            // 생성된 오브젝트의 스크립트를 가져와 초기화합니다.
            SubTouchActivator activator = activatorObj.GetComponent<SubTouchActivator>();
            if (activator != null)
            {
                // 어떤 미니게임을 시작할지, 성공 점수는 얼마인지,
                // 그리고 나중에 다시 호출할 자신(this)의 참조를 넘겨줍니다.
                activator.Initialize(type, successScore, this);
            }
        }
    }

    /// <summary>
    /// 실제 미니게임을 생성하고 시작합니다.
    /// </summary>
    public void SpawnActualMiniGame(SubTouchType type, int successScore)
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
            GameObject miniGameObj = Instantiate(prefabToSpawn, spawnParent);

            SubTouch subTouch = miniGameObj.GetComponent<SubTouch>();
            if (subTouch != null)
            {
                subTouch.Initialize(successScore);
            }
        }
    }
}

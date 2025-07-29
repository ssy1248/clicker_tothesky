using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CollectManager : MonoBehaviour
{
    public static CollectManager Instance { get; private set; }

    // 수집품 프리팹
    [SerializeField]
    public GameObject CollectObjectPrefab;
    // 스테이지당 생성될 오브젝트 갯수를 가져갈 변수
    [SerializeField]
    public int CollectObejctCount;

    [Header("게이지 연동")]
    public Image fillImage; // FillAmount 기반
    public RectTransform barBackground; // Fill의 부모인 바 영역

    private RectTransform spawnedCollectible;
    private Coroutine collectRoutine;

    private GuageManager guageManager;

    [Header("아이템 변수 관련")]
    public float CollectRangeModify = 0;

    // 수집품의 시작(하단)과 끝(상단) 비율을 저장할 변수들
    private float targetFillMin;
    private float targetFillMax;

    private void Awake()
    {
        Instance = this;

        guageManager = FindFirstObjectByType<GuageManager>();
        if (guageManager == null)
        {
            Debug.LogError("씬에서 GuageManager를 찾을 수 없습니다!");
        }
    }

    public GameObject CreateCollectObject(CollectScriptableObject collectData)
    {
        // 1. 위치 및 범위 계산
        float barHeight = barBackground.rect.height;
        float newMinY = -barHeight / 2f;
        float newMaxY = barHeight / 2f;
        float totalHeight = barHeight;
        RectTransform prefabRect = CollectObjectPrefab.GetComponent<RectTransform>();
        float collectibleFillHeight = prefabRect.rect.height / totalHeight;
        float centerFillValue = 1f - fillImage.fillAmount;

        //if (guageManager != null)
        //{
        //    centerFillValue = Mathf.Clamp(centerFillValue,
        //                                  guageManager.DANGER_THRESHOLD_LOW,
        //                                  guageManager.DANGER_THRESHOLD_HIGH);
        //}

        float originalRadius = collectibleFillHeight / 2f;
        float modifiedRadius = originalRadius + (CollectRangeModify / 2f);

        this.targetFillMin = centerFillValue - modifiedRadius;
        this.targetFillMax = centerFillValue + modifiedRadius;

        float spawnY = Mathf.Lerp(newMinY, newMaxY, centerFillValue);

        // 2. 좌표 변환
        Vector2 localPosInBar = new Vector2(0f, spawnY);
        Vector3 worldPosition = barBackground.transform.TransformPoint(localPosInBar);

        // -3. 수집품 생성 및 데이터 할당
        Transform parentOfBar = barBackground.transform.parent;
        GameObject obj = Instantiate(CollectObjectPrefab, parentOfBar);
        spawnedCollectible = obj.GetComponent<RectTransform>();

        Collectiable collectibleComponent = obj.GetComponent<Collectiable>();
        if (collectibleComponent != null)
        {
            collectibleComponent.data = collectData;
        }

        // 4. 위치/순서 지정
        spawnedCollectible.position = worldPosition;
        spawnedCollectible.SetAsLastSibling();

        // 5. 코루틴 시작
        if (collectRoutine != null)
        {
            StopCoroutine(collectRoutine);
        }
        collectRoutine = StartCoroutine(CheckOverlapRoutine());

        // 6. 생성된 게임 오브젝트 반환
        return obj;
    }

    private IEnumerator CheckOverlapRoutine()
    {
        float stayTime = 0f;
        float maxStayTime = 3f;
        float buffer = 0.02f; //범위 완화용 버퍼 추가

        Image collectBarImage = FindCollectBarImageFrom(spawnedCollectible);
        if (collectBarImage == null)
        {
            Debug.LogError("CollectBarImage를 찾을 수 없습니다.");
            yield break;
        }

        collectBarImage.fillAmount = 0f;

        while (spawnedCollectible != null)
        {
            float currentBarFill = fillImage.fillAmount;

            // 버퍼를 적용한 범위 판정
            if (currentBarFill >= (targetFillMin - buffer) && currentBarFill <= (targetFillMax + buffer))
            {
                stayTime += Time.deltaTime;
            }
            else
            {
                stayTime -= Time.deltaTime;
            }

            stayTime = Mathf.Clamp(stayTime, 0, maxStayTime);
            collectBarImage.fillAmount = stayTime / maxStayTime;

            if (stayTime >= maxStayTime)
            {
                // 1. 수집한 오브젝트에서 Collectible 컴포넌트와 데이터를 가져옵니다.
                Collectiable collectedComponent = spawnedCollectible.GetComponent<Collectiable>();
                if (collectedComponent != null && collectedComponent.data != null)
                {
                    // 2. GlobalVariable에 어떤 스테이지의 어떤 아이템을 수집했는지 기록합니다.
                    int stageNum = GlobalVariable.Instance.PlayerCurrentPlayerStage;
                    int itemId = collectedComponent.data.CollectId;
                    GlobalVariable.Instance.CollectItem(stageNum, itemId);
                }

                Destroy(spawnedCollectible.gameObject);
                spawnedCollectible = null;
                yield break;
            }

            yield return null;
        }
    }

    // 수집품 자식에서 차오르는 이미지를 찾는 함수
    private Image FindCollectBarImageFrom(RectTransform root)
    {
        foreach (var img in root.GetComponentsInChildren<Image>(true))
        {
            if (img.gameObject.name == "CollectBarImage")
                return img;
        }
        return null;
    }

    /// <summary>
    /// 현재 활성화된 수집품과 관련 코루틴을 강제로 종료하고 파괴합니다.
    /// </summary>
    public void DestroyCurrentCollectible()
    {
        if (spawnedCollectible != null)
        {
            Debug.Log("시간 초과 또는 다음 수집품 생성을 위해 기존 수집품을 파괴합니다.");

            // 현재 실행중인 코루틴을 반드시 정지
            if (collectRoutine != null)
            {
                StopCoroutine(collectRoutine);
            }

            // 게임 오브젝트 파괴
            Destroy(spawnedCollectible.gameObject);

            // 참조 변수들을 깨끗하게 초기화
            spawnedCollectible = null;
            collectRoutine = null;
        }
    }
}

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

    //[Header("생성 위치 설정")]
    //public float MaxY = 662f;
    //public float MinY = -440f;

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

    public void CreateCollectObject()
    {
        // --- 1. 위치 및 범위 계산 ---
        float barHeight = barBackground.rect.height;
        float newMinY = -barHeight / 2f;
        float newMaxY = barHeight / 2f;
        float totalHeight = barHeight;
        RectTransform prefabRect = CollectObjectPrefab.GetComponent<RectTransform>();
        float collectibleFillHeight = prefabRect.rect.height / totalHeight;
        float centerFillValue = 1f - fillImage.fillAmount;

        // GuageManager에서 안전지대 값을 가져와서 생성 위치 비율을 제한합니다.
        if (guageManager != null)
        {
            centerFillValue = Mathf.Clamp(centerFillValue,
                                          guageManager.DANGER_THRESHOLD_LOW,
                                          guageManager.DANGER_THRESHOLD_HIGH);
        }

        // ================== 아이템 효과 적용 부분 ==================
        // 기존 판정 범위의 절반(반지름)을 계산합니다.
        float originalRadius = collectibleFillHeight / 2f;

        // 아이템 효과를 적용하여 판정 범위 반지름을 수정합니다.
        // CollectRangeModify는 전체 범위 증가량이므로, 절반을 반지름에 더합니다.
        float modifiedRadius = originalRadius + (CollectRangeModify / 2f);

        // 수정된 반지름으로 최종 범위를 설정합니다.
        this.targetFillMin = centerFillValue - modifiedRadius;
        this.targetFillMax = centerFillValue + modifiedRadius;
        // ========================================================

        float spawnY = Mathf.Lerp(newMinY, newMaxY, centerFillValue);

        // --- 2. 좌표 변환 (두 번째 코드의 좌표 변환 로직) ---
        Vector2 localPosInBar = new Vector2(0f, spawnY);
        Vector3 worldPosition = barBackground.transform.TransformPoint(localPosInBar);

        // --- 3. 수집품 생성 및 위치/순서 지정 (두 번째 코드의 생성/순서 로직) ---
        Transform parentOfBar = barBackground.transform.parent;
        GameObject obj = Instantiate(CollectObjectPrefab, parentOfBar);
        spawnedCollectible = obj.GetComponent<RectTransform>();
        spawnedCollectible.position = worldPosition;
        spawnedCollectible.SetAsLastSibling();

        // --- 4. 코루틴 시작 ---
        if (collectRoutine != null)
        {
            StopCoroutine(collectRoutine);
        }
        collectRoutine = StartCoroutine(CheckOverlapRoutine());
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
                Debug.Log("수집 성공!");
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
}

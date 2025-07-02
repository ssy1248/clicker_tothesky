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
    //public Transform spawnParent; // 생성된 수집품의 부모 오브젝트

    private RectTransform spawnedCollectible;
    private Coroutine collectRoutine;

    // 수집품의 시작(하단)과 끝(상단) 비율을 저장할 변수들
    private float targetFillMin;
    private float targetFillMax;

    private void Awake()
    {
        Instance = this;
    }

    public void CreateCollectObject()
    {
        // --- 1. 모든 계산의 기준이 될 오른쪽 막대의 높이를 가져옴 ---
        float barHeight = barBackground.rect.height;
        // 오른쪽 막대의 로컬 Y좌표 최소/최대값 (Pivot이 (0.5, 0.5)일 경우)
        float newMinY = -barHeight / 2f;
        float newMaxY = barHeight / 2f;
        float totalHeight = barHeight; // MaxY - MinY == barHeight

        // --- 2. 수집품의 높이를 비율로 계산 ---
        RectTransform prefabRect = CollectObjectPrefab.GetComponent<RectTransform>();
        float collectibleFillHeight = prefabRect.rect.height / totalHeight;

        // --- 3. 수집품의 중심 비율과 범위 계산 (이전과 동일) ---
        float centerFillValue = 1f - fillImage.fillAmount;
        this.targetFillMin = centerFillValue - (collectibleFillHeight / 2f);
        this.targetFillMax = centerFillValue + (collectibleFillHeight / 2f);

        // --- 4. 수집품 생성 ---
        // Y좌표를 새로 계산된 Min/Max Y 기준으로 구함
        float spawnY = Mathf.Lerp(newMinY, newMaxY, centerFillValue);

        // 부모를 spawnParent가 아닌 barBackground로 변경
        GameObject obj = Instantiate(CollectObjectPrefab, barBackground.transform);
        spawnedCollectible = obj.GetComponent<RectTransform>();

        // X 좌표를 0으로 하여 부모(barBackground)의 중앙에 오도록 설정
        spawnedCollectible.anchoredPosition = new Vector2(0f, spawnY);

        spawnedCollectible.SetAsLastSibling();

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

            Debug.Log($"[Check] Bar Fill: {currentBarFill}, Target Range: [{targetFillMin}, {targetFillMax}]");

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

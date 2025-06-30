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

    [Header("생성 위치 설정")]
    public float MaxY = 662f;
    public float MinY = -440f;

    [Header("게이지 연동")]
    public Image fillImage; // FillAmount 기반
    public RectTransform barBackground; // Fill의 부모인 바 영역
    public Transform spawnParent; // 생성된 수집품의 부모 오브젝트

    private RectTransform spawnedCollectible;
    private Coroutine collectRoutine;

    private void Awake()
    {
        Instance = this;
    }

    public void CreateCollectObject()
    {
        float fill = fillImage.fillAmount;
        float oppositeFill = 1f - fill;

        float spawnY = Mathf.Lerp(MinY, MaxY, oppositeFill);

        GameObject obj = Instantiate(CollectObjectPrefab, spawnParent);
        spawnedCollectible = obj.GetComponent<RectTransform>();
        spawnedCollectible.anchoredPosition = new Vector2(0, spawnY);

        //  랜덤 Y 생성 방식
        /*
        float randY = Random.Range(MinY, MaxY);
        spawnedCollectible.anchoredPosition = new Vector2(0, randY);
        */

        // Update를 따로 두지 않고 시작할 때 충돌 체크 코루틴 시작
        StartCoroutine(CheckOverlapRoutine());
    }

    private IEnumerator CheckOverlapRoutine()
    {
        float stayTime = 0f;
        float maxStayTime = 3f;
        float successThreshold = 10f;

        // 안차오름

        // 자식에서 CollectBarImage를 찾음
        Image collectBarImage = spawnedCollectible.GetComponentInChildren<Image>(includeInactive: true);

        if (collectBarImage == null)
        {
            Debug.LogError("수집바 이미지(CollectBarImage)를 찾을 수 없습니다.");
            yield break;
        }

        collectBarImage.fillAmount = 0f;

        while (spawnedCollectible != null)
        {
            Vector3 barTipWorldPos = GetBarTipWorldPosition();
            float distance = Vector3.Distance(barTipWorldPos, spawnedCollectible.position);

            if (distance < successThreshold)
            {
                stayTime += Time.deltaTime;
                stayTime = Mathf.Clamp(stayTime, 0, maxStayTime);
            }
            else
            {
                stayTime -= Time.deltaTime;
                stayTime = Mathf.Clamp(stayTime, 0, maxStayTime);
            }

            // 수집 진행도 UI에 반영
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

    // 게이지 바 fillAmount 기준으로 현재 머리 위치를 구하는 함수
    private Vector3 GetBarTipWorldPosition()
    {
        float fill = fillImage.fillAmount;
        float y = Mathf.Lerp(MinY, MaxY, fill);
        Vector3 local = new Vector3(0f, y, 0f);
        return barBackground.TransformPoint(local);
    }
}

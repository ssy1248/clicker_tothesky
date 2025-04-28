using TMPro;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    // 게임 거리 / 무한모드 나중에 생성될 스토리 모드의 분기를 나눌 스크립트로 사용할 예정
    
    [Header("UI 모음")]
    // 거리를 나타낼 텍스트 변수
    [SerializeField]
    private TextMeshProUGUI GameDistanceText;

    [Header("변수 모음")]
    // 거리를 초기화할 초기 변수
    [SerializeField]
    int Distance = 0;
    // 1초를 누적할 타이머 변수
    private float distanceTimer = 0f;
    // 체크포인트 거리
    public int CheckPointDistance = 50;
    // 체크포인트를 넘어가기 위한 터치 횟수
    public int CheckPointTouch = 10;

    [Header("스프라이트 모음")]
    // 체크포인트 문 오브젝트 -> 문의 최종 크기는 x 2 y 2(스케일)
    [SerializeField]
    GameObject DoorObject;
    
    private void Awake()
    {
        if(GameDistanceText == null)
        {
            GameDistanceText = GameObject.Find("GameDistanceText").GetComponent<TextMeshProUGUI>();
        }
        if(DoorObject == null)
        {
            DoorObject = GameObject.Find("CheckPoint");
            DoorObject.SetActive(false);
        }

        UpdateDistanceText();
    }

    void Update()
    {
        IncreaseDistanceOverTime();
    }

    private void IncreaseDistanceOverTime()
    {
        distanceTimer += Time.deltaTime;

        if (distanceTimer >= 1f)
        {
            Distance += 1;
            distanceTimer -= 1f;

            UpdateDistanceText();
        }
    }

    private void UpdateDistanceText()
    {
        GameDistanceText.text = Distance.ToString() + " M";
    }

    private void CheckPonintDistanceCalculate(int distance)
    {

    }
}

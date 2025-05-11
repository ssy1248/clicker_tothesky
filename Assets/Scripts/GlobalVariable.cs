using UnityEngine;

public class GlobalVariable : MonoBehaviour
{
    // 씬이동이 있기에 게임씬에서 가져가야할 변해야하는 수 모음

    // 싱글톤 사용 이유 - 유일성 보장, 전역 접근성, 수명관리
    public static GlobalVariable Instance { get; private set; }

    [Header("체크포인트 관련 변수")]
    // 체크포인트 거리
    public int CheckPointDistance = 50;
    // 체크포인트 통과하기 위한 터치 횟수
    public int CheckPointTouchCount = 10;

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
}

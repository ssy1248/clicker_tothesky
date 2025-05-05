using UnityEngine;

public class GlobalVariable : MonoBehaviour
{
    // 씬이동이 있기에 게임씬에서 가져가야할 변해야하는 수 모음

    // 체크포인트 거리
    public int CheckPointDistance;
    // 체크포인트 통과하기 위한 터치 횟수
    public int CheckPointTouchCount;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}

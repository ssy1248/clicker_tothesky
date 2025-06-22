using UnityEngine;

public class StageButtonManager : MonoBehaviour
{
    // 스테이지 버튼을 클릭하면서 글로벌 오브젝트에게 값 전달
    public void SetStageValue(int stageDistance)
    {
        GlobalVariable.Instance.CheckPointDistance = stageDistance;
    }
}

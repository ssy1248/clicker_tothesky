using UnityEngine;

public class CollectManager : MonoBehaviour
{
    public static CollectManager Instance { get; private set; }

    // 수집품 프리팹
    [SerializeField]
    private GameObject CollectObejct;

    // 스테이지당 생성될 오브젝트 갯수를 가져갈 변수
    [SerializeField]
    public int CollectObejctCount;

    public float MaxY = 662;
    public float MinY = -440;

    private void Awake()
    {
        Instance = this;
    }

    public void CreateCollectObject()
    {

    }
}

using TMPro;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    // ���� �Ÿ� / ���Ѹ�� ���߿� ������ ���丮 ����� �б⸦ ���� ��ũ��Ʈ�� ����� ����
    
    [Header("UI ����")]
    // �Ÿ��� ��Ÿ�� �ؽ�Ʈ ����
    [SerializeField]
    private TextMeshProUGUI GameDistanceText;

    [Header("���� ����")]
    // �Ÿ��� �ʱ�ȭ�� �ʱ� ����
    [SerializeField]
    int Distance = 0;
    // 1�ʸ� ������ Ÿ�̸� ����
    private float distanceTimer = 0f;
    // üũ����Ʈ �Ÿ�
    public int CheckPointDistance = 50;
    // üũ����Ʈ�� �Ѿ�� ���� ��ġ Ƚ��
    public int CheckPointTouch = 10;

    [Header("��������Ʈ ����")]
    // üũ����Ʈ �� ������Ʈ -> ���� ���� ũ��� x 2 y 2(������)
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

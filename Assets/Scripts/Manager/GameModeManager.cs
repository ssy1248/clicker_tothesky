using TMPro;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    // ���� �Ÿ� / ���Ѹ�� ���߿� ������ ���丮 ����� �б⸦ ���� ��ũ��Ʈ�� ����� ����
    
    [Header("UI ����")]
    // �Ÿ��� ��Ÿ�� �ؽ�Ʈ ����
    [SerializeField]
    private TextMeshProUGUI GameDistanceText;
    private GameViewManager gameViewManager;

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
    // üũ����Ʈ ���� �÷���
    private bool isAtCheckpoint = false;
    // ���� ��ġ ī��Ʈ
    private int currentTouchCount = 0;      

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
        if(gameViewManager == null)
        {
            gameViewManager = GameObject.Find("Manager").GetComponent<GameViewManager>();
        }

        UpdateDistanceText();
    }

    void Update()
    {
        if (!isAtCheckpoint)
            // �Ÿ� ���� ����
            IncreaseDistanceOverTime();   
        else
            // üũ����Ʈ ��ġ ���
            HandleCheckpointTouch();     
    }

    private void IncreaseDistanceOverTime()
    {
        distanceTimer += Time.deltaTime;

        if (distanceTimer >= 1f)
        {
            Distance += 1;
            distanceTimer -= 1f;

            UpdateDistanceText();

            // üũ����Ʈ ���� üũ
            if (Distance >= CheckPointDistance)
                EnterCheckpoint();
        }
    }

    // üũ����Ʈ ���� ó��
    private void EnterCheckpoint()
    {
        isAtCheckpoint = true;
        DoorObject.SetActive(true);
    }

    private void UpdateDistanceText()
    {
        GameDistanceText.text = Distance.ToString() + " M";
    }

    // üũ����Ʈ ������ ���� ��ġ �Է� ���
    private void HandleCheckpointTouch()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            currentTouchCount++;
            Debug.Log($"Checkpoint Touch: {currentTouchCount}/{CheckPointTouch}");
        }
        #else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            currentTouchCount++;
            Debug.Log($"Checkpoint Touch: {currentTouchCount}/{CheckPointTouch}");
        }
        #endif

        // ��ġ Ƚ�� ���� �� ���� üũ����Ʈ �غ�
        if (currentTouchCount >= CheckPointTouch)
            ExitCheckpoint();
    }

    // üũ����Ʈ ���� & ���� �ܰ� ����
    private void ExitCheckpoint()
    {
        // �Ÿ��� ��ġ �䱸�� 2��� ����
        CheckPointDistance *= 2;
        CheckPointTouch *= 2;

        // ���� �ʱ�ȭ
        currentTouchCount = 0;
        isAtCheckpoint = false;
        DoorObject.SetActive(false);
        distanceTimer = 0f;

        // ���� �ð� �ʱ�ȭ��Ű�� üũ����Ʈ�� ������ �ϸ� �ִϸ��̼��� ����� �ҵ�
        gameViewManager.ResetTimer(120);

        Debug.Log($"Next CheckPoint: Distance at {CheckPointDistance}, TouchCount {CheckPointTouch}");
    }
}

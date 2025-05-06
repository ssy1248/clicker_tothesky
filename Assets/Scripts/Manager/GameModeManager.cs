using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameModeManager : MonoBehaviour
{
    // ���� �Ÿ� / ���Ѹ�� ���߿� ������ ���丮 ����� �б⸦ ���� ��ũ��Ʈ�� ����� ����
    
    [Header("UI ����")]
    // �Ÿ��� ��Ÿ�� �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI GameDistanceText;
    // üũ����Ʈ �Ÿ��� ������ �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI CheckPointDistanceText;
    [SerializeField]
    private Image FilledImage;
    [SerializeField]
    private Image CharacterImage;
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

    [Header("��������Ʈ ���� & ������Ʈ ����")]
    // üũ����Ʈ �� ������Ʈ -> ���� ���� ũ��� x 2 y 2(������)
    [SerializeField]
    GameObject DoorObject;

    // �ִϸ��̼ǿ� ����
    [Header("�ִϸ��̼� ����")]
    [SerializeField, Range(0f, 1f)]
    private float doorOpenThreshold = 0.8f;  // üũ����Ʈ �Ÿ��� �� �ۼ�Ʈ���� �� ���� ����
    private bool hasDoorOpenStarted = false;

    // ���� �� ������
    private Vector3 doorOriginalScale;
    private Vector3 doorOriginalPosition;
    private Vector3 doorStartScale;
    [SerializeField] 
    private Vector3 doorTargetScale = new Vector3(2f, 2f, 1f);

    private void Awake()
    {
        if(GameDistanceText == null)
        {
            GameDistanceText = GameObject.Find("GameDistanceText").GetComponent<TextMeshProUGUI>();
        }
        if (CheckPointDistanceText == null)
        {
            CheckPointDistanceText = GameObject.Find("CheckPointDistanceText").GetComponent<TextMeshProUGUI>();
        }
        if (DoorObject == null)
        {
            DoorObject = GameObject.Find("CheckPoint");
            DoorObject.SetActive(false);
        }

        // �ʱ� Ʈ������ �� ����
        doorOriginalScale = doorTargetScale;
        doorOriginalPosition = DoorObject.transform.localPosition;
        // �ʱ� ������
        doorStartScale = new Vector3(0.1f, 0.1f, doorOriginalScale.z);

        // üũ����Ʈ �Ÿ� �ؽ�Ʈ ����
        CheckPointDistanceText.text = CheckPointDistance.ToString() + " M";

        // �� ����
        DoorObject.SetActive(false);

        // GameViewManager
        gameViewManager = GameObject.Find("Manager").GetComponent<GameViewManager>();

        UpdateDistanceText();
    }

    void Update()
    {
        if (!isAtCheckpoint)
        {
            // �Ÿ� ���� ����
            IncreaseDistanceOverTime();
            // �Ÿ� ��� ������ ������Ʈ
            AnimateDoorScale();  
        }
        else
            // üũ����Ʈ ��ġ ���
            HandleCheckpointTouch();     
    }

    private void IncreaseDistanceOverTime()
    {
        distanceTimer += Time.deltaTime;
        if (distanceTimer >= 1f)
        {
            Distance++;
            distanceTimer -= 1f;
            UpdateDistanceText();

            // 80% �������� �� ���� ����
            float thresholdDistance = CheckPointDistance * doorOpenThreshold;
            if (!hasDoorOpenStarted && Distance >= thresholdDistance)
            {
                hasDoorOpenStarted = true;
                DoorObject.SetActive(true);
                DoorObject.transform.localScale = doorStartScale;
                DoorObject.transform.localPosition = doorOriginalPosition;
            }

            // üũ����Ʈ ���� �� ����
            if (Distance >= CheckPointDistance)
                EnterCheckpoint();
        }
    }

    // üũ����Ʈ ���� ó��
    private void EnterCheckpoint()
    {
        isAtCheckpoint = true;
        // �� �������� ��Ȯ�� ��ǥ �����Ϸ� ����
        DoorObject.transform.localScale = doorOriginalScale;
    }

    // �Ÿ��� ���� �� ������ ����
    private void AnimateDoorScale()
    {
        if (!hasDoorOpenStarted)
            return;

        float thresholdDist = CheckPointDistance * doorOpenThreshold;
        float progress = Mathf.Clamp01((Distance - thresholdDist) / (CheckPointDistance - thresholdDist));

        // doorTargetScale ���
        DoorObject.transform.localScale = Vector3.Lerp(doorStartScale, doorOriginalScale, progress);
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
        distanceTimer = 0f;
        hasDoorOpenStarted = false;

        // Ÿ�̸� ����
        gameViewManager.ResetTimer(120);


        Debug.Log($"Next CheckPoint: Distance at {CheckPointDistance}, TouchCount {CheckPointTouch}");
    }
}

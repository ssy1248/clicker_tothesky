using System.Collections;
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

    [Header("��������Ʈ ���� & ������Ʈ ����")]
    // üũ����Ʈ �� ������Ʈ -> ���� ���� ũ��� x 2 y 2(������)
    [SerializeField]
    GameObject DoorObject;

    // �ִϸ��̼ǿ� ����
    [Header("�ִϸ��̼� ����")]
    // ������ �ִϸ��̼� �ð�
    [SerializeField] 
    private float openDuration = 1f;
    // �ϰ� �ִϸ��̼� �ð�
    [SerializeField] 
    private float closeDuration = 1f;  

    // ���� �� ������
    private Vector3 doorOriginalScale;
    private Vector3 doorOriginalPosition;
    private Vector3 doorStartScale;

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

        // �ʱ� Ʈ������ �� ����
        doorOriginalScale = DoorObject.transform.localScale;
        doorOriginalPosition = DoorObject.transform.localPosition;
        // �ʱ� ������
        doorStartScale = new Vector3(0.1f, 0.1f, doorOriginalScale.z);

        // �� ����
        DoorObject.SetActive(false);

        // GameViewManager
        gameViewManager = GameObject.Find("Manager").GetComponent<GameViewManager>();

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

        // ���� �۰� ���� �� ���� �ִϸ��̼�
        DoorObject.transform.localScale = doorStartScale;
        DoorObject.transform.localPosition = doorOriginalPosition;

        // ������ �Ÿ��� �����ϸ� ���̴µ� 
        // �̰� ������� �ҵ�? -> üũ����Ʈ�� �Ÿ��� 80�ۼ�Ʈ�����ϸ� �׶����� ����?
        StartCoroutine(OpenDoorRoutine());
    }

    private IEnumerator OpenDoorRoutine()
    {
        float elapsed = 0f;
        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / openDuration);
            DoorObject.transform.localScale = Vector3.Lerp(
                doorStartScale, doorOriginalScale, t);
            yield return null;
        }
        DoorObject.transform.localScale = doorOriginalScale;
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

        // Ÿ�̸� ����
        gameViewManager.ResetTimer(120);

        // �ݱ� �ִϸ��̼� ����
        StartCoroutine(CloseDoorRoutine());

        Debug.Log($"Next CheckPoint: Distance at {CheckPointDistance}, TouchCount {CheckPointTouch}");
    }

    private IEnumerator CloseDoorRoutine()
    {
        float elapsed = 0f;
        Vector3 startPos = doorOriginalPosition;
        Vector3 endPos = new Vector3(
            startPos.x, -5f, startPos.z);

        while (elapsed < closeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / closeDuration);
            DoorObject.transform.localPosition =
                Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // ������ ���� �� ��Ȱ��ȭ �� ����
        DoorObject.transform.localPosition = endPos;
        DoorObject.SetActive(false);
        DoorObject.transform.localPosition = doorOriginalPosition;
        DoorObject.transform.localScale = doorOriginalScale;
    }
}

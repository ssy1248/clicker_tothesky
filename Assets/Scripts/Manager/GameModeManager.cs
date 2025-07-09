using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameModeManager : MonoBehaviour
{
    // ���� �Ÿ� / ���Ѹ�� ���߿� ������ ���丮 ����� �б⸦ ���� ��ũ��Ʈ�� ����� ����
    
    [Header("UI ����")]
    [SerializeField]
    private Image CharacterImage;
    private GameViewManager gameViewManager;

    [Header("���� ����")]
    // �Ÿ��� �ʱ�ȭ�� �ʱ� ����
    [SerializeField]
    int Distance;
    // 1�ʸ� ������ Ÿ�̸� ����
    private float distanceTimer = 0f;
    // üũ����Ʈ �Ÿ�
    public int CheckPointDistance;
    // üũ����Ʈ ���� �÷���
    private bool isAtCheckpoint = false;  

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
    // ĳ���� ���� X ��ǥ �����
    private float charStartX;

    private bool isStaminaEmpty = false;

    private GuageManager guageManager;

    [Header("����ǰ ���� ����")]
    private int nextCollectIndex = 0;
    private float collectSpawnInterval;

    private void OnEnable()
    {
        GuageImageAlpha.OnStaminaEmpty += HandleStaminaEmpty;
        GuageImageAlpha.OnStaminaRecovered += HandleStaminaRecovered;
    }

    private void OnDisable()
    {
        GuageImageAlpha.OnStaminaEmpty -= HandleStaminaEmpty;
        GuageImageAlpha.OnStaminaRecovered -= HandleStaminaRecovered;
    }

    private void HandleStaminaEmpty() => isStaminaEmpty = true;
    private void HandleStaminaRecovered() => isStaminaEmpty = false;

    private void Awake()
    {
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

        // �� ����
        DoorObject.SetActive(false);

        // GameViewManager
        gameViewManager = GameObject.Find("GameViewManager").GetComponent<GameViewManager>();

        // ĳ������ ���� AnchoredPosition.x�� �� �� ����
        charStartX = CharacterImage.rectTransform.anchoredPosition.x;

        guageManager = GameObject.FindFirstObjectByType<GuageManager>();
    }

    private void Start()
    {
        // �۷ι� �������� ���� ��������
        Distance = GlobalVariable.Instance.PlayerCurrentDistance;
        CheckPointDistance = GlobalVariable.Instance.CheckPointDistance;

        int totalCollectCount = GlobalVariable.Instance.StageMaxCollectCount;
        collectSpawnInterval = totalCollectCount > 0
            ? CheckPointDistance / (float)(totalCollectCount + 1)
            : CheckPointDistance;
    }

    void Update()
    {
        // ���¹̳� ��������� �Ÿ� ���� ��°�� ��ŵ
        if (isStaminaEmpty) 
            return;

        // GameViewManager���� HandleStaminaZero �Լ��� ������ �Ǹ� Ʈ���Ÿ� ������ �Ÿ� ���� ������ ���´�
        if (!isAtCheckpoint)
        {
            // �Ÿ� ���� ����
            IncreaseDistanceOverTime();
            // �Ÿ� ��� ������ ������Ʈ
            AnimateDoorScale();
            // �̵� ������ ���� �Լ�
            AnimateProgressFill();
        }
        else
        {
            // Ŭ���� �г�
        } 
    }

    private void IncreaseDistanceOverTime()
    {
        // �ִϸ��̼� ���� ���¶�� ���� �ߴ�
        if (isStaminaEmpty)
            return;

        float speedMultiplier = 1f;

        if (guageManager != null && guageManager.GaugeValue <= guageManager.DANGER_THRESHOLD_LOW)
        {
            speedMultiplier = 2f;
        }
        else if (guageManager != null && guageManager.GaugeValue >= guageManager.DANGER_THRESHOLD_HIGH)
        {
            speedMultiplier = 0.5f;
        }


        distanceTimer += (Time.deltaTime * 2.5f)*speedMultiplier;

        while (distanceTimer >= 1f)
        {
            Distance++;
            distanceTimer -= 1f;

            TrySpawnCollectible();

            float thresholdDistance = CheckPointDistance * doorOpenThreshold;
            if (!hasDoorOpenStarted && Distance >= thresholdDistance)
            {
                hasDoorOpenStarted = true;
                DoorObject.SetActive(true);
                DoorObject.transform.localScale = doorStartScale;
                DoorObject.transform.localPosition = doorOriginalPosition;
            }
            if (Distance >= CheckPointDistance)
            {
                EnterCheckpoint();
                break;
            }
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

    // AnimationAllStop�� �ϸ� AnimateProgressFill�� �������
    private void AnimateProgressFill()
    {
        // �ִϸ��̼� ���� ���¶�� ��ġ ���� �ߴ�
        if (isStaminaEmpty)
            return;

        // 1) ���൵ ���
        float progress = Mathf.SmoothStep(0, 1, Distance / (float)CheckPointDistance);

        // 2) y ��ġ ����
        float startY = -682f;
        float endY = 805f;
        float newY = Mathf.Lerp(startY, endY, progress);

        // 3) ĳ���� ��ġ �̵�
        RectTransform charRT = CharacterImage.rectTransform;
        Vector2 anchored = charRT.anchoredPosition;
        anchored.y = newY;
        charRT.anchoredPosition = anchored;
    }

    private void TrySpawnCollectible()
    {
        float expectedSpawnDistance = collectSpawnInterval * (nextCollectIndex + 1);

        Debug.Log($"[TrySpawnCollectible] Distance: {Distance}, Expected: {expectedSpawnDistance}");

        if (Distance >= expectedSpawnDistance && nextCollectIndex < GlobalVariable.Instance.StageMaxCollectCount)
        {
            Debug.Log("����ǰ ���� ���� ���!");
            CollectManager.Instance.CreateCollectObject(); // �������� ���ο��� ���
            nextCollectIndex++;
        }
    }
}

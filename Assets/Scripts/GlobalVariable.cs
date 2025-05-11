using UnityEngine;

public class GlobalVariable : MonoBehaviour
{
    // ���̵��� �ֱ⿡ ���Ӿ����� ���������� ���ؾ��ϴ� �� ����

    // �̱��� ��� ���� - ���ϼ� ����, ���� ���ټ�, �������
    public static GlobalVariable Instance { get; private set; }

    [Header("üũ����Ʈ ���� ����")]
    // üũ����Ʈ �Ÿ�
    public int CheckPointDistance = 50;
    // üũ����Ʈ ����ϱ� ���� ��ġ Ƚ��
    public int CheckPointTouchCount = 10;

    void Awake()
    {
        // ���� ������Ʈ�� �����Ѵٸ� �ı�
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // ó�� ������ �ν��Ͻ���� ����ϰ� �� ��ȯ �� �ı����� �ʵ��� ����
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

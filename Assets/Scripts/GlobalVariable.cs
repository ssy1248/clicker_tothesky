using UnityEngine;

public class GlobalVariable : MonoBehaviour
{
    // ���̵��� �ֱ⿡ ���Ӿ����� ���������� ���ؾ��ϴ� �� ����

    // üũ����Ʈ �Ÿ�
    public int CheckPointDistance;
    // üũ����Ʈ ����ϱ� ���� ��ġ Ƚ��
    public int CheckPointTouchCount;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}

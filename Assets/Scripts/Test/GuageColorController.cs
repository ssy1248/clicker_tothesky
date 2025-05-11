using UnityEngine;
using UnityEngine.UI;

public class GuageColorController : MonoBehaviour
{
    [SerializeField] private Image filledGaugeImage;

    private void Start()
    {
        SetGaugeColor();
    }

    public void SetGaugeColor(/*Color c*/)
    {
        // ������ �ٲ�µ� ��� ������ ������ �ִٺ��� ���� �̻���
        // 0~1 ������ ���ķ� ���� ������ ����
        filledGaugeImage.color = Color.green;
    }
}

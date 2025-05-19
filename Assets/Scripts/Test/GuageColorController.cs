using UnityEngine;
using UnityEngine.UI;

public class GuageColorController : MonoBehaviour
{
    public static GuageColorController Instance { get; private set; }

    [SerializeField] private Image filledGaugeImage;

    void Awake()
    {
        Instance = this;
    }

    public void SetGaugeColor(Color c)
    {
        // 0~1 ������ ���ķ� ���� ������ ����
        filledGaugeImage.color = c;
    }
}

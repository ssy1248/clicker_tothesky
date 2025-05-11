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
        // 색상이 바뀌는데 대신 원래의 색상이 있다보니 색이 이상함
        // 0~1 사이의 알파로 투명도 조절도 가능
        filledGaugeImage.color = Color.green;
    }
}

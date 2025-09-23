using TMPro;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance { get; private set; }

    [SerializeField]
    private int comboCount = 1; // 현재 콤보 수
    [SerializeField]
    private TextMeshProUGUI comboText; // 콤보 수를 표시할 텍스트

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if(comboCount <= 0)
            comboText.text = "";
        else
            UpdateComboText();
    }

    public void AddCombo()
    {
        comboCount++;
        UpdateComboText();
    }

    public void ResetCombo()
    {
        comboCount = 0;
        UpdateComboText();
    }

    private void UpdateComboText()
    {
        if (comboCount <= 0)
        {
            comboText.text = "";
            return;
        }

        comboText.text = $"{comboCount} Combo!!";
    }
}

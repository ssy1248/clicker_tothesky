using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    private void OnEnable()
    {
        // 초기값 세팅 (이때 이벤트 중복 호출 방지)
        if (SEManager.instance == null) return;

        masterSlider.SetValueWithoutNotify(SEManager.instance.GetMasterVolume());
        bgmSlider.SetValueWithoutNotify(SEManager.instance.GetBGMVolume());
        seSlider.SetValueWithoutNotify(SEManager.instance.GetSEVolume());

        masterSlider.onValueChanged.AddListener(SEManager.instance.SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SEManager.instance.SetBGMVolume);
        seSlider.onValueChanged.AddListener(SEManager.instance.SetSEVolume);
    }

    private void OnDisable()
    {
        if (SEManager.instance == null) return;

        masterSlider.onValueChanged.RemoveListener(SEManager.instance.SetMasterVolume);
        bgmSlider.onValueChanged.RemoveListener(SEManager.instance.SetBGMVolume);
        seSlider.onValueChanged.RemoveListener(SEManager.instance.SetSEVolume);
    }
}

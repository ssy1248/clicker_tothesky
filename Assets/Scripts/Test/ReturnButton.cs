using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnButton : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("애니메이션의 속도를 조절합니다.")]
    public float animationSpeed = 5f; // 애니메이션 속도
    [Tooltip("커지는 스케일의 강도를 조절합니다.")]
    public float scaleAmount = 0.1f; // 스케일 강도

    private Vector3 originalScale; // 버튼의 원래 스케일 값

    void Start()
    {
        // 스크립트가 시작될 때 버튼의 원래 크기를 저장합니다.
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Mathf.Sin 함수를 사용해 -1과 1 사이를 부드럽게 왕복하는 값을 만듭니다.
        // Time.time을 곱해 시간에 따라 값이 계속 변하도록 합니다.
        float pulse = Mathf.Sin(Time.time * animationSpeed) * scaleAmount;

        // 원래 크기에 계산된 pulse 값을 더해 스케일을 조절합니다.
        transform.localScale = originalScale + new Vector3(pulse, pulse, pulse);
    }

    public void ReturnGameScene()
    {
        Debug.Log("버튼 눎림");
        GlobalVariable.Instance.EnteredFromShop();
        SceneManager.LoadScene("MainScene");
    }
}

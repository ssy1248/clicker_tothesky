using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    public GameObject GameOverUIObject;

    private void Awake()
    {
        // 같은 오브젝트가 존재한다면 파괴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (GameOverUIObject == null)
            GameOverUIObject = GameObject.Find("GameOverPanel");
    }

    void Update()
    {
        
    }

    public void ShowGameOverPanel()
    {
        AllUISetActiveFalse();
        GameOverUIObject.SetActive(true);
    }

    void AllUISetActiveFalse()
    {

    }
}

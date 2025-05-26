using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnButton : MonoBehaviour
{
    public void ReturnGameScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}

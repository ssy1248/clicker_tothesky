using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnButton : MonoBehaviour
{
    public void ReturnGameScene()
    {
        GlobalVariable.Instance.EnteredFromShop();
        SceneManager.LoadScene("MainScene");
    }
}

using UnityEngine;

public class SoundManager : MonoBehaviour
{
    void Start()
    {
        SEManager.instance.LoopPlaySE("GameStart");
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SEManager.instance.PlaySE("UI Click");
        }
    }
}

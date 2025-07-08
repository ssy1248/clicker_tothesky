using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public GameObject videoPlayer;

    public void SkipBtn()
    {
        videoPlayer.SetActive(false);
    }
}

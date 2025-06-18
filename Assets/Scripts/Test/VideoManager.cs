using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    [Header("Video Settings")]
    [SerializeField] private VideoPlayer videoPlayer; // VideoPlayer 컴포넌트를 연결

    // 버튼에 이 함수 연결
    public void PlayVideo()
    {
        if (videoPlayer == null)
        {
            Debug.LogWarning("VideoPlayer가 할당되지 않았습니다.");
            return;
        }

        // VideoPlayer 오브젝트가 비활성화 상태면 활성화
        if (!videoPlayer.gameObject.activeSelf)
        {
            videoPlayer.gameObject.SetActive(true);
        }

        // 이미 클립이 videoPlayer에 연결되어 있다고 가정하고 바로 재생
        videoPlayer.Play();

        // 현재 클릭한 버튼을 비활성화
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (clickedButton != null)
        {
            clickedButton.SetActive(false);
        }
    }
}

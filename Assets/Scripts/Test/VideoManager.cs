using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    // 인스펙터에서 할당
    public VideoPlayer videoPlayer;
    public GameObject popUpUI;

    // LogoPanel에서 넘겨받을 참조
    private LogoPanel logoPanelController;

    /// <summary>
    /// LogoPanel이 호출하여 자기 자신을 알려주는 초기화 함수
    /// </summary>
    public void Initialize(LogoPanel logoPanel)
    {
        this.logoPanelController = logoPanel;

        // 인트로 시작: 게임 오디오는 뮤트
        SEManager.instance?.BeginIntroMute();

        // 영상이 자연 종료되면 복원 후 다음 단계로
        videoPlayer.loopPointReached += _ =>
        {
            SEManager.instance?.EndIntroMute();
            logoPanelController?.FinishIntroSequence();
        };
    }

    /// <summary>
    /// 스킵 버튼을 누르면 호출될 함수
    /// </summary>
    public void ShowPopUpUI()
    {
        // 팝업을 띄우고 비디오를 일시정지
        popUpUI.SetActive(true);
        videoPlayer.Pause();
    }

    /// <summary>
    /// 팝업의 'Yes' 버튼에 연결할 함수
    /// </summary>
    public void SkipYesBtn()
    {
        // 팝업을 닫고, LogoPanel에게 영상 종료 시퀀스를 실행하라고 요청
        popUpUI.SetActive(false);
        SEManager.instance?.EndIntroMute(); // 복원
        logoPanelController?.FinishIntroSequence();
    }

    /// <summary>
    /// 팝업의 'No' 버튼에 연결할 함수
    /// </summary>
    public void SkipNoBtn()
    {
        // 팝업을 닫고 비디오를 다시 재생
        popUpUI.SetActive(false);
        videoPlayer.Play();
    }
}

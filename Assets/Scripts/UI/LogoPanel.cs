using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LogoPanel : BasePanel
{
    public Button buttonStart;
    
    public Button buttonCredit;

    
    public override UIPanelType TypeOfPanel => UIPanelType.LOGO_PANEL;

    private void Awake()
    {
        buttonStart.onClick.RemoveListener(OnClickStart);
        buttonCredit.onClick.RemoveListener(OnClickCredit);
        buttonStart.onClick.AddListener(OnClickStart);
        buttonCredit.onClick.AddListener(OnClickCredit);
    }

    void Update()
    {
        // UNITY_EDITOR 에서만 이 코드가 포함되도록 하여,
        // 실제 빌드된 게임에서는 이 기능이 포함되지 않도록 합니다.
#if UNITY_EDITOR
        // 'Q' 키가 눌렸는지 확인합니다.
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // GlobalVariable에 있는 데이터 삭제 함수를 호출합니다.
            GlobalVariable.Instance.DeleteSaveData();
        }
#endif
    }

    public override void OnEnter(params object[] datas)
    {
        base.OnEnter(datas);
        this.gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        this.gameObject.SetActive(false);
    }

    public void OnClickStart()
    {
        // 첫 실행 플래그를 확인합니다.
        if (GlobalVariable.Instance.isFirstTimeLaunch)
        {
            // 처음 실행하는 경우
            PlayIntroVideo();
        }
        else
        {
            // 이미 플레이한 적이 있는 경우
            ShowStagePanel();
        }
    }

    private void PlayIntroVideo()
    {
        // 1. 씬에서 VideoPlayer 컴포넌트를 찾습니다.
        VideoPlayer introPlayer = FindObjectOfType<VideoPlayer>(true); // 비활성화된 것도 찾기

        if (introPlayer != null)
        {
            OnClose(); // 로고 패널 닫기

            // 2. 비디오 플레이어의 재생이 끝났을 때 실행될 함수를 등록합니다.
            introPlayer.loopPointReached += OnIntroVideoEnd;

            // 3. 비디오 플레이어 오브젝트를 활성화하고 재생합니다.
            introPlayer.gameObject.SetActive(true);
            introPlayer.Play();

            // 4. 이제 '처음 실행'이 아니므로 플래그를 변경하고 저장합니다.
            GlobalVariable.Instance.isFirstTimeLaunch = false;
            GlobalVariable.Instance.SaveGame();
        }
        else
        {
            // 혹시 비디오 플레이어를 못 찾았을 경우에 대한 예외 처리
            Debug.LogWarning("인트로 비디오 플레이어를 찾을 수 없습니다. 스테이지 선택으로 넘어갑니다.");
            ShowStagePanel();
        }
    }

    // 비디오 재생이 끝나면 호출될 함수
    void OnIntroVideoEnd(VideoPlayer vp)
    {
        // 이벤트 중복 호출을 막기 위해 등록을 해제합니다.
        vp.loopPointReached -= OnIntroVideoEnd;

        // 비디오 플레이어는 다시 비활성화 합니다.
        vp.gameObject.SetActive(false);

        // 스테이지 선택 패널을 보여줍니다.
        ShowStagePanel();
    }

    private void ShowStagePanel()
    {
        OnClose(); // 로고 패널 닫기

        // 기존의 스테이지 패널을 찾거나 새로 띄우는 로직
        GameObject stagePanel = GameObject.Find("StagePanel(Clone)");
        if (stagePanel != null)
        {
            stagePanel.SetActive(true);
        }
        else
        {
            UIManager.Instance.PushPanel(UIPanelType.STAGE_PANEL);
        }
    }

    public void OnClickCredit()
    {
        OnClose();
        UIManager.Instance.PushPanel(UIPanelType.CREDIT_PANEL);
    }
}

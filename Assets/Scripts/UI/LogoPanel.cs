using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LogoPanel : BasePanel
{
    public Button buttonStart;
    
    public Button buttonCredit;
    public override UIPanelType TypeOfPanel => UIPanelType.LOGO_PANEL;

    private VideoPlayer activeIntroPlayer; // 현재 활성화된 비디오 플레이어 참조
    private Button skipButton;             // 스킵 버튼 참조

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
        activeIntroPlayer = FindObjectOfType<VideoPlayer>(true);

        if (activeIntroPlayer != null)
        {
            OnClose(); // 로고 패널 닫기

            // 2. 비디오 플레이어의 자식 오브젝트에서 스킵 버튼을 찾습니다.
            skipButton = activeIntroPlayer.GetComponentInChildren<Button>();
            if (skipButton != null)
            {
                // 3. 스킵 버튼에 'FinishIntroSequence' 함수를 리스너로 등록
                skipButton.onClick.AddListener(FinishIntroSequence);
            }

            // 4. 비디오 재생이 끝나면 실행될 함수를 등록합니다.
            activeIntroPlayer.loopPointReached += OnIntroVideoEnd;

            // 5. 비디오 플레이어 오브젝트를 활성화하고 재생합니다.
            activeIntroPlayer.gameObject.SetActive(true);
            activeIntroPlayer.Play();

            // 6. 플래그 변경 및 저장
            GlobalVariable.Instance.isFirstTimeLaunch = false;
            GlobalVariable.Instance.SaveGame();
        }
        else
        {
            Debug.LogWarning("인트로 비디오 플레이어를 찾을 수 없습니다. 스테이지 선택으로 넘어갑니다.");
            ShowStagePanel();
        }
    }

    // 비디오 재생이 끝나면 호출될 함수
    void OnIntroVideoEnd(VideoPlayer vp)
    {
        FinishIntroSequence();
    }

    // 영상이 끝나거나, 스킵될 때 공통으로 호출되는 함수
    private void FinishIntroSequence()
    {
        // 이미 처리가 끝났으면 중복 실행 방지
        if (activeIntroPlayer == null) return;

        // 1. 등록했던 모든 이벤트를 해제하여 중복 호출을 막습니다.
        activeIntroPlayer.loopPointReached -= OnIntroVideoEnd;
        if (skipButton != null)
        {
            skipButton.onClick.RemoveListener(FinishIntroSequence);
        }

        // 2. 비디오 플레이어는 다시 비활성화 합니다.
        activeIntroPlayer.gameObject.SetActive(false);

        // 3. 참조를 초기화합니다.
        activeIntroPlayer = null;
        skipButton = null;

        // 4. 스테이지 선택 패널을 보여줍니다.
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

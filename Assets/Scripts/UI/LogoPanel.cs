using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LogoPanel : BasePanel
{
    public Button buttonStart;
    
    public Button buttonCredit;

    public Button buttonExit;
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
        // "게임을 한 번이라도 시작한 적이 없는가?"를 확인합니다.
        if (GlobalVariable.Instance.hasStartedGameBefore == false)
        {
            // 1. 이제 첫 플레이를 시작했으므로 플래그를 true로 변경합니다.
            GlobalVariable.Instance.hasStartedGameBefore = true;

            // 2. 이 상태를 즉시 저장하여, 다음 실행부터는 인트로가 나오지 않도록 합니다.
            GlobalVariable.Instance.SaveGame();

            // 3. 처음 실행하는 경우이므로 인트로 비디오를 재생합니다.
            PlayIntroVideo();
        }
        else
        {
            // 이미 플레이한 적이 있는 경우 스테이지 선택 패널로 바로 이동합니다.
            ShowStagePanel();
        }
    }

    private void PlayIntroVideo()
    {
        // 1. 씬에서 VideoPlayer 컴포넌트를 찾습니다.
        activeIntroPlayer = FindObjectOfType<VideoPlayer>(true);

        if (activeIntroPlayer != null)
        {
            // 1. VideoPlayer 오브젝트에서 VideoManager 스크립트를 찾습니다.
            VideoManager videoManager = activeIntroPlayer.GetComponent<VideoManager>();
            if (videoManager == null)
            {
                Debug.LogError("VideoPlayer 오브젝트에 VideoManager 스크립트가 없습니다!");
                FinishIntroSequence(); // 비디오 매니저가 없으면 그냥 스킵 처리
                return;
            }

            OnClose();

            // 2. 스킵 버튼을 찾습니다.
            skipButton = activeIntroPlayer.GetComponentInChildren<Button>();
            if (skipButton != null)
            {
                // 3. VideoManager에게 LogoPanel 자신(this)을 알려줘서 나중에 통신할 수 있게 합니다.
                videoManager.Initialize(this);

                // 4. 스킵 버튼의 리스너를 모두 지우고, 'ShowPopUpUI' 함수를 새로 등록합니다.
                skipButton.onClick.RemoveAllListeners();
                skipButton.onClick.AddListener(videoManager.ShowPopUpUI);
            }

            // 4. 비디오 재생이 끝나면 실행될 함수를 등록합니다.
            activeIntroPlayer.loopPointReached += OnIntroVideoEnd;

            // 5. 비디오 플레이어 오브젝트를 활성화하고 재생합니다.
            activeIntroPlayer.gameObject.SetActive(true);
            activeIntroPlayer.Play();
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
    public void FinishIntroSequence()
    {
        // 이미 처리가 끝났으면 중복 실행 방지
        if (activeIntroPlayer == null) return;

        // 1. 등록했던 모든 이벤트를 해제하여 중복 호출을 막습니다.
        activeIntroPlayer.loopPointReached -= OnIntroVideoEnd;

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

    public void OnClickExit()
    {
        Debug.Log("게임 종료 버튼이 클릭되었습니다.");

        // 유니티 에디터에서 실행 중일 경우
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        // 실제 빌드된 게임(PC, 모바일 등)에서 실행 중일 경우
#else
    Application.Quit();
#endif
    }
}

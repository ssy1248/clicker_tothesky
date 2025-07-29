using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearPanel : MonoBehaviour
{
    [Header("UI 텍스트")]
    public TextMeshProUGUI clearTimeText;
    public TextMeshProUGUI collectCountText; // 수집품 텍스트를 인스펙터에서 연결

    [Header("UI 버튼")]
    public Button nextRoundButton; // 마지막 스테이지에서 비활성화하기 위해 연결

    [Header("챕터 데이터베이스")]
    public ChapterDatabase chapterDatabase;

    private void OnEnable()
    {
        // 1. 클리어 시간 표시
        float time = GlobalVariable.Instance.LastClearTime;
        int minutes = (int)time / 60;
        int secs = (int)time % 60;
        clearTimeText.text = $"{minutes:00}:{secs:00}";

        // 2. 마지막 라운드일 경우 '다음' 버튼 비활성화
        int currentPlayStage = GlobalVariable.Instance.PlayerCurrentPlayerStage;
        int totalStageCount = 0;
        if (chapterDatabase != null)
        {
            foreach (ChapterData chapter in chapterDatabase.allChapterData)
            {
                totalStageCount += chapter.stagesInChapter.Length;
            }
        }

        // 현재 플레이한 스테이지가 전체 스테이지 중 마지막인지 확인합니다.
        if (currentPlayStage >= totalStageCount - 1)
        {
            nextRoundButton.gameObject.SetActive(false);
        }
        else
        {
            nextRoundButton.gameObject.SetActive(true);
        }
    }

    // 'Home' 버튼을 눌렀을 때 호출될 함수
    public void OnClickHomeButton()
    {
        // 타이틀 씬으로 이동
        SceneManager.LoadScene("TitleScene");

        // UIManager가 있다면 특정 패널을 띄우는 것을 보장할 수 있습니다.
        // UIManager.Instance.ShowPanel(UIPanelType.LOGO_PANEL);
    }

    // 'NextRound' 버튼을 눌렀을 때 호출될 함수
    public void OnClickNextRoundButton()
    {
        int nextStageIndex = GlobalVariable.Instance.PlayerCurrentPlayerStage + 1;

        // 다음 스테이지 정보 세팅
        GlobalVariable.Instance.SetupStage(nextStageIndex, chapterDatabase);

        // 상점 씬으로 이동
        SceneManager.LoadScene("ShopScene");

    }
}

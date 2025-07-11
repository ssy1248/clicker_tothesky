using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable] // 이 줄을 추가해야 인스펙터에서 보입니다.
public struct StageData
{
    public string stageName; // (선택사항) 스테이지 이름
    public Sprite stageSprite; // 스테이지(월) 이미지
    public float gameTime; // 스테이지 시간
    public int clearDistance; // 클리어 목표 거리
    public int maxCollectibles; // 최대 수집품 개수
}

public class StagePanelManager : MonoBehaviour
{
    [Header("UI 요소 연결")]
    public Image stageRoundNumberImage; // 월(1, 2, 3...)을 표시할 Image 컴포넌트

    [Header("스테이지 데이터베이스")]
    public StageDatabase stageDatabase;

    [Header("스테이지 데이터")]
    public StageData[] allStageData; // Sprite 배열 대신 StageData 배열 사용

    private int currentStageIndex = 0; // 현재 선택된 월 인덱스 (0 = 1월)

    void Start()
    {
        // 1. GlobalVariable에서 플레이어가 마지막으로 클리어한 스테이지 정보를 가져옴 - PlayerClearRound는 0부터 시작하는 인덱스이므로, 다음 스테이지는 +1을 해줍니다.
        int nextStageIndex = GlobalVariable.Instance.PlayerClearRound + 1;

        // 2. 만약 모든 스테이지를 클리어했다면, 마지막 스테이지를 표시
        if (nextStageIndex >= stageDatabase.allStageData.Length)
        {
            nextStageIndex = stageDatabase.allStageData.Length - 1;
        }

        // 3. 계산된 인덱스를 현재 스테이지 인덱스로 설정
        currentStageIndex = nextStageIndex;

        // 4. UI를 업데이트
        UpdateStageUI();
    }

    public void ShowNextStage()
    {
        currentStageIndex++;
        if (currentStageIndex >= allStageData.Length)
        {
            currentStageIndex = 0;
        }
        UpdateStageUI();
    }

    public void ShowPreviousStage()
    {
        currentStageIndex--;
        if (currentStageIndex < 0)
        {
            currentStageIndex = allStageData.Length - 1;
        }
        UpdateStageUI();
    }

    // UI를 업데이트하는 함수
    private void UpdateStageUI()
    {
        // 데이터베이스에서 현재 스테이지 데이터를 가져옵니다.
        StageData currentStage = stageDatabase.allStageData[currentStageIndex];
        stageRoundNumberImage.sprite = currentStage.stageSprite;
    }

    // "START" 버튼을 눌렀을 때 호출될 함수
    public void StartGame()
    {
        if (currentStageIndex > GlobalVariable.Instance.PlayerClearRound + 1)
        {
            Debug.Log("이 스테이지는 아직 잠겨있습니다!");
            return;
        }

        // 스테이지 정보 세팅
        GlobalVariable.Instance.SetupStage(currentStageIndex, stageDatabase);

        // 씬 로드
        SceneManager.LoadScene("ShopScene");
    }

    public void ReturnMenu()
    {
        GameObject logoPanel = GameObject.Find("LogoPanel(Clone)");

        if (logoPanel != null) // stagePanel을 찾았다면 (null이 아니라면)
        {
            // 해당 오브젝트를 활성화합니다.
            logoPanel.SetActive(true);
        }
        else
        {
            UIManager.Instance.PushPanel(UIPanelType.LOGO_PANEL);
        }
    }
}


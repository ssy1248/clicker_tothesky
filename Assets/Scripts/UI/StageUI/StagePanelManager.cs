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
    public int clearDistance; // 클리어 목표 거리
    public int maxCollectibles; // 최대 수집품 개수
}

public class StagePanelManager : MonoBehaviour
{
    [Header("UI 요소 연결")]
    public Image stageRoundNumberImage; // 월(1, 2, 3...)을 표시할 Image 컴포넌트

    [Header("스테이지 데이터")]
    public StageData[] allStageData; // Sprite 배열 대신 StageData 배열 사용

    private int currentStageIndex = 0; // 현재 선택된 월 인덱스 (0 = 1월)

    void Start()
    {
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
        // 현재 선택된 스테이지 데이터를 가져옴
        StageData currentStage = allStageData[currentStageIndex];

        // 이미지 교체
        stageRoundNumberImage.sprite = currentStage.stageSprite;
    }

    // "START" 버튼을 눌렀을 때 호출될 함수
    public void StartGame()
    {
        if (currentStageIndex > GlobalVariable.Instance.PlayerClearRound)
        {
            // 아직 플레이할 수 없는 스테이지를 선택한 경우
            Debug.Log("이 스테이지는 아직 잠겨있습니다!");

            // 여기에 "아직 잠긴 스테이지입니다" 같은 UI 팝업이나 메시지를 띄워주면

            return; // 함수를 여기서 종료시켜 게임 시작 로직이 실행되지 않도록 함
        }

        // 현재 선택된 스테이지 데이터를 가져옴
        StageData selectedStage = allStageData[currentStageIndex];

        // GlobalVariable에 데이터 설정
        GlobalVariable.Instance.CheckPointDistance = selectedStage.clearDistance;
        GlobalVariable.Instance.StageMaxCollectCount = selectedStage.maxCollectibles;

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


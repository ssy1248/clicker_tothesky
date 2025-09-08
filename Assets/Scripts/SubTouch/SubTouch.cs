using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubTouch : MonoBehaviour
{
    protected int successScore; // 성공 점수는 모든 자식이 공통으로 사용

    /// <summary>
    /// 미니게임을 시작하는 공통 함수
    /// </summary>
    public virtual void Initialize(int score)
    {
        this.successScore = score;
    }

    /// <summary>
    /// 미니게임을 끝내는 공통 함수
    /// </summary>
    protected virtual void EndMiniGame()
    {
        // SubTouchManager에게 종료 알림 등의 공통 로직 추가 가능
        Destroy(gameObject);
    }
}

using UnityEngine;

public class SubTouchNote : MonoBehaviour
{
    // 노트 처리 결과를 판정과 함께 외부에 알리기 위한 델리게이트
    public delegate void NoteResultDelegate(SubTouchNote note, Judgement judgement);
    public static event NoteResultDelegate OnNoteProcessEnd;

    /// <summary>
    /// 노트 처리 결과를 외부에 알리고 자신을 파괴합니다.
    /// </summary>
    /// <param name="judgement">최종 판정 결과</param>
    protected void ReportResult(Judgement judgement)
    {
        // 외부에 판정 결과를 방송(Invoke)합니다.
        OnNoteProcessEnd?.Invoke(this, judgement);
        // 결과를 보고한 후 오브젝트를 파괴합니다.
        Destroy(gameObject);
    }
}

using UnityEngine;
[CreateAssetMenu(fileName = "ItemDB", menuName = "Kit/Data/ItemDB", order=0)]
public class ItemDB : ScriptableObject
{
    //아이템 순번
    public int ItemOrder;
    //아이템 아이콘
    public Sprite icon;
    //아이템 이름
    public string name;
    //아이템 희귀도
    public int rareness;
    //StatusTypes 중 HP 관련 수치 변화 변수 입력
    public int HP;
    //StatusTypes 중 시간 관련 수치 변화 변수 입력
    public int Time;
    //StatusTypes 중 클릭 관련 수치 변화 변수 입력
    public int ClickStack;
    //StatusTypes 중 상태 관련 수치 변화 변수 입력
    public int Status;
    //StatusTypes 중 부활 관련 수치 변화 변수 입력
    public int Revive;
    //StatusTypes 중 건너뛰기 관련 수치 변화 변수 입력
    public int Skip;
    //StatusTypes 중 확률 관련 수치 변화 변수 입력
    public int Percentage;
    //StatusTypes 중 개수 관련 수치 변화 변수 입력
    public int Number;
}

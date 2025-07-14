using UnityEngine;

[CreateAssetMenu(fileName = "CollectiableObject", menuName = "Game Data/Collectiable")]
public class CollectScriptableObject : ScriptableObject
{
    // 수집품 아이디(필수)
    public int CollectId;
    // 수집품 이름 
    public string CollectName;
    // 수집품 이미지
    public Sprite CollectSprite;
    // 수집품 설명 -> 나중 도감 같은곳에 사용할 목적
    public string CollectDescription;
}

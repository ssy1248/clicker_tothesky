using System.Collections.Generic;
using UnityEngine;

// CharacterData.cs
// 데이터를 담을 순수 C# 클래스입니다.
[System.Serializable] // 직렬화 가능하도록 설정
public class CharacterData
{
    public int id;
    public string name;
    public int hp;
    public int attack;
}

// 에셋 메뉴에서 쉽게 생성할 수 있도록 메뉴를 추가합니다.
[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "ScriptableObjects/CharacterDatabase", order = 1)]
public class CharacterDatabase : ScriptableObject
{
    // 캐릭터 데이터 리스트를 저장할 변수
    public List<CharacterData> characters;
}

using NUnit.Framework.Interfaces;
using UnityEditor;

public class DataImportMenu
{
    // --- 캐릭터 데이터 임포트 ---
    private const string CharacterCsvPath = "Assets/Data/CSV/CharacterData.csv";
    private const string CharacterAssetPath = "Assets/Data/Databases/CharacterDatabase.asset";

    [MenuItem("Tools/Import Data/Import Character Data")]
    public static void ImportCharacterData()
    {
        // 범용 임포터 호출
        GenericDataImporter.Import<CharacterData, CharacterDatabase>(CharacterCsvPath, CharacterAssetPath);
    }


    // --- 아이템 데이터 임포트 ---
    private const string ItemCsvPath = "Assets/Data/CSV/ItemData.csv";
    private const string ItemAssetPath = "Assets/Data/Databases/ItemDatabase.asset";

    [MenuItem("Tools/Import Data/Import Item Data")]
    public static void ImportItemData()
    {
        // 동일한 범용 임포터를 다른 데이터 타입으로 호출
        //GenericDataImporter.Import<ItemData, ItemDatabase>(ItemCsvPath, ItemAssetPath);
    }
}

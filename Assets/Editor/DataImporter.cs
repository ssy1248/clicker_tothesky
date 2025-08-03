using UnityEngine;
using UnityEditor; // UnityEditor 네임스페이스 필요
using System.IO;   // 파일 입출력을 위한 System.IO
using System.Collections.Generic;

public class DataImporter
{
    // CSV 파일들이 있는 경로
    private static string csvFolderPath = "Assets/Data/CSV";
    // 생성된 스크립터블 오브젝트가 저장될 경로
    private static string assetPath = "Assets/Data/CharacterDatabase.asset";

    // 유니티 상단 메뉴에 "Tools/Import Character Data" 메뉴를 추가
    [MenuItem("Tools/Import Character Data")]
    public static void ImportData()
    {
        // 지정된 폴더에 있는 모든 CSV 파일 경로를 가져옴
        string[] csvFiles = Directory.GetFiles(csvFolderPath, "*.csv");

        // 데이터를 담을 전체 리스트 생성
        List<CharacterData> allCharacters = new List<CharacterData>();

        foreach (string filePath in csvFiles)
        {
            // 파일의 모든 텍스트를 줄 단위로 읽어옴
            string[] lines = File.ReadAllLines(filePath);

            // 첫 줄(헤더)은 건너뛰고 두 번째 줄부터 읽음 (i=1)
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                // 쉼표(,)를 기준으로 데이터 분리
                string[] values = line.Split(',');

                // CharacterData 객체 생성 및 값 할당
                CharacterData data = new CharacterData();
                data.id = int.Parse(values[0]);
                data.name = values[1];
                data.hp = int.Parse(values[2]);
                data.attack = int.Parse(values[3]);

                // 리스트에 추가
                allCharacters.Add(data);
            }
        }

        // 기존 에셋이 있는지 확인
        CharacterDatabase database = AssetDatabase.LoadAssetAtPath<CharacterDatabase>(assetPath);

        if (database == null)
        {
            // 기존 에셋이 없으면 새로 생성
            database = ScriptableObject.CreateInstance<CharacterDatabase>();
            AssetDatabase.CreateAsset(database, assetPath);
        }

        // 데이터베이스의 리스트를 새로 만든 리스트로 교체
        database.characters = allCharacters;

        // 변경사항 저장
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"<color=green>캐릭터 데이터 임포트 완료! 총 {allCharacters.Count}개의 데이터가 {assetPath}에 저장되었습니다.</color>");
    }
}

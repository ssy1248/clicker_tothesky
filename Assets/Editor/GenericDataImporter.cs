using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection; // 리플렉션 사용을 위해 필요

public class GenericDataImporter
{
    /// <summary>
    /// 지정된 CSV 파일의 데이터를 파싱하여 ScriptableObject 에셋을 생성하거나 업데이트합니다.
    /// </summary>
    /// <typeparam name="T">데이터 구조를 정의하는 일반 클래스 (예: CharacterData)</typeparam>
    /// <typeparam name="U">데이터 리스트를 담을 ScriptableObject 클래스 (예: CharacterDatabase)</typeparam>
    /// <param name="csvPath">프로젝트 상대 경로 (예: "Assets/Data/CSV/Character.csv")</param>
    /// <param name="assetPath">생성될 에셋의 경로 (예: "Assets/Data/CharacterDatabase.asset")</param>
    public static void Import<T, U>(string csvPath, string assetPath)
        where T : class, new() // T는 참조 타입이며, 기본 생성자를 가져야 함
        where U : ScriptableObject // U는 ScriptableObject를 상속해야 함
    {
        // 1. CSV 파일 읽기
        if (!File.Exists(csvPath))
        {
            Debug.LogError($"[GenericDataImporter] 파일을 찾을 수 없습니다: {csvPath}");
            return;
        }

        string[] lines = File.ReadAllLines(csvPath);
        if (lines.Length < 2)
        {
            Debug.LogWarning($"[GenericDataImporter] 데이터가 없는 파일입니다: {csvPath}");
            return;
        }

        string[] headers = lines[0].Trim().Split(',');
        List<T> dataList = new List<T>();

        // 2. 리플렉션을 사용하여 데이터 파싱 및 리스트 생성
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Split(',');
            T dataEntry = new T();

            for (int j = 0; j < headers.Length; j++)
            {
                // CSV 헤더 이름과 일치하는 T 클래스의 필드(변수) 정보를 가져옴
                FieldInfo field = typeof(T).GetField(headers[j].Trim());

                if (field != null && j < values.Length)
                {
                    try
                    {
                        // 필드의 타입에 맞게 문자열 값을 변환 (int, float, bool, string, enum 등 지원)
                        var convertedValue = Convert.ChangeType(values[j], field.FieldType);
                        field.SetValue(dataEntry, convertedValue);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[GenericDataImporter] 데이터 변환 오류 발생. 파일: {csvPath}, 행: {i + 1}, 필드: {headers[j]}, 오류: {e.Message}");
                    }
                }
            }
            dataList.Add(dataEntry);
        }

        // 3. ScriptableObject 에셋 생성 또는 업데이트
        U database = AssetDatabase.LoadAssetAtPath<U>(assetPath);
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<U>();
            AssetDatabase.CreateAsset(database, assetPath);
        }

        // 리플렉션을 사용해 'database' 객체에서 List<T> 타입의 필드를 찾아 데이터를 할당
        FieldInfo listField = typeof(U).GetField(GetDataListName<T>());
        if (listField != null)
        {
            listField.SetValue(database, dataList);
        }
        else
        {
            Debug.LogError($"[GenericDataImporter] {typeof(U).Name} 클래스에서 '{GetDataListName<T>()}' 이름의 List<{typeof(T).Name}> 필드를 찾을 수 없습니다.");
            return;
        }

        // 4. 변경사항 저장
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"<color=cyan>[GenericDataImporter]</color> 임포트 완료! 총 {dataList.Count}개의 '{typeof(T).Name}' 데이터가 {assetPath}에 저장되었습니다.");
    }

    /// <summary>
    /// 데이터 리스트 필드의 예상 이름을 반환합니다. (예: CharacterData -> characters)
    /// </summary>
    private static string GetDataListName<T>()
    {
        // 클래스 이름에 's'를 붙여 복수형으로 만듭니다. (예: CharacterData -> CharacterDatas)
        // 실제 프로젝트에서는 더 정교한 규칙을 적용할 수 있습니다.
        return typeof(T).Name + "s";
    }
}

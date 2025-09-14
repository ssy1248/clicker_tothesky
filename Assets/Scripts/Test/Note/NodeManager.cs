using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public GameObject[] Note;
    public GameObject SpawnPoint;

    private RectTransform spawnAreaRect; // SpawnPoint의 RectTransform 컴포넌트

    void Start()
    {
        // SpawnPoint의 RectTransform 컴포넌트를 가져옴
        spawnAreaRect = SpawnPoint.GetComponent<RectTransform>();
    }

    void Update()
    {
        // 테스트용: 스페이스바를 누를 때마다 노트 생성
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnNote();
        }
    }

    public void SpawnNote()
    {
        // 1. 생성될 위치 계산
        // spawnAreaRect.rect는 Pivot을 기준으로 한 로컬 좌표와 크기를 제공합니다.
        float randomX = Random.Range(spawnAreaRect.rect.xMin, spawnAreaRect.rect.xMax);
        float randomY = Random.Range(spawnAreaRect.rect.yMin, spawnAreaRect.rect.yMax);

        Vector2 spawnPosition = new Vector2(randomX, randomY);

        // 2. 노트 프리팹 생성
        // Instantiate의 두 번째 인자로 부모(spawnAreaRect.transform)를 지정하면
        // 해당 UI 요소의 자식으로 생성되어 스케일과 위치가 올바르게 적용됩니다.
        int randomIndex = Random.Range(0, Note.Length);
        GameObject newNote = Instantiate(Note[randomIndex], spawnAreaRect.transform);

        // 3. 생성된 노트의 위치 설정
        // UI 요소의 위치는 localPosition으로 설정해야 부모를 기준으로 배치됩니다.
        newNote.GetComponent<RectTransform>().localPosition = spawnPosition;
    }
}

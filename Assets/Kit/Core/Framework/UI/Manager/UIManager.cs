using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class UIPanelTypeJson
{
    public List<UIPanelInfo> infoList;
}

/// <summary>
/// UI 패널들을 관리하는 클래스
/// </summary>
public class UIManager : MonoBehaviour
{
    // MonoBehaviour 싱글톤 인스턴스
    public static UIManager Instance { get; private set; }

    [Header("패널 생성 위치")]
    [Tooltip("모든 UI 패널이 생성될 부모 Transform (반드시 Graphic Raycaster가 있는 Canvas여야 합니다)")]
    public Transform panelParent;

    private Dictionary<UIPanelType, string> panelPathDict;
    private Dictionary<UIPanelType, BasePanel> panelDict;
    private Stack<BasePanel> panelStack;

    void Awake()
    {
        // 싱글톤 초기화
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // JSON 파싱 로직을 Awake에서 안전하게 호출
        ParseUIPanelTypeJson();
    }

    /// <summary>
    /// 패널을 스택에 추가하고 화면에 표시합니다.
    /// </summary>
    public void PushPanel(UIPanelType panelType, params object[] _datas)
    {
        if (panelStack == null)
        {
            panelStack = new Stack<BasePanel>();
        }

        BasePanel panel = GetPanel(panelType);

        // 패널을 가져오지 못했다면 중단
        if (panel == null) return;

        if (panelStack.Count > 0)
        {
            BasePanel topPanel = panelStack.Peek();
            // 필요에 따라 이전 패널을 OnPause 처리하는 로직 추가
            // if (panel.TypeOfPanelStyle != UIPanelStyleType.WINDOW)
            //     topPanel.OnPause();
        }

        panel.transform.SetAsLastSibling(); // 가장 위에 보이도록 순서 조정
        panel.OnEnter(_datas);
        panelStack.Push(panel);
    }

    /// <summary>
    /// 가장 위에 있는 패널을 스택에서 제거하고 닫습니다.
    /// </summary>
    public void PopPanel()
    {
        if (panelStack == null || panelStack.Count <= 0) return;

        BasePanel topPanel = panelStack.Pop();
        topPanel.OnExit();

        if (panelStack.Count <= 0) return;

        BasePanel topPanel2 = panelStack.Peek();
        // 필요에 따라 이전 패널을 OnResume 처리하는 로직 추가
        // if (topPanel.TypeOfPanelStyle != UIPanelStyleType.WINDOW)
        //     topPanel2.OnResume();
    }

    /// <summary>
    /// 지정된 타입의 패널 인스턴스를 가져옵니다. 없으면 새로 생성합니다.
    /// </summary>
    private BasePanel GetPanel(UIPanelType panelType)
    {
        if (panelDict == null)
        {
            panelDict = new Dictionary<UIPanelType, BasePanel>();
        }

        // 이미 생성된 패널이 있는지 확인
        if (panelDict.TryGetValue(panelType, out BasePanel panel))
        {
            return panel;
        }
        else // 생성된 패널이 없으면 새로 생성
        {
            // 1. 경로 딕셔너리에서 프리팹 경로를 가져옴
            if (!panelPathDict.TryGetValue(panelType, out string path) || string.IsNullOrEmpty(path))
            {
                Debug.LogError($"[UIManager] {panelType}에 해당하는 경로가 UIPanelType.json에 없습니다.");
                return null;
            }

            // 2. Resources 폴더에서 프리팹을 로드
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"[UIManager] 경로에서 프리팹을 로드할 수 없습니다: {path}");
                return null;
            }

            // 3. Instantiate 시점에 부모(panelParent)를 바로 지정합니다.
            //    이것이 스케일과 위치 문제를 해결하는 가장 중요한 부분입니다.
            GameObject insPanel = Instantiate(prefab, panelParent);

            // 4. 생성된 패널의 정보를 딕셔너리에 저장하고 반환
            BasePanel newPanel = insPanel.GetComponent<BasePanel>();
            panelDict.Add(panelType, newPanel);
            return newPanel;
        }
    }

    /// <summary>
    /// Resources/UIPanelType.json 파일을 읽어 패널 경로를 초기화합니다.
    /// </summary>
    private void ParseUIPanelTypeJson()
    {
        panelPathDict = new Dictionary<UIPanelType, string>();
        TextAsset ta = Resources.Load<TextAsset>("UIPanelType");
        if (ta == null)
        {
            Debug.LogError("[UIManager] Resources 폴더에서 'UIPanelType.json' 파일을 찾을 수 없습니다.");
            return;
        }

        UIPanelTypeJson jsonObject = JsonUtility.FromJson<UIPanelTypeJson>(ta.text);
        foreach (UIPanelInfo info in jsonObject.infoList)
        {
            panelPathDict.Add(info.panelType, info.path);
        }
    }
}

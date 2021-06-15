using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HierachyUI : MonoBehaviour
{
    public ScrollRect _scrollHierarchy;
    public HierachyNodeItem _nodeTemplete;
    [HideInInspector]
    public PhoneHierachyPanel _PanelView;
    private Scene dontDestroyScene;
    private Dictionary<string, HierachyNodeItem> _sceneNodes = new Dictionary<string, HierachyNodeItem>();
    private bool _isSearchMode = false;
    public InputField _input;
    public Button _btnSearch;
    public Button _btnClear;
    void Awake()
    {
        _nodeTemplete.gameObject.SetActive(false);
        this._btnSearch.onClick.AddListener(this.OnSearch);
        this._btnClear.onClick.AddListener(this.OnClear);
    }

    private void OnClear()
    {
        if (this._isSearchMode)
        {
            this.Init();
        }
    }

    private void OnSearch()
    {
        string inputStr = this._input.text;
        List<Transform> objList = new List<Transform>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scenNode = SceneManager.GetSceneAt(i);
            GameObject[] objs = scenNode.GetRootGameObjects();
            foreach (GameObject obj in objs)
            {
                 MobileHierarchyUtils.GetChildRecursive(obj.transform, inputStr,ref objList);
            }//end foreach
        }//end for

        if (objList.Count > 0)
        {
            this._isSearchMode = true;
            this.BuildSearchList(objList);
        }
    }

    private void BuildSearchList(List<Transform> objList)
    {
        this.DestoryAll();
        foreach (Transform tran in objList)
        {
            this.AddOneNode(tran.gameObject, NodeType.Normal, this._scrollHierarchy.content.transform);
        }
    }

    public void Init()
    {
        this._input.text = "";
        this._isSearchMode = false;
        if (dontDestroyScene == null)
            dontDestroyScene = GetDontDestroyScene();

#if UNITY_EDITOR
        UnityEditor.Selection.activeGameObject = gameObject;
#endif
        CreateSceneNodes();
    }

    private Scene GetDontDestroyScene()
    {
        GameObject temp = null;
        try
        {
            temp = new GameObject();
            DontDestroyOnLoad(temp);
            Scene dontDestroyOnLoad = temp.scene;
            DestroyImmediate(temp);
            temp = null;
            return dontDestroyOnLoad;
        }
        finally
        {
            if (temp != null)
                DestroyImmediate(temp);
        }
    }

    private void DestoryAll()
    {
        foreach (HierachyNodeItem node in this._sceneNodes.Values)
        {
            if (node.gameObject != null)
                GameObject.Destroy(node.gameObject);
        }
        _sceneNodes.Clear();
    }

    private void CreateSceneNodes()
    {
        this.DestoryAll();
        
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            CreateSceneNode(SceneManager.GetSceneAt(i));
        }
        CreateSceneNode(dontDestroyScene);
    }


    private void CreateSceneNode(Scene loadingScene)
    {
        if (!loadingScene.IsValid())
            return;
        this.AddOneNode(loadingScene, NodeType.Scene, this._scrollHierarchy.content.transform);
        this.ForceRebuildHierachy();
        this.RebuildInspector(null);
    }

    public HierachyNodeItem AddOneNode(object target, NodeType type, Transform root)
    {
        GameObject obj = GameObject.Instantiate(this._nodeTemplete.gameObject);
        bool isTargetVisible = true;
        if (type == NodeType.Scene)
        {
            obj.name = ((Scene)target).name;
        }
        else
        {
            GameObject viewObj = target as GameObject;
            obj.name = viewObj.name;
            if (viewObj.activeSelf == false || viewObj.activeInHierarchy == false)
                isTargetVisible = false;
        }

        obj.transform.SetParent(root, false);
        obj.SetActive(true);
     
        string uid = MobileHierarchyUtils.GenerateUId();
        HierachyNodeItem item = obj.GetComponent<HierachyNodeItem>();
        item.Render(target, type, uid, isTargetVisible);
        item._AddOneCallBack = this.AddOneNode;
        item._ClickCallBack = this.OnClickNodeItem;
        item._RemoveCallBack = this.RemoveOneNode;
        item._RebuildCallBack = this.ForceRebuildHierachy;

        _sceneNodes.Add(uid, item);
        return item;
    }

    public void OnClickNodeItem(string uid)
    {
        foreach (HierachyNodeItem node in this._sceneNodes.Values)
        {
            if (node != null && node.gameObject != null)
                node.SetSelectedWindowId(uid);
        }

        HierachyNodeItem curNode = _sceneNodes[uid];
        if (curNode.GetNodeType() == NodeType.Normal)
        {
            this.RebuildInspector(curNode);
        }
    }

    public void RebuildInspector(HierachyNodeItem node)
    {
        this._PanelView.RebuildInspector(node);
    }

    public void RemoveOneNode(string uid)
    {
        _sceneNodes.Remove(uid);
    }

    public void ForceRebuildHierachy()
    {
        List<string> emptys = new List<string>();
        foreach (string uid in this._sceneNodes.Keys)
        {
            if (this._sceneNodes[uid] == null || this._sceneNodes[uid].gameObject == null)
                emptys.Add(uid);
        }

        foreach (string uid in emptys)
        {
            this._sceneNodes.Remove(uid);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(this._scrollHierarchy.content);
    }
}//end class
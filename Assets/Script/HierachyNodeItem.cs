using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum NodeType
{
    Scene = 1,
    Normal = 2,
};

public delegate HierachyNodeItem AddOneNodeDelegate(object arg0, NodeType arg1, Transform arg2);

public class HierachyNodeItem : MonoBehaviour
{
    public static readonly Color ACTIVE_NAME_COLOR = new Color(1f, 1f, 1f);
    public static readonly Color DEACTIVE_NAME_COLOR = new Color(0.5f, 0.5f, 0.5f);

    public GameObject _Content;
    public GameObject _SceneBg;
    public GameObject _SelectBg;

    public Button _ClickBtn;
    public GameObject _SceneIcon;
    public Button _ShowChildBtn;
    public Button _HideChildBtn;
    public Text _NameTxt;
    public RawImage _ShowIcon;
    public RawImage _HideIcon;

    public Transform _childRoot;

    [HideInInspector]
    public NodeType _type;
    [HideInInspector]
    public string _uid;
    [HideInInspector]
    public bool _isSelected = false;
    [HideInInspector]
    public bool _isRenderChild = false;
    [HideInInspector]
    public bool _isActive = false;
    private List<HierachyNodeItem> _childItems = new List<HierachyNodeItem>();

    [HideInInspector]
    public object _target;

    //delegate
    public AddOneNodeDelegate _AddOneCallBack;
    public UnityAction<string> _ClickCallBack;
    public UnityAction<string> _RemoveCallBack;
    public UnityAction _RebuildCallBack;
    private void Awake()
    {
        _ClickBtn.onClick.AddListener(OnButtonClick);
        _ShowChildBtn.onClick.AddListener(OnShowChildClick);
        _HideChildBtn.onClick.AddListener(OnHideChildClick);
    }

    public void OnButtonClick()
    {
        if (this._type == NodeType.Scene)
        {
            if (this._isRenderChild)
                this.OnHideChildClick();
            else
                this.OnShowChildClick();
            return;
        }

        if (this._isSelected)
            return;
        this._ClickCallBack.Invoke(_uid);
    }

    public void SetSelectedWindowId(string uid)
    {
        this._isSelected = uid.Equals(this._uid);
        this.SetSelectState();
    }

    public void OnShowChildClick()
    {
        if (this._isRenderChild)
            return;
        this.RenderChild();
    }

    public void OnHideChildClick()
    {
        this.DestoryChild();
    }

    public NodeType GetNodeType()
    {
        return this._type;
    }

    private void RenderChild()
    {
        this.InitChildGameObjects();
        this._isRenderChild = true;
        this._childItems.Clear();
        foreach (GameObject obj in _ChildGameobjects)
        {
            HierachyNodeItem item = this._AddOneCallBack.Invoke(obj, NodeType.Normal, this._childRoot);
            _childItems.Add(item);
        }
        this.SetChildState();
        this._RebuildCallBack.Invoke();
    }

    private void DestoryChild()
    {
        for (int i = 0; i < this._childItems.Count; ++i)
        {
            this._RemoveCallBack.Invoke(_childItems[i]._uid);
            GameObject.Destroy(_childItems[i].gameObject);
        }
        _childItems.Clear();
        this._isRenderChild = false;
        this.SetChildState();
        this._RebuildCallBack.Invoke();
        this._ChildGameobjects.Clear();
    }

    public void SetNodeActive(bool isActive)
    {
        if (this._type == NodeType.Normal)
        {
            GameObject go = this._target as GameObject;
            if (go != null)
                go.SetActive(isActive);
            
            this._isActive = isActive;
            this.SetVisibleState(_isActive);
        }
    }

    protected bool HasChild()
    {
        if (this._type == NodeType.Scene)
        {
            Scene scene = ((Scene)this._target);
            if (scene.IsValid())
            {
                var gos = scene.GetRootGameObjects();
                return gos.Length > 0;
            }
        }
        else
        {
            GameObject go = this._target as GameObject;
            return go.transform.childCount > 0;
        }
        return false;
    }

    protected List<GameObject> _ChildGameobjects = new List<GameObject>();
    protected List<GameObject> InitChildGameObjects()
    {
        _ChildGameobjects.Clear();
        if (this._type == NodeType.Scene)
        {
            Scene scene = ((Scene)this._target);
            if (scene.IsValid())
            {
                var gos = scene.GetRootGameObjects();
                _ChildGameobjects.Clear();
                for (int i = 0; i < gos.Length; i++)
                {
                    _ChildGameobjects.Add(gos[i]);
                }//end for
            }
        }
        else
        {
            _ChildGameobjects.Clear();
            GameObject go = this._target as GameObject;
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var childGo = go.transform.GetChild(i).gameObject;
                if (childGo.GetComponent<HierachyNodeItem>() == null)
                {
                    _ChildGameobjects.Add(childGo);
                }
            }//end for
        }

        return _ChildGameobjects;
    }

    private void SetVisibleState(bool isTargetVisible)
    {
        this._NameTxt.color = isTargetVisible ? HierachyNodeItem.ACTIVE_NAME_COLOR : HierachyNodeItem.DEACTIVE_NAME_COLOR;
        this._ShowIcon.color = isTargetVisible ? HierachyNodeItem.ACTIVE_NAME_COLOR : HierachyNodeItem.DEACTIVE_NAME_COLOR;
        this._HideIcon.color = isTargetVisible ? HierachyNodeItem.ACTIVE_NAME_COLOR : HierachyNodeItem.DEACTIVE_NAME_COLOR;
    }

    public void Render(object target, NodeType t, string uid, bool isTargetVisible)
    {
        this._target = target;
        this._uid = uid;
        this._type = t;
        this._isActive = isTargetVisible;

        this._SceneBg.SetActive(this._type == NodeType.Scene);
        this._SceneIcon.SetActive(this._type == NodeType.Scene);
        this._NameTxt.text = this.gameObject.name;
        this.SetVisibleState(isTargetVisible);

        this.SetChildState();
        this.SetSelectState();
    }

    public void SetSelectState()
    {
        this._SelectBg.SetActive(this._isSelected);
    }

    private void SetChildState()
    {
        bool hasChild = this.HasChild();
        this._ShowChildBtn.gameObject.SetActive(this._isRenderChild == false && hasChild);
        this._HideChildBtn.gameObject.SetActive(_isRenderChild && hasChild);
        this._childRoot.gameObject.SetActive(_isRenderChild && hasChild);
        LayoutRebuilder.ForceRebuildLayoutImmediate(this._childRoot.GetComponent<RectTransform>());

        Transform parent = this._childRoot.parent;
        while (parent != null)
        {
            if (parent.GetComponent<ContentSizeFitter>() != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(parent.GetComponent<RectTransform>());
            parent = parent.parent;
        }
    }

}//end class
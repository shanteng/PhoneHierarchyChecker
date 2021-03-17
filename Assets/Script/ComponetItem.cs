using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



public class ComponetItem : MonoBehaviour
{
    public Button _ShowChildBtn;
    public Button _HideChildBtn;

    public Button _EnableCheckBtn;
    public GameObject _CheckIcon;
    public Text _NameTxt;

    public Transform _childRoot;

    public PropertyItem _templete;
    private bool _isRenderChild = false;
    private List<PropertyItem> _childItems = new List<PropertyItem>();
    private Behaviour m_behaviour;

    [HideInInspector]
    public UnityAction _rebuildFun;

    private ComponentRenderData _data;
    private void Awake()
    {
        this._templete.gameObject.SetActive(false);
        _EnableCheckBtn.onClick.AddListener(OnEnableCheckClick);
        _ShowChildBtn.onClick.AddListener(OnShowChildClick);
        _HideChildBtn.onClick.AddListener(OnHideChildClick);
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

    private void RenderChild()
    {
        this._isRenderChild = true;
        this._childItems.Clear();
        foreach (PropertyRenderData proData in this._data.propertyDatas)
        {
            this.AddOneProperty(proData);
        }

        this.SetChildState();
        if (this._rebuildFun != null)
            this._rebuildFun.Invoke();
    }

    private void AddOneProperty(PropertyRenderData data)
    {
        GameObject obj = GameObject.Instantiate(this._templete.gameObject);
        obj.transform.SetParent(this._childRoot, false);
        obj.SetActive(true);
        PropertyItem item = obj.GetComponent<PropertyItem>();
        item.Render(data);
        this._childItems.Add(item);
    }

    private void DestoryChild()
    {
        for (int i = 0; i < this._childItems.Count; ++i)
        {
            GameObject.Destroy(_childItems[i].gameObject);
        }
        _childItems.Clear();
        this._isRenderChild = false;
        this.SetChildState();
        if (this._rebuildFun != null)
            this._rebuildFun.Invoke();
    }

    public void OnEnableCheckClick()
    {
        bool isBehaviour = this._data.component is Behaviour;
        if (isBehaviour)
        {
            this._data.isEnabled = !this._data.isEnabled;
            (this._data.component as Behaviour).enabled = this._data.isEnabled;
            this.SetCheckState();
        }
    }

    protected List<GameObject> childGameobjects = new List<GameObject>();

    public void Render(ComponentRenderData compoData)
    {
        this._data = compoData;
        m_behaviour = compoData.component as Behaviour;
        this._EnableCheckBtn.gameObject.SetActive(compoData.isBehaviour);
        this._NameTxt.text = compoData.name;
        this.SetChildState();
        this.SetCheckState();
    }

    private void SetCheckState()
    {
        if (this._data.isBehaviour == false)
            return;
        this._CheckIcon.SetActive(this._data.isEnabled);
    }

    private void SetChildState()
    {
        int count = this._data.propertyDatas.Count;
        this._ShowChildBtn.gameObject.SetActive(this._isRenderChild == false && count > 0);
        this._HideChildBtn.gameObject.SetActive(_isRenderChild && count > 0);
        this._childRoot.gameObject.SetActive(_isRenderChild && count > 0);
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhoneHierachyPanel : MonoBehaviour
{
    public Button _btnClose;
    public HierachyUI _hierachyUi;
    public InspectorUI _inspectorUi;
    public static PhoneHierachyPanel me;

    void Awake()
    {
        this._hierachyUi._PanelView = this;
        me = this;
        this._btnClose.onClick.AddListener(Hide);
    }

    private void Hide()
    {
        this.gameObject.SetActive(false);
    }


    public void Init()
    {
        //初始化Hierarchy
        this._hierachyUi.Init();
    }

    public void RebuildInspector(HierachyNodeItem node)
    {
        this._inspectorUi.RebuildInspector(node);
    }

}//end class
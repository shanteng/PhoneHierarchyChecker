using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhoneHierachyPanel : MonoBehaviour
{
    public static string PrefabName = "PhoneHierarchyPanel";

    public Button _btnClose;
    public UIDragHandler _drager;
    public HierachyUI _hierachyUi;
    public InspectorUI _inspectorUi;
    public static PhoneHierachyPanel me;
    private Vector2 mPostion = Vector2.zero;
    private RectTransform mRect;

    public Button _btnShrink;
    public Button _btnOut;

    void Awake()
    {
        this._hierachyUi._PanelView = this;
        me = this;
        this.mRect = this.GetComponent<RectTransform>();
        this._btnClose.onClick.AddListener(Hide);
        this._btnShrink.onClick.AddListener(OnShrink);
        this._btnOut.onClick.AddListener(OnShrink);


        this._drager._beginCall = this.OnBeginDrag;
        this._drager._dragCall = this.OnDrag;
        this._drager._endCall = this.OnEndDrag;
        this.gameObject.name = PhoneHierachyPanel.PrefabName;
    }

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData)
    {
        mPostion.x += eventData.delta.x;
        mPostion.y += eventData.delta.y;
        this.mRect.anchoredPosition = mPostion;
    }

    public void OnEndDrag(PointerEventData eventData) { }


    private void OnShrink()
    {
        bool isVis = this._inspectorUi.gameObject.activeSelf;
        this._inspectorUi.gameObject.SetActive(!isVis);
        this._btnOut.gameObject.SetActive(isVis);
        this._btnShrink.gameObject.SetActive(!isVis);
    }

    private void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Init()
    {
        //初始化Hierarchy
        this._hierachyUi.Init();
        this._btnOut.gameObject.SetActive(false);
        this._btnShrink.gameObject.SetActive(true);
    }

    public void RebuildInspector(HierachyNodeItem node)
    {
        this._inspectorUi.RebuildInspector(node);
    }

}//end class
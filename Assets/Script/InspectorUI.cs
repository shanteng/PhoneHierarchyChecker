using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectorUI : MonoBehaviour
{
    public ScrollRect _scrollInspector;
    public GameObjectItem _InspectorIem;
    public ComponetItem _ComponetItemTemplete;
    private List<ComponetItem> componentNodes = new List<ComponetItem>();
    private HierachyNodeItem _selectNodeItem;
    void Awake()
    {
        _ComponetItemTemplete.gameObject.SetActive(false);
    }

    public void RebuildInspector(HierachyNodeItem node)
    {
        _selectNodeItem = node;
        for (int i = 0; i < componentNodes.Count; i++)
        {
            GameObject.Destroy(componentNodes[i].gameObject);
        }
        this.componentNodes.Clear();
        this._InspectorIem.gameObject.SetActive(this._selectNodeItem != null);

        if (this._selectNodeItem == null)
            return;

        GameObject targetObj = _selectNodeItem._target as GameObject;
        if (targetObj == null)
            return;

        this._InspectorIem.Render(this._selectNodeItem, targetObj);
        //取出所有的Component
        var comps = targetObj.GetComponents<Component>();
        for (int i = 0; i < comps.Length; i++)
        {
            ComponentRenderData compoData = new ComponentRenderData();
            compoData.CopyFromComponent(comps[i]);
            this.CreateOneComponent(compoData);
        }//end for

        this.ForceRebuildInspector();
    }

    private void CreateOneComponent(ComponentRenderData compoData)
    {
        GameObject obj = GameObject.Instantiate(this._ComponetItemTemplete.gameObject);
        obj.transform.SetParent(this._scrollInspector.content.transform, false);
        obj.SetActive(true);
        string uid = MobileHierarchyUtils.GenerateUId();
        ComponetItem item = obj.GetComponent<ComponetItem>();
        item.Render(compoData);
        item._rebuildFun = this.ForceRebuildInspector;
        this.componentNodes.Add(item);
    }

    public void ForceRebuildInspector()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(this._scrollInspector.content);
    }

}//end class
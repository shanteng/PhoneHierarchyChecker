using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectItem : MonoBehaviour
{
    public Button _EnableCheckBtn;
    public GameObject _CheckIcon;
    public Text _NameTxt;

    private HierachyNodeItem _node;
    private void Awake()
    {
        _EnableCheckBtn.onClick.AddListener(OnEnableCheckClick);
    }

    public void OnEnableCheckClick()
    {
        this._node._isActive = !this._node._isActive;
        this._node.SetNodeActive(this._node._isActive);
        this._CheckIcon.SetActive(_node._isActive);
    }

    public void Render(HierachyNodeItem node, GameObject targetObj)
    {
        this._node = node;
        this._CheckIcon.SetActive(node._isActive);
        this._NameTxt.text = targetObj.name;
    }


}//end class
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyItem : MonoBehaviour
{
    public Text _NameTxt;
    public RectTransform _rectValueBg;
    public Text _ValueTxt;

    private void Awake()
    {

    }

    public void Render(PropertyRenderData data)
    {
        _NameTxt.text = data.name;
        _ValueTxt.text = data.value;
        LayoutRebuilder.ForceRebuildLayoutImmediate(this._ValueTxt.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectValueBg);
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.GetComponent<RectTransform>());
    } 

}//end class
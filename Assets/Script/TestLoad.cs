using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLoad : MonoBehaviour
{
    public Button _btnLoad;
    public Transform _Canvas;
    private PhoneHierachyPanel _view;

    private void Awake()
    {
        this._btnLoad.onClick.AddListener(LoadShowPanel);
    }

    private void LoadShowPanel()
    {
        if (this._view == null)
        {
            GameObject obj = Resources.Load<GameObject>(PhoneHierachyPanel.PrefabName);
            GameObject ui = GameObject.Instantiate(obj, _Canvas, false);
          
            /*   var offmini = rectForm.offsetMin;
               var offmax = rectForm.offsetMax;
               rectForm.offsetMax = Vector2.zero;
               rectForm.offsetMin = Vector2.zero;
               rectForm.localScale = Vector3.one;
               rectForm.localPosition = Vector3.zero;
            */
            this._view = ui.GetComponent<PhoneHierachyPanel>();
        }

        this._view.transform.SetAsLastSibling();
        this._view.gameObject.SetActive(true);
     
        this._view.Init();
        StartCoroutine(WaitRebuild());
    }

    IEnumerator WaitRebuild()
    {
        yield return new WaitForFixedUpdate();
        var rectForm = _view.gameObject.GetComponent<RectTransform>();
        float halfWidth = rectForm.sizeDelta.x / 2;
        rectForm.anchoredPosition = new Vector2(-halfWidth, 0);
    }
}//end class
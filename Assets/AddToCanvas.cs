using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AddToCanvas : MonoBehaviour
{
    CanvasGroup _canvasGroup;
    #region MonoBehaviour Callbacks

    void Awake() {
        _canvasGroup = this.GetComponent<CanvasGroup>();
        StartCoroutine(waitForGameToLoad());
    }
    #endregion
    IEnumerator waitForGameToLoad(){
        yield return new WaitForSeconds(0.5f);
        this.transform.SetParent(GameObject.Find("Canvas In Game").GetComponent<Transform>(), false);
		GetComponent<RectTransform>().anchoredPosition = new Vector2(0f,0f);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0f,0f);
        GetComponent<RectTransform>().sizeDelta = new Vector2(100f,100f);
        
    }
    void Update() {

    }

}

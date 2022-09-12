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
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }
    #endregion

}

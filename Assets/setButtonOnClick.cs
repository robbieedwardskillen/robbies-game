using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor.Events;//yes these two aren't the same


public class setButtonOnClick : MonoBehaviour
{
    GameObject modalWindows;
    void Awake() {
        modalWindows = GameObject.Find("Canvas/Modal Windows");
    }
    // Start is called before the first frame update
    void Start()
    {
        UnityEventTools.AddPersistentListener(GetComponent<Button>().onClick, new UnityAction(testMethod));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void testMethod() {
        print("test");
    }
}

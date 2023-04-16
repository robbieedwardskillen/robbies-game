using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
#if UNITY_EDITOR

using UnityEditor.Events;//not to be confused with UnityEngine.Events..

#endif



public class setButtonOnClick : MonoBehaviour
{
    GameObject modalWindows;
    void Awake() {
        modalWindows = GameObject.Find("Canvas/Modal Windows");
    }
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR

        UnityEventTools.AddPersistentListener(GetComponent<Button>().onClick, new UnityAction(testMethod));

#endif

        


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void testMethod() {
        print("test");
    }
}

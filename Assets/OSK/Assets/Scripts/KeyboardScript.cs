using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class KeyboardScript : MonoBehaviour
{
    public GameObject inputField;
    public TextMeshProUGUI TextField;
    public GameObject RusLayoutSml, RusLayoutBig, EngLayoutSml, EngLayoutBig, SymbLayout;
    

    public void alphabetFunction(string alphabet)
    {

        inputField.GetComponent<TMP_InputField>().text = inputField.GetComponent<TMP_InputField>().text + alphabet;
        //TextField.text=TextField.text + alphabet;

    }

    public void BackSpace()
    {

        if(inputField.GetComponent<TMP_InputField>().text.Length>0) inputField.GetComponent<TMP_InputField>().text= inputField.GetComponent<TMP_InputField>().text.Remove(inputField.GetComponent<TMP_InputField>().text.Length-1);

    }

    public void CloseAllLayouts()
    {

        RusLayoutSml.SetActive(false);
        RusLayoutBig.SetActive(false);
        EngLayoutSml.SetActive(false);
        EngLayoutBig.SetActive(false);
        SymbLayout.SetActive(false);

    }

    public void ShowLayout(GameObject SetLayout)
    {

        CloseAllLayouts();
        SetLayout.SetActive(true);

    }

}

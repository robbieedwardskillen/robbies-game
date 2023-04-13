using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class radialMenuElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI elementName;
    public Image image;
    public Image image2;
    private RectTransform rect;
    private void Start() {
        rect = GetComponent<RectTransform>();

    }
    public void SetName(string name){
        this.elementName.text = name;
    }
    public string GetName(){
        return this.elementName.text;
    }
    public void SetImage (Sprite spr){
        image.sprite = spr;
    }
    public Sprite GetImage (){
        return image.sprite;
    }
    public void SetImage2 (Sprite spr){
        image2.sprite = spr;
    }
    public Sprite GetImage2 (){
        return image2.sprite;
    }
    public void OnPointerClick(PointerEventData eventData){

    }
    public void OnPointerEnter(PointerEventData eventData){
        transform.parent.gameObject.transform.Find("mask").gameObject.transform.Find("CardImage").GetComponent<Image>().enabled = true;
        transform.parent.gameObject.transform.Find("mask").gameObject.transform.Find("CardImage").GetComponent<Image>().sprite = image2.sprite;
        GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f ,0f);
        
    }
    public void OnPointerExit(PointerEventData eventData){
        GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0f);
    }

}

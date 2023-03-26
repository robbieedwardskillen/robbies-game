using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class radialMenuElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI elementName;
    public RawImage image;
    public Image image2;
    private RectTransform rect;
    private void Start() {
        rect = GetComponent<RectTransform>();

    }
    public void SetName(string name){
        this.elementName.text = name;
    }
    public void SetImage (Texture img){
        image.texture = img;
    }
    public Texture GetImage (){
        return image.texture;
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
        GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f ,0f);
        
    }
    public void OnPointerExit(PointerEventData eventData){
        GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 0f);
    }

}

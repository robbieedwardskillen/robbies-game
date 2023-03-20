using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class radialMenuElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string elementName;
    public RawImage image;
    private RectTransform rect;
    private void Start() {
        rect = GetComponent<RectTransform>();
    }
    public void SetName(string name){
        this.elementName = name;
    }
    public void SetImage (Texture img){
        image.texture = img;
    }
    public Texture GetImage (){
        return image.texture;
    }
    public void OnPointerClick(PointerEventData eventData){

    }
    public void OnPointerEnter(PointerEventData eventData){
        print("enter");
        GetComponent<RectTransform>().localScale = new Vector3(2,2,0f);
    }
    public void OnPointerExit(PointerEventData eventData){
        print("exit");
        GetComponent<RectTransform>().localScale = new Vector3(1,1,0f);
    }
}

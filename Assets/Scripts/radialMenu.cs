using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class radialMenu : MonoBehaviour
{
    public GameObject elementPrefab;

    public float radius = 75f;

    public List<Sprite> images;
    public List<Sprite> images2;
    public Sprite placeholderImage1;
    public Sprite placeholderImage2;
    public List<radialMenuElement> elements;
    // Start is called before the first frame update
    void Awake()
    {
        elements = new List<radialMenuElement>();
        Open();
        transform.Find("mask").gameObject.transform.Find("CardImage").GetComponent<Image>().enabled = false;
    }

    void addElement(string name, Sprite img, Sprite img2){
        GameObject element = Instantiate(elementPrefab, transform);
        radialMenuElement rme = element.GetComponent<radialMenuElement>();
        rme.SetName(name);
        rme.SetImage(img);
        rme.SetImage2(img2);
        elements.Add(rme);
    }
    public void Open(){
        for (int i=0; i<8; i++){ //elements.Count
            int actualNumber = i + 1;
            if (images[i] == null || images2[i] == null){
                addElement("Card" + actualNumber.ToString(), placeholderImage1, placeholderImage2);
            }
            else {
                addElement("Card" + actualNumber.ToString(), images[i], images2[i]);
            }

        }
        Rearrange();
        //change to 8 individual cards
    }
    void Rearrange(){
        float radiansOfSeparation = (Mathf.PI * 2) / elements.Count;
        for (int i = 0; i < elements.Count; i++){
            float x = Mathf.Sin(radiansOfSeparation * i) * radius;
            float y = Mathf.Cos(radiansOfSeparation * i) * radius;
            elements[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0f);
        }
    }
}

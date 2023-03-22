using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class radialMenu : MonoBehaviour
{
    public GameObject elementPrefab;

    public float radius = 75f;

    public List<Texture> images;

    List<radialMenuElement> elements;
    // Start is called before the first frame update
    void Start()
    {
        elements = new List<radialMenuElement>();
        Open();
    }

    void addElement(string name, Texture img){
        GameObject element = Instantiate(elementPrefab, transform);
        radialMenuElement rme = element.GetComponent<radialMenuElement>();
        rme.SetName(name);
        rme.SetImage(img);
        elements.Add(rme);
    }
    public void Open(){
        for (int i=0; i<6; i++){
            if (images[i] != null)
            addElement("Card" + i.ToString(), images[i]);
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

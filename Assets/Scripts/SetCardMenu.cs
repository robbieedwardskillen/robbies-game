using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.Shift;
public class SetCardMenu : MonoBehaviour
{
    private RadialMenu rm;
    public GameObject cardPrefab;
    private GameObject card;
    // Start is called before the first frame update
    void Awake()
    {
        rm = GameObject.Find("Canvas In Game/RadialMenu").GetComponent<RadialMenu>();
        for (int i = 0; i < rm.elements.Count; i++){
            card = Instantiate(cardPrefab) as GameObject;
            card.GetComponent<ChapterButton>().backgroundImage = rm.elements[i].image.sprite;
            card.transform.parent = this.gameObject.transform;
            card.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f ,1f);
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.Shift;
public class SetCardMenu : MonoBehaviour
{
    private radialMenu rm;
    public GameObject cardPrefab;
    private GameObject card;
    // Start is called before the first frame update
    void Start()
    {
        rm = GameObject.Find("Canvas In Game/RadialMenu").GetComponent<radialMenu>();
        for (int i = 0; i < rm.elements.Count; i++){
            card = Instantiate(cardPrefab) as GameObject;
            card.GetComponent<ChapterButton>().backgroundImage = rm.elements[i].image.sprite;
            card.transform.parent = this.gameObject.transform;
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

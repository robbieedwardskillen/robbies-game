using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Michsky.UI.Shift
{
    public class VirtualCursor : PointerInputModule
    {
        private GameObject keyboard;
        private GameObject keyboardEnglishBig;
        private List<RaycastResult> rr;
        [Header("Objects")]
        public RectTransform border;
        public GameObject cursorObject;

        [Header("Input")]
        public EventSystem vEventSystem;
        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";

        [Header("Settings")]
        [Range(0.1f, 5)] public float speed = 1;

        Animator cursorAnim;
        PointerEventData pointer;
        Vector2 cursorPos;
        RectTransform cursorObj;

        private ManageCanvas manageCanvas;

        public new void Start()
        {
            manageCanvas = GameObject.Find("Canvas Manager").GetComponent<ManageCanvas>();
            rr = new List<RaycastResult>();
            cursorObj = this.GetComponent<RectTransform>();
            pointer = new PointerEventData(vEventSystem);
            cursorAnim = cursorObject.GetComponent<Animator>();
            keyboard = GameObject.Find("/Canvas/OnScreenKeyboard");
            keyboardEnglishBig = GameObject.Find("/Canvas/OnScreenKeyboard/kb_eng_bg");
        }
        public static Vector2 WorldToScreenPoint(Camera cam, Vector3 worldPoint)
        {
            if ((Object) cam == (Object) null)
                return new Vector2(worldPoint.x, worldPoint.y);
            return (Vector2) cam.WorldToScreenPoint(worldPoint);
        }
        
        void Update()
        {
            cursorPos.x += Input.GetAxis(horizontalAxis) * speed * 1000 * Time.deltaTime;
            cursorPos.x = Mathf.Clamp(cursorPos.x, -+border.rect.width / 2, border.rect.width / 2);

            cursorPos.y += Input.GetAxis(verticalAxis) * speed * 1000 * Time.deltaTime;
            cursorPos.y = Mathf.Clamp(cursorPos.y, -+border.rect.height / 2, border.rect.height / 2);

            cursorObj.anchoredPosition = cursorPos;
            if (manageCanvas.myPlayerInstantiated)
                this.Process();
        }

        public override void Process()
        {  
            Vector2 screenPos = Vector2.zero;
/*             if (manageCanvas.inGame){
                screenPos = Camera.main.WorldToScreenPoint(cursorObj.transform.position);
            } else { */
                screenPos = WorldToScreenPoint(null, cursorObj.transform.position);
            //}
            pointer.position = screenPos;
            eventSystem.RaycastAll(pointer, this.m_RaycastResultCache);
            RaycastResult raycastResult = FindFirstRaycast(this.m_RaycastResultCache);
            //test

            
            //end test
            pointer.pointerCurrentRaycast = raycastResult;
            this.ProcessMove(pointer);

            if (Input.GetButtonDown("Submit"))
            {   
                pointer.pressPosition = cursorPos;
                pointer.clickTime = Time.unscaledTime;
                pointer.pointerPressRaycast = raycastResult;
/*                 var output = JsonUtility.ToJson(raycastResult, true);
                foreach (var i in this.m_RaycastResultCache){
                    Debug.Log(i);
                } */
		    
                if (this.m_RaycastResultCache.Count > 0)
                {  
                    //-Robbie added so it doesn't deselect needs to only be in input page
                    if (raycastResult.gameObject.GetComponent<TMP_InputField>() != null || raycastResult.gameObject.name == "Text") {
                        pointer.selectedObject = raycastResult.gameObject;

                        if (raycastResult.gameObject.GetComponent<TextMeshProUGUI>().text != "LOGIN") {
                            keyboard.SetActive(true);

                            if (keyboard.activeSelf == true)
                                {
                                    GameObject.Find("/Canvas/OnScreenKeyboard/kb_eng_bg").SetActive(true);
                                    keyboard.GetComponent<KeyboardScript>().ShowLayout(keyboardEnglishBig);
                                }
                        }

                    } else {
                        if (raycastResult.gameObject.GetComponent<Button>() == null){
                            if (raycastResult.gameObject.name != "kb_symb" && raycastResult.gameObject.name != "kb_rus_sml" && raycastResult.gameObject.name != "kb_rus_bg"
                                 && raycastResult.gameObject.name != "kb_eng_sml" && raycastResult.gameObject.name != "kb_eng_bg") {
                                    keyboard.GetComponent<KeyboardScript>().CloseAllLayouts();
                                }
                        } else if (raycastResult.gameObject.name == "Button (44) Enter") {
                                keyboard.GetComponent<KeyboardScript>().CloseAllLayouts();
                        }
                            
                    }

                    pointer.pointerPress = ExecuteEvents.ExecuteHierarchy(raycastResult.gameObject, pointer, ExecuteEvents.submitHandler);
                    pointer.rawPointerPress = raycastResult.gameObject;
                }

                else
                {
                    pointer.rawPointerPress = null;
                }
                    
            }

            else
            {
                pointer.pointerPress = null;
                pointer.rawPointerPress = null;
            }
        }

        public void AnimateCursorIn()
        {
            if (gameObject.activeSelf == true)
                cursorAnim.Play("In");
        }

        public void AnimateCursorOut()
        {
            if (gameObject.activeSelf == true)
                cursorAnim.Play("Out");
        }
    }
}
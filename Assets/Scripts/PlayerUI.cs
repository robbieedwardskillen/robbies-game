using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerUI : MonoBehaviour
{
    #region Public Fields
    [Tooltip("Pixel offset from the player target")]
    [SerializeField]
    private Vector3 screenOffset = new Vector3(0f,30f,0f);
    [Tooltip("UI Text to display Player's Name")]
    [SerializeField]
    private TextMeshProUGUI playerNameText;

    [Tooltip("UI Slider to display Player's Health")]
    [SerializeField]
    private Slider playerHealthSlider;
    #endregion

    #region Private Fields
    private PlayerManager target;
    float characterControllerHeight = 0f;
    Transform targetTransform;
    Renderer targetRenderer;
    CanvasGroup _canvasGroup;
    Vector3 targetPosition;
    #endregion

    #region MonoBehaviour Callbacks

    void Start()
    {
        _canvasGroup = this.GetComponent<CanvasGroup>();
        this.transform.SetParent(GameObject.Find("Canvas In Game").GetComponent<Transform>(), false);
    }

    void Update()
    {
        // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }
        // Reflect the Player Health
        if (playerHealthSlider != null)
        {
            playerHealthSlider.value = target.Health;
        }
    }

    void FixedUpdate()//suppoed to be LateUpdate but changing it for Cinemachine 
    {
        // Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
        if (targetRenderer!=null)
        {
            this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        }
        // #Critical
        // Follow the Target GameObject on screen.
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;
            //this.transform.position = Camera.main.WorldToScreenPoint (targetPosition) + screenOffset;
            this.transform.position = Camera.main.WorldToScreenPoint (targetPosition) + screenOffset;
        }
    }

    #endregion


    #region Public Methods

    public void SetTarget(PlayerManager _target)
    { 
        if (_target == null)
        {   
            //Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }
        // Cache references for efficiency
        target = _target;

        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponent<Renderer>();

        characterControllerHeight = target.Height; 

        if (playerNameText != null)
        {
            playerNameText.GetComponent<TextMeshProUGUI>().text = target.photonView.Owner.NickName;
        }
    }

    #endregion


    
}


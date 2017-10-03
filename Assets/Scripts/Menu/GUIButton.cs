using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menus
{
    public class GUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
      IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        // Audio clip that is played when the button is clicked.
        public AudioClip OnClicked;

        // General list of every game object that is currently being
        // hovered over.
        public static List<GameObject> allActiveButtons = new List<GameObject>();

        // How much the scale oscilates in either direction while the button hovers.
        const float ButtonScaleRange = 0.15f;
        // The frequency of the oscilations, in oscilations-per-2Pi seconds.
        const float ButtonScaleFrequency = 6.0f;
        // How the scale increase when the button is being pressed.
        const float ButtonScalePressed = 0.5f;
        // How fast the scale transitions when changing states, in %-per-frame.
        const float transitionSpeed = 0.09f;

        //  True if the mouse/pointer is hovering over this element.
        public bool hover = false;
        //  True if the mouse/pointer is pressing this element.
        public bool press = false;

        float currentScale = 1.0f;
        float hoverStartTime;
        Vector3 startingScale;

        private void Awake()
        {
            startingScale = transform.localScale;
        }

        private void OnDestroy()
        {
            allActiveButtons.Remove(gameObject);
        }

        private void Update()
        {
            float targetScale = 1.0f;
            if (press)
            {
                targetScale = 1.0f + ButtonScalePressed;
            }
            else if (hover)
            {
                targetScale = 1.0f + ButtonScaleRange + Mathf.Cos(
                    (hoverStartTime - Time.realtimeSinceStartup) * ButtonScaleFrequency) *
                    ButtonScaleRange;
            }
            currentScale = currentScale * (1.0f - transitionSpeed) + targetScale * transitionSpeed;
            transform.localScale = startingScale * currentScale;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            CommonData.mainManager.stateManager.HandleUIEvent(gameObject, null);
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            press = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            hover = false;
            press = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hoverStartTime = Time.realtimeSinceStartup;
            hover = true;
            allActiveButtons.Add(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hover = false;
            allActiveButtons.Remove(gameObject);
        }
    }

}
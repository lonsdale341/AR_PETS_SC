using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menus
{
    public class BaseMenu : MonoBehaviour
    {

        const float MenuRenderDepth = 10.0f;
        static Vector3 MenuOffset = new Vector3(0, 0, MenuRenderDepth);

        private void Awake()
        {
            Canvas canvas = GetComponent<Canvas>();
            RectTransform rt = canvas.GetComponent<RectTransform>();
            rt.SetPositionAndRotation(MenuOffset, Quaternion.identity);
            if (canvas == null)
            {
                // Prefabs that use this class are required to
                // have a canvas component.
                Debug.LogError("UI Menu could not find canvas!");
            }
            else
            {
                // Set up canvas input.
                gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                
                    
            }
        }
    }
}

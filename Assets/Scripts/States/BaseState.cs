using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    public class BaseState
    {

        // Depth to spawn UI, when running in mobile mode.
        private const float UIDepthMobile = 6.0f;


        public StateManager manager;

        // The actual object representing any UI used by the state.
        protected GameObject gui;
        //  Метод инициализации. Вызывается после состояния
        //Добавляется в стек.
        public virtual void Initialize()
        {
        }

        //Функция очистки. Вызывается непосредственно перед тем
        //как состояние Удаляится из стека. Возвращает необязательный
        //StateExitValue
        public virtual StateExitValue Cleanup()
        {
            return null;
        }

        //Приостанавливает состояние. Вызывается, когда над ним появляется что-то новое.
        public virtual void Suspend()
        {
        }

        // Возобновkztncz состояние.
        // Вызывается, когда состояние становится активным при удалении верхнего состояния.
        // Это состояние может отправлять необязательный объект, содержащий любые результаты / данные. Результаты также могут быть пустыми, если данные не отправляются.
        public virtual void Resume(StateExitValue results)
        {
        }

        // Called once per frame when the state is active.
        public virtual void Update()
        {
        }

        // Called at fixed timesteps during gameplay.  Will not normally
        // be called during UI, as timestep is set to 0 during those times.
        public virtual void FixedUpdate()
        {
        }

        // Called by buttons and other UI elements, when they trigger.
        public virtual void HandleUIEvent(GameObject source, object eventData)
        {
        }

        // Spawn a UI Prefab, and return the component associated with it.
        protected T SpawnUI<T>(string prefabLookup)
        {
            gui = GameObject.Instantiate(CommonData.prefabs.menuLookup[prefabLookup]);
            gui.transform.position = new Vector3(0, 0, UIDepthMobile);
            gui.transform.SetParent(CommonData.canvasHolder.transform, false);
           // ResizeUI(gui);
            return gui.GetComponent<T>();
        }

        // Sets the size of the UI and makes sure it fits on the screen, etc.
        protected virtual void ResizeUI(GameObject gui)
        {
            Camera camera = CommonData.canvasHolder.GetComponentInChildren<Camera>();
            RectTransform rt = gui.GetComponent<RectTransform>();
            Vector2 lowerLeft =
                camera.WorldToScreenPoint(rt.TransformPoint(rt.anchorMin + rt.offsetMin));
            Vector2 upperRight =
                camera.WorldToScreenPoint(rt.TransformPoint(rt.anchorMax + rt.offsetMax));

            float totalWidth = Mathf.Abs(upperRight.x - lowerLeft.x);
            float totalHeight = Mathf.Abs(upperRight.y - lowerLeft.y);
            float guiScale = 1.0f;
            if (totalWidth > Screen.width)
            {
                guiScale = Screen.width / totalWidth;
            }
            if (totalHeight > Screen.height)
            {
                guiScale = Math.Min(Screen.height / totalHeight, guiScale);
            }
            gui.transform.localScale *= guiScale;
        }

        // Shows any UI that is currently hidden.
        protected void ShowUI()
        {
            if (gui != null)
            {
                gui.SetActive(true);
            }
        }

        // Hides all active GUI, and clears it from the ActiveButtonList.
        protected void HideUI()
        {
            if (gui != null)
            {
                gui.SetActive(false);
            }
            Menus.GUIButton.allActiveButtons.Clear();
        }


        // Destroy the UI prefab.
        protected void DestroyUI()
        {
            if (gui != null)
            {
                GameObject.Destroy(gui);
                gui = null;
            }
        }
    }

    // Когда состояния выходят, они могут возвращать необязательный объект данных во все, что является следующим состоянием
    // Вниз по стеку. В первую очередь полезно для состояний, таких как да / нет диалоговые окна и т. Д. Этот класс представляет эти возвращаемые значения.
    public class StateExitValue
    {
        public StateExitValue(Type sourceState, object data = null)
        {
            this.data = data;
            this.sourceState = sourceState;
        }

        public Type sourceState;
        public object data;
    }
}

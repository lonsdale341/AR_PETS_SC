using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    class WaitForTask : BaseState
    {

        protected bool isComplete = false;
        string waitText;
        bool useDots;
        System.Threading.Tasks.Task task;

        Menus.SingleLabelGUI menuComponent;

        public WaitForTask(System.Threading.Tasks.Task task, string waitText = "",
                           bool useDots = false)
        {
            this.waitText = waitText;
            this.task = task;
            this.useDots = useDots;
        }

        public override void Initialize()
        {
            Debug.Log("Init WaitForTask");
            menuComponent = SpawnUI<Menus.SingleLabelGUI>(StringConstants.PrefabsSingleLabelMenu);
            UpdateLabelText();
        }

        // Called once per frame when the state is active.
        public override void Update()
        {
            if (task.IsCompleted)
            {
                manager.PopState();
            }
            else
            {
                UpdateLabelText();
            }
        }

        private void UpdateLabelText()
        {
            if (menuComponent != null)
            {
                menuComponent.LabelText.text =
                  waitText + (useDots ? Utilities.StringHelper.CycleDots() : "");
            }
        }

        public override StateExitValue Cleanup()
        {
            DestroyUI();
            return new StateExitValue(typeof(WaitForTask), new Results(task));
        }

        // Class for encapsulating the results of the database load, as
        // well as information about whether the load was successful
        // or not.
        public class Results
        {
            public System.Threading.Tasks.Task task;

            public Results(System.Threading.Tasks.Task task)
            {
                this.task = task;
            }
        }
    }

}
 

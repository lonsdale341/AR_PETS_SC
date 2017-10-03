using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    class WaitingForDBLoad<T> : BaseState
    {

        protected bool isComplete = false;
        protected bool wasSuccessful = false;
        protected T result = default(T);
        protected string path;
        protected int failedFetches = 0;

        const int MaxDatabaseRetries = 5;

        Firebase.Database.FirebaseDatabase database;

        Menus.SingleLabelGUI menuComponent;

        public WaitingForDBLoad(string path)
        {
            this.path = path;
        }

        // Initialization method.  Called after the state
        // is added to the stack.
        public override void Initialize()
        {
            Debug.Log("Init WaitingForDB");
            database = Firebase.Database.FirebaseDatabase.GetInstance(CommonData.app);
            database.GetReference(path).GetValueAsync().ContinueWith(HandleResult);
            menuComponent = SpawnUI<Menus.SingleLabelGUI>(StringConstants.PrefabsSingleLabelMenu);
            UpdateLabelText();
        }

        protected virtual void HandleResult(
            System.Threading.Tasks.Task<Firebase.Database.DataSnapshot> task)
        {
            
            if (task.IsFaulted)
            {
                Debug.Log("HandleResult_1 WaitingForDB");
                HandleFaultedFetch(task);
                return;
            }
            else if (task.IsCompleted)
            {
                Debug.Log("HandleResult_2 WaitingForDB");
                wasSuccessful = true;
                if (task.Result != null)
                {
                    string json = task.Result.GetRawJsonValue();
                    if (!string.IsNullOrEmpty(json))
                    {
                        Debug.Log(json);
                        result = JsonUtility.FromJson<T>(json);
                    }
                }
            }
            isComplete = true;
        }

        // Called once per frame when the state is active.
        public override void Update()
        {
            Debug.Log("UPDATE");
            if (isComplete)
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
            // Debug.Log("UpdateLabelText WaitingForDB");
            if (menuComponent != null)
            {
                menuComponent.LabelText.text =
                  StringConstants.LabelLoading + Utilities.StringHelper.CycleDots();
            }
        }

        // If a fetch from the database comes back failed, try again, until the
        // maximum number of retries have been reached.  Failures are most often
        // caused by connectivity issues or database access rules.
        //Если выборка из базы данных не удалась, повторите попытку, 
        //пока не будет достигнуто максимальное количество попыток.
        // Отказы чаще всего возникают из-за проблем с подключением или правил доступа к базе данных.
        protected void HandleFaultedFetch(
            System.Threading.Tasks.Task<Firebase.Database.DataSnapshot> task)
        {
            Debug.Log("HandleFaultedFetch WaitingForDB");
            Debug.LogError("Database exception!  Path = [" + path + "]\n" + task.Exception);
            // Retry after failure.
            if (failedFetches++ < MaxDatabaseRetries)
            {
                database.GetReference(path).GetValueAsync().ContinueWith(HandleResult);
            }
            else
            {
                // Too many failures.  Exit the state, with wasSuccessful set to false.
                isComplete = true;
            }
        }

        public override StateExitValue Cleanup()
        {
            DestroyUI();
            return new StateExitValue(
              typeof(WaitingForDBLoad<T>), new Results(path, result, wasSuccessful));
        }

        // Class for encapsulating the results of the database load, as
        // well as information about whether the load was successful
        // or not.
        public class Results
        {

            public string path;
            public T results;
            public bool wasSuccessful;

            public Results(string path, T results, bool wasSuccessful)
            {
                this.path = path;
                this.results = results;
                this.wasSuccessful = wasSuccessful;
            }
        }
    }


}
 
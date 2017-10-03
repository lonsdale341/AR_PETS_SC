
using System.Collections.Generic;
//using NUnit.Framework;
using UnityEngine;
using System.Collections;
using Vuforia;

public class PoseController : MonoBehaviour
{
    #region PUBLIC_MEMBERS

    #endregion //PUBLIC_MEMBERS

    public List<GameObject> ListModels;
    
    #region PRIVATE_MEMBERS
    [SerializeField]
    private UDTEventHandler udtEventHandler;
    [SerializeField]
    private ProximityDetector proximityDetector;
    [SerializeField]
    private Collider CheckCollider;

  
    public GameObject msgTapTheCircle;
    public GameObject msgTryAgain;
    public GameObject msgGetCloser;
    public GameObject ModelContainer;
    public GameObject Center;
    public GameObject Left;
    public GameObject Right;
    public RuntimeAnimatorController catAnimatioController;
    public RuntimeAnimatorController puppyAnimatioController;
    public GameObject Target;
    public  enum TrackingMode
    {
        CONSTRAINED_TO_CAMERA,
        UDT_BASED,
        IDLE
    }

    // initial mode
    public  TrackingMode mTrackingMode ;

    private Vector3 mPosOffsetAtTargetCreation;

    private const float mInitialDistance = 2.5f;

    private bool mBuildingUDT = false;

    private Camera cam;

    public GameObject SelectModel;

    private Animator animPet;
    public string[] anim;
    public int NumberAnimation;
    #endregion //PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS

    void Awake()
    {

       // msgTapTheCircle.SetActive(true);
       // msgTryAgain.SetActive(false);
       // msgGetCloser.SetActive(false);

    }

    void Start()
    {

        mTrackingMode = TrackingMode.IDLE;
        ListModels=new List<GameObject>();
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);


       

    }

    void Update()
    {
        if (CheckTapOnObject())
        {
            Debug.Log("PUT");
            ChangeMode();

        }


    }

    void SpawnMyModel(string mode)
    {
        if (mode=="simple")
        {
           // GameObject model = GameObject.Instantiate(CommonData.prefabs.menuLookup[CommonData.nameMyPet]);
            GameObject model = GameObject.Instantiate(CommonData.prefabs.menuLookup[CommonData.currentUser.data.nameMyPet]);
            model.transform.SetParent(Center.transform, false);
            model.transform.localPosition = new Vector3(0,0,0);
            model.GetComponentInChildren<LabelController>().Init(Target,"My pet"); 
            ListModels.Add(model);
        }
        if (mode=="facebook")
        {
            //GameObject model = GameObject.Instantiate(CommonData.prefabs.menuLookup[CommonData.nameMyPet]);
            GameObject model = GameObject.Instantiate(CommonData.prefabs.menuLookup[CommonData.currentUser.data.nameMyPet]);
            model.transform.SetParent(Left.transform, false);
            model.transform.localPosition = new Vector3(0, 0, 0);
            model.GetComponentInChildren<LabelController>().Init(Target, "My pet"); 
            ListModels.Add(model);
            RuntimeAnimatorController animController = catAnimatioController;
            if (CommonData.currentUser.data.nameFriendPet.StartsWith("puppy"))
            {
                animController = puppyAnimatioController;
            }
          // string name = CommonData.nameMyPet;
          // 
          // while (name==CommonData.nameMyPet)
          // {
          //     name = "";
          //     if (Random.RandomRange(1,3)==1)
          //     {
          //         name = "cat_";
          //         animController = catAnimatioController;
          //     }
          //     else
          //     {
          //         name = "puppy_";
          //         animController = puppyAnimatioController;
          //     }
          //     name += Random.Range(1, 3).ToString();
          // }

          //  GameObject model_2 = GameObject.Instantiate(CommonData.prefabs.menuLookup[name]);
            GameObject model_2 = GameObject.Instantiate(CommonData.prefabs.menuLookup[CommonData.currentUser.data.nameFriendPet]);
            model_2.transform.SetParent(Right.transform, false);
            model_2.transform.localPosition = new Vector3(0,0, 0);
            model_2.GetComponentInChildren<LabelController>().Init(Target, "Friend"); 
            model_2.GetComponent<Animator>().runtimeAnimatorController = animController;
            ListModels.Add(model_2);
        }
        
    }

    void DeleteModel()
    {
        foreach (GameObject model in ListModels)
        {
            Destroy(model);
        }
        ListModels.Clear();
    }
    public void InitHide()
    {
        mBuildingUDT = false;
        udtEventHandler.ShowQualityIndicator(false);
        CheckCollider.enabled = false;
        DeleteModel();
    }
    public void InitActive()
    {
        mBuildingUDT = false;
        udtEventHandler.ShowQualityIndicator(true);
        CheckCollider.enabled = true;
        msgTapTheCircle.SetActive(true);
        msgTryAgain.SetActive(false);
        msgGetCloser.SetActive(false);
        SpawnMyModel(CommonData.modeState);
        ShowModel(false);
        mTrackingMode = TrackingMode.CONSTRAINED_TO_CAMERA;
        
    }
    void LateUpdate()
    {

        switch (mTrackingMode)
        {
            case TrackingMode.IDLE:
            {
                InitHide();
            }
                break;

            case TrackingMode.CONSTRAINED_TO_CAMERA:
                {
                    // In this phase, the Pet is constrained to remain
                    // in the camera view, so it follows the user motion
                    Vector3 constrainedPos = cam.transform.position + cam.transform.forward * mInitialDistance;
                    this.transform.position = constrainedPos;

                    // Update object rotation so that it always look towards the camera
                    // and its "up vector" is always aligned with the gravity direction.
                    // NOTE: since we are using DeviceTracker, the World up vector is guaranteed 
                    // to be aligned (approximately) with the real world gravity direction 
                    RotateToLookAtCamera();

                    // Check if we were waiting for a UDT creation,
                    // and switch mode if UDT was created
                    if (mBuildingUDT && udtEventHandler && udtEventHandler.TargetCreated)
                    {

                        ImageTargetBehaviour trackedTarget = GetActiveTarget();

                        if (trackedTarget != null)
                        {
                            mBuildingUDT = false;

                            // Switch mode to UDT based tracking
                            mTrackingMode = TrackingMode.UDT_BASED;

                            // Update header text
                            DisplayMessage(msgGetCloser);


                            // Hide quality indicator
                            udtEventHandler.ShowQualityIndicator(false);

                            // Show the penguin
                            ShowModel(true);

                            // Wake up the proximity detector
                            if (proximityDetector)
                            {
                                proximityDetector.Sleep(false);
                            }

                            // Save a snapshot of the current position offset
                            // between the object and the target center
                            mPosOffsetAtTargetCreation = this.transform.position - trackedTarget.transform.position;
                        }
                    }
                }
                break;
            case TrackingMode.UDT_BASED:
                {
                    // Update the object world position according to the UDT target position
                    ImageTargetBehaviour trackedTarget = GetActiveTarget();
                    if (trackedTarget != null)
                    {
                        this.transform.position = trackedTarget.transform.position + mPosOffsetAtTargetCreation;
                    }

                    // Update object rotation so that it always look towards the camera
                    // and its "up vector" is always aligned with the gravity direction.
                    // NOTE: since we are using DeviceTracker, the World up vector is guaranteed 
                    // to be aligned (approximately) with the real world gravity direction 
                    RotateToLookAtCamera();
                }
                break;
        }
    }

    #endregion //MONOBEHAVIOUR_METHODS



    #region PUBLIC_METHODS
    public void ChangeAnim()
    {
       ListModels[0].GetComponent<Animator>().SetBool("Eat",true);

       

        StartCoroutine("DoSomething");

        Debug.Log("check");
    }
    IEnumerator DoSomething()
    {
        yield return new WaitForSeconds(0.1f);
        ListModels[0].GetComponent<Animator>().SetBool("Eat", false);
        yield return null;
    }
    public void ResetState()
    {
        mTrackingMode = TrackingMode.CONSTRAINED_TO_CAMERA;
        mBuildingUDT = false;

       

        // Update message and mode text
        DisplayMessage(msgTapTheCircle);


        // Hide the quality indicator
         udtEventHandler.ShowQualityIndicator(true);

        // Show the model
        ShowModel(false);

        // Make the proximity detector sleep
        if (proximityDetector)
        {
            proximityDetector.Sleep(true);
        }

    }

    #endregion //PUBLIC_METHODS


    #region PRIVATE_METHODS

    // Callback called when Vuforia has started
    private void OnVuforiaStarted()
    {
        cam = Vuforia.DigitalEyewearARController.Instance.PrimaryCamera ?? Camera.main;

        //StartCoroutine(ResetAfter(0.5f));
    }

    private IEnumerator ResetAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ResetState();

    }

    private void ChangeMode()
    {

        if (mTrackingMode == TrackingMode.CONSTRAINED_TO_CAMERA)
        {
            SwitchToUDTMode();
        }
    }



    private void SwitchToUDTMode()
    {
        if (mTrackingMode == TrackingMode.CONSTRAINED_TO_CAMERA)
        {
            // check if UDT frame quality is medium or high
            if (udtEventHandler.IsFrameQualityHigh() || udtEventHandler.IsFrameQualityMedium())
            {
                // Build a new UDT
                // Note that this may take more than one frame
                CreateUDT();

            }
            else
            {
                DisplayMessage(msgTryAgain);

            }
        }
    }

    private void CreateUDT()
    {
        float fovRad = cam.fieldOfView * Mathf.Deg2Rad;
        float halfSizeY = mInitialDistance * Mathf.Tan(0.5f * fovRad);
        float targetWidth = 2.0f * halfSizeY; // portrait
        if (Screen.width > Screen.height)
        { // landscape
            float screenAspect = Screen.width / (float)Screen.height;
            float halfSizeX = screenAspect * halfSizeY;
            targetWidth = 2.0f * halfSizeX;
        }

        mBuildingUDT = true;
        udtEventHandler.BuildNewTarget(targetWidth);
    }

    private void RotateToLookAtCamera()
    {
        Vector3 objPos = this.transform.position;
        Vector3 objGroundPos = new Vector3(objPos.x, 0, objPos.z); // y = 0
        Vector3 camGroundPos = new Vector3(cam.transform.position.x, 0, cam.transform.position.z);
        Vector3 objectToCam = camGroundPos - objGroundPos;
        objectToCam.Normalize();
        this.transform.rotation *= Quaternion.FromToRotation(this.transform.forward, objectToCam);
    }

    private void DisplayMessage(GameObject messageObj)
    {

        msgTapTheCircle.SetActive((msgTapTheCircle == messageObj));
        msgTryAgain.SetActive((msgTryAgain == messageObj));
        msgGetCloser.SetActive((msgGetCloser == messageObj));
    }



    private bool CheckTapOnObject()
    {
        if (CheckCollider == null)
            return false;

        // Test picking to check if user tapped on model
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                return (hit.collider == CheckCollider);
            }
        }

        return false;
    }

    private ImageTargetBehaviour GetActiveTarget()
    {
        StateManager stateManager = TrackerManager.Instance.GetStateManager();
        foreach (var tb in stateManager.GetActiveTrackableBehaviours())
        {
            if (tb is ImageTargetBehaviour)
            {
                // found target
                return (ImageTargetBehaviour)tb;
            }
        }
        return null;
    }

    private void ShowModel(bool isVisible)
    {
        foreach (GameObject model in ListModels)
        {
            model.SetActive(isVisible);
        }
    }
    

    
    #endregion //PRIVATE_METHODS

}

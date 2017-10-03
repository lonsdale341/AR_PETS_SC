/*===============================================================================
Copyright (c) 2015-2016 PTC Inc. All Rights Reserved.
 
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Vuforia;

public class UDTEventHandler : MonoBehaviour, IUserDefinedTargetEventHandler
{
    #region PUBLIC_MEMBERS

    /// <summary>
    /// Can be set in the Unity inspector to reference a ImageTargetBehaviour that is instanciated for augmentations of new user defined targets.
    /// </summary>
    public ImageTargetBehaviour ImageTargetTemplate;

    public int LastTargetIndex {
        get { return (mTargetCounter - 1) % MAX_TARGETS; }
    }

    public bool TargetCreated { get; set; }

    #endregion PUBLIC_MEMBERS



    #region PRIVATE_MEMBERS

    private const int MAX_TARGETS = 1;
    private UserDefinedTargetBuildingBehaviour mTargetBuildingBehaviour;
    private ObjectTracker mObjectTracker;
    
    // DataSet that newly defined targets are added to
    private DataSet mBuiltDataSet;
    
    // Currently observed frame quality
    private ImageTargetBuilder.FrameQuality mFrameQuality = ImageTargetBuilder.FrameQuality.FRAME_QUALITY_NONE;
    
    // Counter used to name newly created targets
    private int mTargetCounter;

    private GameObject udtFrameQualityIndicator;

    #endregion //PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS

    void Start()
    {
        mTargetBuildingBehaviour = GetComponent<UserDefinedTargetBuildingBehaviour>();
        if (mTargetBuildingBehaviour) {
            mTargetBuildingBehaviour.RegisterEventHandler(this);
            Debug.Log("Registering User Defined Target event handler.");
        }

        TargetCreated = false;

        if (!(udtFrameQualityIndicator = GameObject.Find("UDTFrameQualityIndicator"))) {
            Debug.Log("<color=yellow>ALERT: The UDT qualityIndicator is null and can't be displayed.</color>");
        }
    }

    void Update()
    {
        UpdateQualityIndicator();
    }

    #endregion //MONOBEHAVIOUR_METHODS


    #region IUserDefinedTargetEventHandler implementation

    /// <summary>
    /// Called when UserDefinedTargetBuildingBehaviour has been initialized successfully
    /// </summary>
    public void OnInitialized()
    {
        mObjectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        if (mObjectTracker != null) {
            // Create a new dataset
            mBuiltDataSet = mObjectTracker.CreateDataSet();
            mObjectTracker.ActivateDataSet(mBuiltDataSet);
        }
    }

    /// <summary>
    /// Updates the current frame quality
    /// </summary>
    public void OnFrameQualityChanged(ImageTargetBuilder.FrameQuality frameQuality)
    {
        Debug.Log("Frame quality changed: " + frameQuality.ToString());
        mFrameQuality = frameQuality;
        if (mFrameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_LOW) {
            Debug.Log("Low camera image quality");
        }
    }

    /// <summary>
    /// Takes a new trackable source and adds it to the dataset
    /// This gets called automatically as soon as you 'BuildNewTarget with UserDefinedTargetBuildingBehaviour
    /// </summary>
    public void OnNewTrackableSource(TrackableSource trackableSource)
    {
        mTargetCounter++;
        
        // Deactivates the dataset first
        mObjectTracker.DeactivateDataSet(mBuiltDataSet);

        // Destroy the oldest target if the dataset is full or the dataset 
        // already contains five user-defined targets.
        if (mBuiltDataSet.HasReachedTrackableLimit() || mBuiltDataSet.GetTrackables().Count() >= MAX_TARGETS) {
            IEnumerable<Trackable> trackables = mBuiltDataSet.GetTrackables();
            Trackable oldest = null;
            foreach (Trackable trackable in trackables) {
                if (oldest == null || trackable.ID < oldest.ID)
                    oldest = trackable;
            }
            
            if (oldest != null) {
                Debug.Log("Destroying oldest trackable in UDT dataset: " + oldest.Name);
                mBuiltDataSet.Destroy(oldest, true);
            }
        }
        
        // Get predefined trackable and instantiate it
        ImageTargetBehaviour imageTargetCopy = (ImageTargetBehaviour)Instantiate(ImageTargetTemplate);
        imageTargetCopy.gameObject.name = "UserDefinedTarget-" + mTargetCounter;
        
        // Add the duplicated trackable to the data set and activate it
        mBuiltDataSet.CreateTrackable(trackableSource, imageTargetCopy.gameObject);
        
        // Activate the dataset again
        mObjectTracker.ActivateDataSet(mBuiltDataSet);

        // Extended Tracking with user defined targets only works with the most recently defined target.
        // If tracking is enabled on previous target, it will not work on newly defined target.
        // Don't need to call this if you don't care about extended tracking.
        StopExtendedTracking();
        mObjectTracker.Stop();
        mObjectTracker.ResetExtendedTracking();
        mObjectTracker.Start();

        // Make sure Taregt build beehaviour keeps scanning...
        mTargetBuildingBehaviour.StartScanning();

        TargetCreated = true;
    }

    #endregion //IUserDefinedTargetEventHandler implementation


    #region PUBLIC_METHODS

    /// <summary>
    /// Instantiates a new user-defined target and is also responsible for dispatching callback to 
    /// IUserDefinedTargetEventHandler::OnNewTrackableSource
    /// </summary>
    public void BuildNewTarget(float targetWidth)
    {
        if (mFrameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_MEDIUM ||
            mFrameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_HIGH) {
            // create the name of the next target.
            // the TrackableName of the original, linked ImageTargetBehaviour is extended with a continuous number to ensure unique names
            string targetName = string.Format("{0}-{1}", ImageTargetTemplate.TrackableName, mTargetCounter);

            Debug.Log("Building target with size: " + targetWidth);

            TargetCreated = false;
            mTargetBuildingBehaviour.BuildNewTarget(targetName, targetWidth);
        } else {
            Debug.Log("Cannot build new target, due to poor camera image quality");
            TargetCreated = false;
        }
    }

    public bool IsFrameQualityHigh()
    {
        return (mFrameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_HIGH);
    }

    public bool IsFrameQualityMedium()
    {
        return (mFrameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_MEDIUM);
    }

    public void ShowQualityIndicator(bool isVisible)
    {
        if (udtFrameQualityIndicator != null) {
            Renderer renderer = udtFrameQualityIndicator.GetComponent<Renderer>();
            renderer.enabled = isVisible;
        }
    }

    #endregion //PUBLIC_METHODS


    #region PRIVATE_METHODS

    /// <summary>
    /// This method only demonstrates how to handle extended tracking feature when you have multiple targets in the scene
    /// So, this method could be removed otherwise
    /// </summary>
    private void StopExtendedTracking()
    {
        // If Extended Tracking is enabled, we first disable it for all the trackables
        // and then enable it only for the newly created target
        bool extTrackingEnabled = true;
        if (extTrackingEnabled) {
            StateManager stateManager = TrackerManager.Instance.GetStateManager();

            // 1. Stop extended tracking on all the trackables
            foreach (var tb in stateManager.GetTrackableBehaviours()) {
                var itb = tb as ImageTargetBehaviour;
                if (itb != null) {
                    itb.ImageTarget.StopExtendedTracking();
                }
            }
    
            // 2. Start Extended Tracking on the most recently added target
            List<TrackableBehaviour> trackableList = stateManager.GetTrackableBehaviours().ToList();
            ImageTargetBehaviour lastItb = trackableList[LastTargetIndex] as ImageTargetBehaviour;
            if (lastItb != null) {
                if (lastItb.ImageTarget.StartExtendedTracking())
                    Debug.Log("Extended Tracking successfully enabled for " + lastItb.name);
            }
        }
    }

    private void UpdateQualityIndicator()
    {
        if (udtFrameQualityIndicator != null) {

            bool isMediumOrHighQuality = (IsFrameQualityMedium() || IsFrameQualityHigh()) ? true : false;

            Renderer renderer = udtFrameQualityIndicator.GetComponent<Renderer>();

            if (isMediumOrHighQuality) {
                renderer.material.color = new Color(0.0f, 1.0f, 0.0f, 1.0f); // green
            } else {
                renderer.material.color = new Color(1.0f, 0.0f, 0.0f, 1.0f); // red
            }

            udtFrameQualityIndicator.transform.LookAt(Camera.main.transform);
        }
    }

    #endregion //PRIVATE_METHODS

}




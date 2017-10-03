
using UnityEngine;
using System.Collections;

public class ProximityDetector : MonoBehaviour
{
    #region PRIVATE_MEMBERS

    private Vector3 mLastObjectToCameraVec;
    private bool mSleeping = true;
    private bool mApproaching = false;
    private float mTargetYawAngle = 0;
    private float mCurrentYawAngle = 0;
    private float mTargetPitchAngle = 0;
    private float mCurrentPitchAngle = 0;

    #endregion //PRIVATE_MEMBERS



    #region MONOBEHAVIOUR_METHODS

    // Use this for initialization
    void Start()
    {
        mSleeping = true;
        mApproaching = false;
        mLastObjectToCameraVec = GetObjectToCameraVector();
        mCurrentYawAngle = 0;
        mTargetYawAngle = 0;
        mCurrentPitchAngle = 0;
        mTargetPitchAngle = 0;
    }
    
    // Update is called once per frame
    void Update()
    {
        CheckMotion();

        UpdateRotation();
    }

    #endregion //MONOBEHAVIOUR_METHODS



    #region PUBLIC_METHODS

    public void Sleep(bool sleep)
    {
        mSleeping = sleep;
    }

    #endregion //PUBLIC_METHODS



    #region PRIVATE_METHODS

    private void CheckMotion()
    {
        // Determine if user (camera) is moving toward the model or away from it
        Vector3 objToCam = GetObjectToCameraVector();
        if (objToCam.magnitude < mLastObjectToCameraVec.magnitude - 0.15f * objToCam.magnitude) {
            // User (camera) is getting close to the object
            mLastObjectToCameraVec = objToCam;
            mApproaching = true;
        } else if (objToCam.magnitude > mLastObjectToCameraVec.magnitude + 0.1f * objToCam.magnitude) {
            // User (camera) is getting far from object
            mLastObjectToCameraVec = objToCam;
            mApproaching = false;
        }
    }

    private void UpdateRotation()
    {
        if (mSleeping) {
            mTargetYawAngle = 60.0f;
            mTargetPitchAngle = -5.0f;
        } else { // not sleeping
            // React to motion
            if (mApproaching) {
                mTargetYawAngle = 5.0f;
                mTargetPitchAngle = 5.0f;
            } else {
                mTargetYawAngle = 60.0f;
                mTargetPitchAngle = -5.0f;
            }
        }

        // Update rotation smoothly
        mCurrentYawAngle = Mathf.LerpAngle(mCurrentYawAngle, mTargetYawAngle, 0.08f);
        mCurrentPitchAngle = Mathf.LerpAngle(mCurrentPitchAngle, mTargetPitchAngle, 0.08f);
        this.transform.localRotation =
            Quaternion.AngleAxis(mCurrentYawAngle, Vector3.up) *
        Quaternion.AngleAxis(-mCurrentPitchAngle, Vector3.right);
    }

    private Vector3 GetObjectToCameraVector()
    {
        Camera cam = Vuforia.DigitalEyewearARController.Instance.PrimaryCamera ?? Camera.main;
        return cam.transform.position - this.transform.position;
    }

    #endregion //PRIVATE_METHODS
}

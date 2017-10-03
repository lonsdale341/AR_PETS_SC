using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelController : MonoBehaviour
{
    public StopRotation stopRotate;
    public Text label;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init(GameObject target, string text)
    {
        stopRotate.target = target.transform;
        label.text = text;
    }
}

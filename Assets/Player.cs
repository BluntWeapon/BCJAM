using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private CharacterController charCon;
    private Vector3 moveVector;
    private Quaternion lookVector;
    private float rotX, rotY;

	// Use this for initialization
	void Start () {
		charCon = gameObject.GetComponent<CharacterController>();
        lookVector = Quaternion.identity;
	}
	
	// Update is called once per frame
	void Update () {
        moveVector = Vector3.zero;
	    if (Input.GetKey(KeyCode.W)) {
	        moveVector += Vector3.forward;
	    }

        charCon.SimpleMove(moveVector);

	    rotX += Input.GetAxis ( "Mouse X" );
	    rotY += Input.GetAxis("Mouse Y");

        Camera.main.transform.localEulerAngles = new Vector3 ( -rotY, rotX, 0 );

        //lookVector *= Quaternion.AngleAxis(Input.GetAxis("Mouse X"), transform.up) * Quaternion.AngleAxis( Input.GetAxis("Mouse Y"), transform.right);
	    //Camera.main.transform.rotation = lookVector;
	}
}

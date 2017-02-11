using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Game : MonoBehaviour {
	
	[SerializeField]
	private Transform player, camera;
	
	[SerializeField]
	private Transform CamPositionTop, CamPositionSide, CamPositionIso;

    private IEnumerator camMoveCoroutine = null;
    private CharacterController controller;
	

	private enum Mode{
		Top, Side, Iso
	}
	private Mode mode;

	// Use this for initialization
	void Start () {
		mode = Mode.Top;
	    controller = player.GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {

        //Camera Movement on Perspective Change
	    bool cameraExecute = false;
	    IEnumerator newCamCoroutine = null;
	    if (Input.GetKeyDown(KeyCode.D)) {
            newCamCoroutine = moveCamera(Mode.Side);
            mode = Mode.Side;
	        cameraExecute = true;
	    } else if( Input.GetKeyDown ( KeyCode.W ) ) {
            newCamCoroutine = moveCamera ( Mode.Top );
            mode = Mode.Top;
	        cameraExecute = true;
	    } else if (Input.GetKeyDown(KeyCode.A)) {
            newCamCoroutine = moveCamera ( Mode.Iso );
            mode = Mode.Iso;
	        cameraExecute = true;
	    }

	    if (cameraExecute && newCamCoroutine != null) {
            if( camMoveCoroutine != null )
                StopCoroutine(camMoveCoroutine);

	        StartCoroutine(newCamCoroutine);
	        camMoveCoroutine = newCamCoroutine;
	    }
		

        //Movement (And Rotation)
		Vector3 move = Vector3.zero;
		
		if( mode == Mode.Top ){
			
			if( Input.GetKey( KeyCode.RightArrow ) ){
		        move += transform.right * 0.1f;
		    }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                move += transform.right * -0.1f;
            }

        }

	    controller.Move(move);

	}

    IEnumerator moveCamera(Mode targetMode) {

        //Indicator for whether we're done
        bool finished = false;

        Transform target;
        switch (targetMode) {
            case Mode.Top:
                target = CamPositionTop;
                break;
            case Mode.Side:
                target = CamPositionSide;
                break;
            case Mode.Iso:
                target = CamPositionIso;
                break;
            default:
                yield break;
        }

        do {
            camera.position = Vector3.Lerp ( camera.position, target.position, 0.15f );
            camera.rotation = Quaternion.Slerp(camera.rotation, target.rotation, 0.2f);
            if (Vector3.Distance(target.position, camera.position) < 0.001f) {
                finished = true;
            }

            yield return 1; // Break for a frame

        } while (!finished);

    }
}

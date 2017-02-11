using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
	
	[SerializeField]
	private Transform player, camera;
	
	[SerializeField]
	private Transform CamPositionTop, CamPositionSide, CamPositionIso;

    private IEnumerator camMoveCoroutine = null;
	
	private enum Mode{
		Top, Side, Iso
	}
	private Mode mode;

	// Use this for initialization
	void Start () {
		mode = Mode.Top;
	}
	
	// Update is called once per frame
	void Update () {

	    if (Input.GetKeyDown(KeyCode.D)) {
            camMoveCoroutine = moveCamera(Mode.Side);
        } else if( Input.GetKeyDown ( KeyCode.W ) ) {
            camMoveCoroutine = moveCamera ( Mode.Top );
        } else if (Input.GetKeyDown(KeyCode.A)) {
            camMoveCoroutine = moveCamera ( Mode.Iso );
        }
		
		Vector3 move = Vector3.zero;
		
		if( mode == Mode.Top ){
			
			if( Input.GetKey( KeyCode.RightArrow ) ){
		        move += transform.right * 0.1f;
		    }
			
		}

	    player.transform.position += move;

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
            camera.position = Vector3.Lerp ( camera.position, target.position, 0.2f );
            camera.rotation = Quaternion.Slerp(camera.rotation, target.rotation, 0.2f);
            if (Vector3.Distance(target.position, camera.position) < 0.001f) {
                finished = true;
            }

            yield return 1; // Break for a frame

        } while (!finished);

        print("Ho!");

    }
}

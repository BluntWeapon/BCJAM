using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

public class Game : MonoBehaviour {
	
	[SerializeField]
	private Transform player, camera;
	
	[SerializeField]
	private Transform CamPositionTop, CamPositionSide, CamPositionIso;

    [SerializeField] private float SlerpTime = 0.05f;

    private IEnumerator camMoveCoroutine = null;
    private CharacterController controller;

    [SerializeField] private Material playerBlue, playerRed;
    private enum PColor {
        Red, Blue
    }
    private PColor pColor = PColor.Blue;
    private Renderer playerRenderer;
	

	private enum Mode{
		Top, Side, Iso
	}
	private Mode mode;

    private bool isSwappingPerspective = false;

	// Use this for initialization
	void Start () {
		mode = Mode.Top;
	    controller = player.GetComponent<CharacterController>();
	    playerRenderer = player.gameObject.GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {

        //Camera Movement on Perspective Change
	    IEnumerator newCamCoroutine = null;
	    if (Input.GetKeyDown(KeyCode.D)) {
            newCamCoroutine = moveCamera(Mode.Side);
	    } else if( Input.GetKeyDown ( KeyCode.W ) ) {
            newCamCoroutine = moveCamera ( Mode.Top );
	    } else if (Input.GetKeyDown(KeyCode.A)) {
            newCamCoroutine = moveCamera ( Mode.Iso );
	    }

        if ( newCamCoroutine != null ) {
            if( camMoveCoroutine != null )
                StopCoroutine(camMoveCoroutine);

	        StartCoroutine(newCamCoroutine);
	        camMoveCoroutine = newCamCoroutine;
	    }
        if( isSwappingPerspective ) return;




        //Color Swap
        if (Input.GetMouseButtonDown(0)) {
	        pColor = PColor.Blue;
	        playerRenderer.material = playerBlue;
	    } else if (Input.GetMouseButtonDown(1)) {
	        pColor = PColor.Red;
	        playerRenderer.material = playerRed;
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

        if( mode == Mode.Top ) {//TODO This check should be done somewhere else
            //TODO Drop player
            Debug.Log ( "Ho!" );

            RaycastHit hit;
            Physics.Raycast ( new Ray ( player.position, Vector3.down ), out hit, 60f );
            player.position = new Vector3 ( player.position.x, hit.point.y + 1.001f, player.position.z );
        }

        //Indicator for whether we're done
        isSwappingPerspective = true;

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
            camera.position = Vector3.Slerp( camera.position, target.position, SlerpTime );
            camera.rotation = Quaternion.Slerp(camera.rotation, target.rotation, SlerpTime );
            if (Vector3.Distance(target.position, camera.position) < 0.01f) {
                camera.position = target.position;

                if (targetMode == Mode.Top) {
                    player.position = new Vector3( player.position.x, 40f, player.position.z );
                }

                mode = targetMode;
                isSwappingPerspective = false;
                break;
            }

            yield return 1; // Break for a frame

        } while (isSwappingPerspective);

    }

    private void SetPlayerHigh() {
        
    }

}

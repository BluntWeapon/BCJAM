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

    [SerializeField] private List<Collider> blueColliders, redColliders;
	

	private enum Mode{
		Top, Side, Iso
	}
	private Mode mode;

    private bool isSwappingPerspective = false;

    [SerializeField] private float jumpVelocity = 4f, gravity = 3f;
    private float jumping = 0f;

    [SerializeField] private float moveSpeed = 4f;

	// Use this for initialization
	void Start () {
		mode = Mode.Top;
	    controller = player.GetComponent<CharacterController>();
	    playerRenderer = player.gameObject.GetComponent<Renderer>();

        //Disable all blue colliders
        foreach( Collider collider in blueColliders ) {
            collider.enabled = false;
        }
        //Enable all red colliders
        foreach( Collider collider in redColliders ) {
            collider.enabled = true;
        }
    }
	
	// Update is called once per frame
	void Update () {

        //TODO This doesn't check if dimensions can be changed
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


        
        //TODO This doesn't check if colour can be swapped first
        //Color Swap
        if (Input.GetMouseButtonDown(0)) {
	        pColor = PColor.Blue;
	        playerRenderer.material = playerBlue;

            //Disable all blue colliders
            foreach (Collider collider in blueColliders) {
                collider.enabled = false;
            }
            //Enable all red colliders
            foreach( Collider collider in redColliders ) {
                collider.enabled = true;
            }

        } else if (Input.GetMouseButtonDown(1)) {
	        pColor = PColor.Red;
	        playerRenderer.material = playerRed;

            //Disable all red colliders
            foreach( Collider collider in redColliders ) {
                collider.enabled = false;
            }
            //Enable all blue colliders
            foreach( Collider collider in blueColliders ) {
                collider.enabled = enabled;
            }
        }
		

        //Movement (And Rotation)
		Vector3 move = Vector3.zero;
		
		if( mode == Mode.Top ){
			
			if( Input.GetKey( KeyCode.RightArrow ) ){
		        move += transform.right * moveSpeed * Time.deltaTime;
		    }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                move += -transform.right * moveSpeed * Time.deltaTime;
            }

        }

        if (mode == Mode.Side)
        {

            if (Input.GetKey(KeyCode.RightArrow))
            {
                move += transform.right * moveSpeed  * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                move += -transform.right * moveSpeed * Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                jumping = jumpVelocity;
            }
            else if ( !controller.isGrounded ) {
                jumping -= gravity * Time.deltaTime;
            } else if (controller.isGrounded) {
                jumping = 0f;
            }

            move += transform.up * jumping * Time.deltaTime;

        }

        controller.Move(move);

    }

    IEnumerator moveCamera(Mode targetMode) {

        if( mode == Mode.Top && targetMode != Mode.Top ) {//TODO This check should be done somewhere else
            RaycastHit hit;
            Physics.SphereCast( new Ray ( player.position, Vector3.down ), 0.5f , out hit, 60f );
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
                    player.position = new Vector3( player.position.x, 25f, player.position.z );
                }

                SwitchDimension( targetMode );

                mode = targetMode;
                isSwappingPerspective = false;
                break;
            }

            yield return 1; // Break for a frame

        } while (isSwappingPerspective);

    }

    private void SwitchDimension(Mode targetDimension) {

        switch (targetDimension) {
            case Mode.Top:
                player.gameObject.layer = 8;
                break;
            case Mode.Side:
                player.gameObject.layer = 9;
                break;
            case Mode.Iso:
                player.gameObject.layer = 10;
                break;
        }

    }

}

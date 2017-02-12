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
		        move += transform.right * 0.1f;
		    }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                move += transform.right * -0.1f;
            }

        }

        if (mode == Mode.Side)
        {

            if (Input.GetKey(KeyCode.RightArrow))
            {
                move += transform.right * 0.1f;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                move += transform.right * -0.1f;
            }

        }

        controller.Move(move);

        //Raycast downwards from edge. Detects green, red and blue box collisions and passes.
        raycastDetection();

    }

    //Indicator for whether we're done
    void raycastDetection() {
        // -A explanation-
        // This creates a copy of the Vector3 instead of referencing it from GameObject.Find actual Object. V3's are struct. Structs are on the stack instead of the heap.
        // This is different from how C++ deals with Structs, because this is C#. Structs vs Object. C# has this integrated into because it has C++ blood.
        float playerY = 0; // If statements won't initialize variables. This needs to be here to initialize.
        Vector3 playerPosition = GameObject.Find("Player").transform.position;  
        Vector3 hoverDistance = new Vector3 (0, 10.0f, 0);
        Vector3 rayOrigin = new Vector3(playerPosition.x, playerPosition.y - (playerSize.y / 2), playerPosition.z);
        Vector3 btm = GameObject.Find("Player").transform.TransformDirection(-Vector3.up);
        // Vector3 fwd = GameObject.Find("Player").transform.TransformDirection(Vector3.right);

        Ray ray = new Ray (rayOrigin, btm);
        RaycastHit hit;

        if (mode == Mode.Top)
        {
            GameObject.Find("Player").transform.position = new Vector3(playerPosition.x, 5, playerPosition.z); // Add fixed distance from topmost surface.
            // playerPosition = new Vector3 (playerPosition.x, 5, playerPosition.z); // This is incorrectly. // Refer to -A explanation- 
        }

        if (Physics.Raycast(ray, out hit)) {
            Debug.DrawRay( ray.origin, hit.point - ray.origin, Color.red);
            // Debug.Log(hit.point.y);
            playerY = hit.point.y;
        }

        if (mode == Mode.Side) {
            GameObject.Find("Player").transform.position = new Vector3(playerPosition.x, playerY + (playerSize.y / 2), playerPosition.z); // Add fixed distance from topmost surface.
            // playerPosition = new Vector3 (playerPosition.x, 5, playerPosition.z); // This is incorrectly. // Refer to -A explanation- 
        }


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

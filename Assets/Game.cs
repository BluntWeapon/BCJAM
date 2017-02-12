using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Game : MonoBehaviour {
	
	[SerializeField]
	private Transform player, camera;
    private Vector3 playerSize;

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
        playerSize = GameObject.Find("Player").GetComponent<Collider>().bounds.size;
        Debug.Log(playerSize);
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

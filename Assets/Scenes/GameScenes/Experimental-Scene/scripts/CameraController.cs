using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour {
	
	public bool multiplayer;
	
	//variables not visible in the inspector
	public static float movespeed;
	public static float zoomSpeed;
	public static float mouseSensitivity;
    public static float clampAngle;
	
    float rotationY = 0;
    float rotationX = 0;
	
	bool canRotate;
 
    void Start(){
	//get start rotation
        Vector3 rot = transform.localRotation.eulerAngles;
        rotationY = rot.y;
        rotationX = rot.x;
	
		if(transform.position.x < 0 && multiplayer){
			rotationY += 180;
		}
    }
	
	void Update(){
	//if the mobile prefab is added to the scene
        if((GameObject.Find("MobileController") && MobileController.camEnabled) || (GameObject.Find("Mobile multiplayer") && MobileMultiplayer.camEnabled)){
            MobileCamera();
        }
	}
	
	void MobileCamera(){
		//check if exactly one finger is touching the screen
		if(Input.touchCount == 1){
			//rotate camera based on the touch position
			Touch touch = Input.GetTouch(0);
			
			if(touch.phase == TouchPhase.Began){
				if(EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)){
					canRotate = false;
				}
				else{
					canRotate = true;
				}
			}
			
			if(!canRotate)
				return;
				
			float mouseX = touch.deltaPosition.x;
			float mouseY = -touch.deltaPosition.y;
			rotateCamera(mouseX, mouseY);
		}
		//check for two touches
		else if(Input.touchCount == 2){
            //store two touches
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            //find the position in the previous frame of each touch
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            //find the magnitude of the vector (the distance) between the touches in each frame
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            //find the difference in the distances between each frame
            float z = (prevTouchDeltaMag - touchDeltaMag) * 0.001f * zoomSpeed;
			
			//zoom camera by moving it forward/backward
			transform.Translate(new Vector3(0, 0, -z));
		}
	}
	
	
	void rotateCamera(float mouseX, float mouseY){
		//check if mobile controls are enabled to adjust sensitivity
		if(GameObject.Find("MobileController") == null && GameObject.Find("Mobile multiplayer") == null){
            rotationY += mouseX * mouseSensitivity * Time.deltaTime;
            rotationX += mouseY * mouseSensitivity * Time.deltaTime;
		}
		else{
            rotationY += mouseX * mouseSensitivity * Time.deltaTime * 0.05f; 
            rotationX += mouseY * mouseSensitivity * Time.deltaTime * 0.05f;
		}
	
		//clamp x rotation to limit it
		rotationX = Mathf.Clamp(rotationX, -clampAngle, clampAngle);
	
		//apply rotation
		transform.rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
	}
}

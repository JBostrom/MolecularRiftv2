using UnityEngine;
using System.Collections;
using Leap;

public class PlayerController : MonoBehaviour {

    //Molecule Control
    public float moveSensitivity = 40f;
    public float moveSpeed = 0.05f;
    public float rotationSensitivity = 0.5f;
    public float rotationSpeed = 1f;
    public float transformSensitivity = 0.3f;
    public float transformSpeed = 0.05f;
    public float rotationSpeedMultiplierByDistance = 0.0f;

    //Menu
    public Camera menuEnablingCamera;
    private Vector3 menuInEnablingCameraView;
    public float menuDistanceFromHand = 0.13f;
    public int menuShowing = 0;

    private GameObject[] menus;

    //Scripts
    private AddAtoms addAtoms;
    private Controller leapController;

    //Gameobjects & Transforms
    private GameObject molRoot;
    private Vector3 molRootOffset;
    private Transform mainCamera;
    private Transform LeapHandControllerTransform;
    private Transform LeftHandObjectTransform;
    private Transform menuTransform;

    //Leap Camera
    private Hand leftHand;
    private Hand rightHand;
    private Frame currentFrame;
    private Vector3 lastFramPosRH;
    private Vector3 movementStart;
    
	void Start ()
	{
		leapController = new Controller ();
        addAtoms = gameObject.GetComponent<AddAtoms>();
        molRoot = GameObject.Find ("molRoot");
        molRootOffset = Vector3.zero;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        menuTransform = transform.FindChild("Menu");
        LeapHandControllerTransform = transform.FindChild("LMHeadMountedRig/TrackingSpace/CenterEyeAnchor/HeadMount/LeapHandController");
        LeftHandObjectTransform = LeapHandControllerTransform.FindChild("ImageFullLeftHand(Clone)/HandContainer");

        //Hide menu toggle if only 1 ligand is present

        if (addAtoms.CollectionOfLigands == null)
        {
            menuTransform.FindChild("ToggleMenuButtons").gameObject.SetActive(false);
        }

        menus = new GameObject[menuTransform.childCount - 1];

        for (int i = 0; i < menus.Length; i++)
        {
            menus[i] = menuTransform.GetChild(i).gameObject;
            menus[i].SetActive(false);
        }

        menus[menuShowing].SetActive(true);

        // Move children of molRoot to position molRoot origin in middle of ligand
        
        GameObject[] hetatms = GameObject.FindGameObjectsWithTag("hetatmbs");

        Vector3 sumOfPos = Vector3.zero;

        for (int i = 0; i < hetatms.Length; i++)
        {
            sumOfPos += hetatms[i].transform.localPosition;
        }

        Vector3 offset = sumOfPos / hetatms.Length;

        foreach (Transform child in molRoot.transform)
        {
            child.localPosition -= offset;
        } 
        
    }
	
	void Update () 
	{
        if (LeftHandObjectTransform == null)
        {
            LeftHandObjectTransform = LeapHandControllerTransform.FindChild("ImageFullLeftHand(Clone)/HandContainer");
        }
        
        currentFrame = leapController.Frame();

        //Identify Hands

        if (molRoot != null)
        {
            if (!currentFrame.Hands.IsEmpty)
            {
                if (currentFrame.Hands.Rightmost.IsRight)
                {
                    rightHand = currentFrame.Hands.Rightmost;
                }
                else if (currentFrame.Hands.Leftmost.IsRight)
                {
                    rightHand = currentFrame.Hands.Leftmost;
                }
                else
                {
                    rightHand = null;
                }
                if (currentFrame.Hands.Leftmost.IsLeft)
                {
                    leftHand = currentFrame.Hands.Leftmost;
                }
                else if (currentFrame.Hands.Rightmost.IsLeft)
                {
                    leftHand = currentFrame.Hands.Rightmost;
                }
                else
                {
                    leftHand = null;
                }
            }
            else
            {
                rightHand = null;
                leftHand = null;
            }

            // Input

            if (rightHand != null)
            {
                if (lastFramPosRH == null)
                {
                    lastFramPosRH = ToUnityAndVRAlign(rightHand.PalmPosition);
                }

                if (Input.GetKey("1") || Input.GetButton("Fire3") || Input.GetButton("Button.DpadUp") || Input.GetButton("RawButton.DpadUp")) //Rotate
                {
                    float xChange = Mathf.Abs(ToUnityAndVRAlign(rightHand.PalmPosition).x - lastFramPosRH.x);
                    float yChange = Mathf.Abs(ToUnityAndVRAlign(rightHand.PalmPosition).y - lastFramPosRH.y);
                    float zChange = Mathf.Abs(ToUnityAndVRAlign(rightHand.PalmPosition).z - lastFramPosRH.z);

                    //Possible way of chanigng rotation speed based on distance from molRoot

                    float changedRotationSpeed = rotationSpeed;

                    if (rotationSpeedMultiplierByDistance > 0)
                    {
                        float distance = Vector3.Distance(transform.position, molRoot.transform.position);
                        changedRotationSpeed = changedRotationSpeed = rotationSpeed * distance * 0.1f * rotationSpeedMultiplierByDistance;

                    }
                    
                    {
                        if (xChange > rotationSensitivity)
                        {
                            if (ToUnityAndVRAlign(rightHand.PalmPosition).x - lastFramPosRH.x < 0)
                            {
                                molRoot.transform.Rotate(Vector3.Scale(Vector3.forward, new Vector3(changedRotationSpeed, changedRotationSpeed, changedRotationSpeed)), Space.World);
                            }
                            else
                            {
                                molRoot.transform.Rotate(Vector3.Scale(Vector3.back, new Vector3(changedRotationSpeed, changedRotationSpeed, changedRotationSpeed)), Space.World);
                            }
                        }
                        if (yChange > rotationSensitivity)
                        {
                            if (ToUnityAndVRAlign(rightHand.PalmPosition).y - lastFramPosRH.y < 0)
                            {
                                molRoot.transform.Rotate(Vector3.Scale(Vector3.left, new Vector3(changedRotationSpeed, changedRotationSpeed, changedRotationSpeed)), Space.World);
                            }
                            else
                            {
                                molRoot.transform.Rotate(Vector3.Scale(Vector3.right, new Vector3(changedRotationSpeed, changedRotationSpeed, changedRotationSpeed)), Space.World);
                            }
                        }
                        if (zChange > rotationSensitivity)
                        {
                            if (ToUnityAndVRAlign(rightHand.PalmPosition).z - lastFramPosRH.z < 0)
                            {
                                molRoot.transform.Rotate(Vector3.Scale(Vector3.up, new Vector3(changedRotationSpeed, changedRotationSpeed, changedRotationSpeed)), Space.World);
                            }
                            else
                            {
                                molRoot.transform.Rotate(Vector3.Scale(Vector3.down, new Vector3(changedRotationSpeed, changedRotationSpeed, changedRotationSpeed)), Space.World);
                            }
                        }
                    }
                }

                if (Input.GetKey("2") || Input.GetButton("Fire1")) // Transform
                {
                    if (Mathf.Abs(ToUnityAndVRAlign(rightHand.PalmPosition).x - lastFramPosRH.x) > transformSensitivity)
                    {
                        molRoot.transform.Translate((Vector3.Scale(ToUnityAndVRAlign(rightHand.PalmPosition) - lastFramPosRH, new Vector3(transformSpeed, transformSpeed, transformSpeed))), mainCamera);
                    }
                }

                if (Input.GetKey("3") || Input.GetButton("Jump")) // User Move
                {
                    if (Input.GetKeyDown("3") || Input.GetButtonDown("Jump"))
                    {
                        movementStart = ToUnityAndVRAlign(rightHand.PalmPosition);
                    }

                    if (ToUnityAndVRAlign(rightHand.PalmPosition).x - movementStart.x > moveSensitivity)
                    {
                        transform.Translate(Vector3.Scale(Vector3.right, new Vector3(moveSpeed, moveSpeed, moveSpeed)), mainCamera);
                    }
                    else if (ToUnityAndVRAlign(rightHand.PalmPosition).x - movementStart.x < -moveSensitivity)
                    {
                        transform.Translate(Vector3.Scale(Vector3.left, new Vector3(moveSpeed, moveSpeed, moveSpeed)), mainCamera);
                    }

                    if (ToUnityAndVRAlign(rightHand.PalmPosition).y - movementStart.y > moveSensitivity)
                    {
                        transform.Translate(Vector3.Scale(Vector3.up, new Vector3(moveSpeed, moveSpeed, moveSpeed)), mainCamera);
                    }
                    else if (ToUnityAndVRAlign(rightHand.PalmPosition).y - movementStart.y < -moveSensitivity)
                    {
                        transform.Translate(Vector3.Scale(Vector3.down, new Vector3(moveSpeed, moveSpeed, moveSpeed)), mainCamera);
                    }

                    if (ToUnityAndVRAlign(rightHand.PalmPosition).z - movementStart.z > moveSensitivity)
                    {
                        transform.Translate(Vector3.Scale(Vector3.forward, new Vector3(moveSpeed, moveSpeed, moveSpeed)), mainCamera);
                    }
                    else if (ToUnityAndVRAlign(rightHand.PalmPosition).z - movementStart.z < -moveSensitivity)
                    {
                        transform.Translate(Vector3.Scale(Vector3.back, new Vector3(moveSpeed, moveSpeed, moveSpeed)), mainCamera);
                    }
                }

                
                lastFramPosRH = ToUnityAndVRAlign(rightHand.PalmPosition);
            }

            if (Input.GetKey("4") || Input.GetButton("Fire2")) // Reset molroot
            {
                molRoot.transform.position = (mainCamera.position + mainCamera.forward * 50) - molRootOffset;
            }

            if (Input.GetAxis("Vertical") > 0)
            {
                transform.Translate(Vector3.Scale(Vector3.forward, new Vector3(moveSpeed, moveSpeed, moveSpeed)), mainCamera);
            }

            if (Input.GetAxis("Vertical") < 0)
            {
                transform.Translate(Vector3.Scale(Vector3.back, new Vector3(moveSpeed, moveSpeed, moveSpeed)), mainCamera);
            }

            if (Input.GetAxis("Horizontal") > 0)
            {
                transform.Translate(Vector3.Scale(Vector3.right, new Vector3(moveSpeed, moveSpeed, moveSpeed)), mainCamera);
            }

            if (Input.GetAxis("Horizontal") < 0)
            {
                transform.Translate(Vector3.Scale(Vector3.left, new Vector3(moveSpeed, moveSpeed, moveSpeed)), mainCamera);
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                molRootOffset = transform.position - molRoot.transform.position;
                
                foreach (Transform child in molRoot.transform)
                {
                    child.position -= molRootOffset;
                }
                
                molRoot.transform.position += molRootOffset;
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                Application.LoadLevel(0);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            //Menu
            
            if (LeftHandObjectTransform != null)
            {
                menuTransform.position = (LeftHandObjectTransform.position);
                menuTransform.Translate(Vector3.right * menuDistanceFromHand);
                menuTransform.LookAt(2 * menuTransform.position - LeapHandControllerTransform.position);

                menuInEnablingCameraView = menuEnablingCamera.WorldToViewportPoint(menuTransform.position);

                if (menuInEnablingCameraView.x > 0 &&
                    menuInEnablingCameraView.x < 1 &&
                    menuInEnablingCameraView.y > 0 &&
                    menuInEnablingCameraView.y < 1 &&
                    menuInEnablingCameraView.z > 0.15)
                {
                    menuTransform.gameObject.SetActive(true);
                }
                else
                {
                    menuTransform.gameObject.SetActive(false);
                }
            }
            else
            {
                menuTransform.gameObject.SetActive(false);
            }
        }
    }

    public void SwitchToMenu(int menuNumber)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (i == menuNumber)
            {
                menus[i].SetActive(true);
            }
            else
            {
                menus[i].SetActive(false);
            }
        }
        
        menuShowing = menuNumber;
    }

    public Vector3 ToUnityAndVRAlign(Vector leapVector)
    {
        Vector3 unityVector = leapVector.ToUnity();
        return new Vector3(-unityVector.x, unityVector.z, unityVector.y);
    }
}

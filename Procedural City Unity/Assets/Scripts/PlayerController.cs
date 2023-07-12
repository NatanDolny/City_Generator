using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    PlayerHandler playerMovement;
    public float mouseXSens = 30;
    public float mouseYSens = 30;
    public Vector2 mouseRotation;
    Rigidbody rb;
    GameObject cameraObject;

    ProceduralScript proceduralScript;
    PauseMenu pauseMenu;

    public bool active = true;
    public bool paused = false;
    public bool canMove = true;
    public bool canRotate = true;

    public float mouseSensitivity = 1.8f;
    public float slowSpeed = 20f;
    public float moveSpeed = 35f;
    public float boostSpeed = 75f;
    public float speedAcceleration = 1.5f;

    public CursorLockMode wantedLM;
    public float currentIncrease = 1;
    public float currentIncreaseMem = 0;

    public Vector3 initPosition;
    public Vector3 initRotation;

    private void Awake()
    {
        playerMovement = new PlayerHandler();
        proceduralScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<ProceduralScript>();
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<PauseMenu>();
    }

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.Initialiser(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(!paused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseMenu.Initialiser(true);
            }
            if (canMove)
            {
                Vector3 deltaPosition = Vector3.zero;
                float currentSpeed = moveSpeed;

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    currentSpeed = boostSpeed;
                }
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    currentSpeed = slowSpeed;
                }

                if (Input.GetKey(KeyCode.W))
                {
                    deltaPosition += transform.forward;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    deltaPosition -= transform.right;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    deltaPosition -= transform.forward;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    deltaPosition += transform.right;
                }
                if (Input.GetKey(KeyCode.Q))
                {
                    deltaPosition -= transform.up;
                }
                if (Input.GetKey(KeyCode.E))
                {
                    deltaPosition += transform.up;
                }

                if (deltaPosition != Vector3.zero)
                {
                    transform.position += deltaPosition * Time.deltaTime * currentSpeed;
                }
            }

            if (canRotate)
            {
                transform.rotation *= Quaternion.AngleAxis(
                    -Input.GetAxis("Mouse Y") * mouseSensitivity,
                    Vector3.right);

                transform.rotation = Quaternion.Euler(
                    transform.eulerAngles.x,
                    transform.eulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity,
                    transform.eulerAngles.z);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseMenu.Initialiser(false);
            }
        }
    }
}

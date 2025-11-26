using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]

public class SC_FPSController : MonoBehaviour
{
    private float walkingSpeed = 7.5f;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float lookSpeed;
    [SerializeField] private float lookXLimit;

    [SerializeField] private Camera newCamera;
    [SerializeField] private Camera oldCamera;
    private Camera playerCamera;
    private Vector3 initpos;

    [SerializeField] private Image flashImage;
    [SerializeField] private float flashDuration = 0.5f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [Header("ライト関連")]
    [SerializeField]private Light _light;
    [SerializeField]private float _damage;
    [SerializeField]private float _defaultIntensity;
    [SerializeField]private float _highIntensity;

    [HideInInspector]
    public bool canMove = true;

    private RaycastHit _hit;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        initpos = transform.position;
        ResetPos();
    }

    private void ResetPos()
    {
        transform.position = initpos;
        playerCamera = newCamera;
        oldCamera.gameObject.SetActive(false);
        newCamera.gameObject.SetActive(true);
    }

    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchCamera();
        }

        //Reset Position when R key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPos();
        }


        if(Input.GetMouseButtonUp(0))
        {
            _light.intensity=_defaultIntensity;
        }
        //ライト処理
        if(Input.GetMouseButton(0))
        {
            if(playerCamera==oldCamera)return;
            _light.intensity=_highIntensity;

            if(Physics.Raycast(playerCamera.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0)),out _hit,1000))
            {
                if(!_hit.collider.CompareTag("Enemy"))return;
                var targetEnemy=_hit.collider.GetComponent<MovingEnemy>();
                targetEnemy.SendMessage("Attacked",_damage*Time.deltaTime);
            }
        }

    }

    private void Move()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    public void SwitchCamera()
    {
        if (playerCamera == oldCamera)
        {
            playerCamera = newCamera;
            oldCamera.gameObject.SetActive(false);
            newCamera.gameObject.SetActive(true);

        }
        else
        {
            playerCamera = oldCamera;
            newCamera.gameObject.SetActive(false);
            oldCamera.gameObject.SetActive(true);
        }
    }

    public void TakesPic()
    {
        StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        float halfDuration = flashDuration / 2;
        Color flashColor = flashImage.color;
        
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, 1, t / halfDuration);
            flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            yield return null;
        }

        flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 1);

        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1, 0, t / halfDuration);
            flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            yield return null;
        }

        flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0);
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    float xRotation = 0f;
    [HideInInspector]
    public float mouseX;
    [HideInInspector]
    public float mouseY;
    public GameObject Weapon;
    private GunBehaviour gb;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        gb = Weapon.GetComponent<GunBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        //mouseX = gb.Current_upRecoil + Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        //mouseY = gb.Current_sideRecoil + Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        if (gb.disparar)
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime + gb.current_uprecoil;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //transform.Rotate(new Vector3(xRotation, 0f, 0f),);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * (mouseX + gb.current_rightrecoil));
        }
        else
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //transform.Rotate(new Vector3(xRotation, 0f, 0f),);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
        


    }
    
}


using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float sprintSpeed = 5f; 
    [SerializeField] private float rotateSpeed = 200F;
    
    float yaw;
    float pitch;


    float currentSpeed;
    void Start()
    {
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        /*if(Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        }
          if(Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);
        }
          if(Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);
        }
          if(Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * currentSpeed * Time.deltaTime);
        }
        */

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput,0,verticalInput) * currentSpeed * Time.deltaTime;
        transform.Translate(movement);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = normalSpeed;
        }

        //Rotacion teclado

        if (Input.GetKey(KeyCode.Q))
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0,-1,0));
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0,1,0));
        }


        //Rotacion Mouse
        
        float mousexInput = Input.GetAxis("Mouse X");
        float mouseyInput = Input.GetAxis("Mouse Y");
        
        /*Vector3 rotation = new Vector3(-mouseyInput, mousexInput, 0) * rotateSpeed * Time.deltaTime;
        transform.Rotate(rotation);*/

    yaw+= mousexInput * rotateSpeed * Time.deltaTime;
    pitch-= mouseyInput * rotateSpeed * Time.deltaTime;

    pitch = Mathf.Clamp(pitch,-90f,90f);
    transform.rotation = Quaternion.Euler(pitch, yaw,0);

    }
}

                
            
            
            
          
        

        

    

     


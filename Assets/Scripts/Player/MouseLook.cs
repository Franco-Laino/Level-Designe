using Unity.Mathematics;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    [SerializeField] private float sensitivity = 100f;  // Sensibilidad del mouse.
    private Transform player;  // Transform del prota.

    private float axisMouseX;
    private float axisMouseY;


	private float axisXRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Movimiento del mouse
        axisMouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        axisMouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // Rotacion vertical limitada
        axisXRotation -= axisMouseY;
        axisXRotation = Mathf.Clamp(axisXRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(axisXRotation, 0f, 0f);
        player.Rotate(Vector3.up * axisMouseX);
	}
}

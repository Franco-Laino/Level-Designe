using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float speed = 6f;
    private CharacterController controller;

    [SerializeField] private bool canMove = true;  // Detiene todo el escenario del juego para que el prota pueda hacer los puzzles.

    // Inputs
    private float axisX = Input.GetAxis("Horizontal");
    private float axisY = Input.GetAxis("Vertical");




	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return; // Cuando el prota esté en un minijuego, no se mueve.
        
        Vector3 move = transform.right * axisX + transform.forward * axisY;  // Movimiento relativo al prota.
        controller.Move(move * speed * Time.deltaTime);  // Mover al prota.

    }
}

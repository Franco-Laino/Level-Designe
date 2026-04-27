using UnityEngine;

public class Flashlight : MonoBehaviour
{

    private Light lightSource;  // La fuente de luz.

	[SerializeField] private float maxBattery = 100f; // Maximo de la bateria.
	[SerializeField] private float battery;  // Nivel actual de la bateria.

    [SerializeField] private float drainRate = 10f;  // Drena la bateria de la linterna.
    [SerializeField] private float range = 10f;  // Rango de alumbrado de la linterna.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        battery = maxBattery;  // Inicializa la bateria.

        lightSource.enabled = false;  // Apaga la luz al inicio.
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F) && battery > 0)  // Si mantiene F y tiene bateria.
		{
			lightSource.enabled = true;  // Enciende la luz.

            battery -= drainRate * Time.deltaTime;  // Consume la bateria.

			RaycastHit hit;
			if (Physics.Raycast(transform.position, transform.forward, out hit, range))  // Lanza el raycast solo cuando la linterna esta activada.
			{
                EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();  // Detecta a cualquier enemigo.

				if (enemy != null)
                {
                    enemy.Repel(transform.position);
                }
			}
			else
			{
				lightSource.enabled = false;  // Apaga la luz si no esta usando la linterna o no hay batería.
			}
		}

        
    }

}

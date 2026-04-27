using UnityEngine;

// Hereda de EnemyBase
public class BichoNegroAI : EnemyBase
{
	private Rigidbody rb;  // Rigidbody para mover con físicas

	[SerializeField] private float normalSpeed = 3.5f; // Velocidad acechando
	[SerializeField] private float fastSpeed = 6f;     // Velocidad persiguiendo

	[SerializeField] private bool isRepelled = false;  // ¿Está siendo repelido?
	[SerializeField] private float repelTimer = 0f;    // Tiempo de repel

	[SerializeField] private float stateTimer = 0f;    // Cambio de comportamiento

	private float currentSpeed; // Velocidad actual usada

	void Start()
	{
		rb = GetComponent<Rigidbody>();

		// Configuración recomendada del rigidbody
		rb.freezeRotation = true;  // Evita que se vuelque
		currentSpeed = normalSpeed;
	}

	void FixedUpdate() // usamos FixedUpdate porque usamos físicas
	{
		// Si está en zona segura → no se mueve
		if (IsInSafeZone())
		{
			rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
			return;
		}

		HandleRepel();      // Maneja el estado de repel
		HandleMovement();   // Decide cómo se mueve
	}

	// Manejo del repel (linterna)
	public void HandleRepel()
	{
		if (isRepelled)
		{
			repelTimer -= Time.fixedDeltaTime;

			if (repelTimer <= 0)
			{
				isRepelled = false;
			}
		}
	}

	// Movimiento general
	public void HandleMovement()
	{
		// Si está repelido no persigue
		if (isRepelled) return;

		stateTimer -= Time.fixedDeltaTime;

		// Cambia comportamiento cada cierto tiempo
		if (stateTimer <= 0)
		{
			stateTimer = Random.Range(2f, 5f);

			// Decide si va rápido o lento
			if (Random.value > 0.5f)
			{
				currentSpeed = fastSpeed; // persecución
			}
			else
			{
				currentSpeed = normalSpeed; // acecho
			}
		}

		// Dirección hacia el jugador
		Vector3 dir = (GetPlayer().position - transform.position).normalized;

		// Movimiento con física (manteniendo gravedad)
		rb.linearVelocity = new Vector3(dir.x * currentSpeed, rb.linearVelocity.y, dir.z * currentSpeed);
	}

	// Repel con linterna
	public override void Repel(Vector3 from)
	{
		isRepelled = true;
		repelTimer = 2f;

		// Dirección opuesta a la linterna
		Vector3 dir = (transform.position - from).normalized;

		// Se impulsa hacia atrás
		rb.linearVelocity = new Vector3(dir.x * fastSpeed, rb.linearVelocity.y, dir.z * fastSpeed);
	}

	// Daño al jugador
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Prota"))
		{
			collision.gameObject.GetComponent<ProtaHealth>().TakeDamage();
		}
	}
}
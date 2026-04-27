using UnityEngine;
using UnityEngine.AI;

public class BichoNegroAI : EnemyBase
{

    NavMeshAgent agent;  // declaramos el NavMeshAgent para el enemigo.
    Rigidbody rb;  // declaramos el rigidbody.

    [SerializeField] private float normalSpeed = 3.5f;  // Velocidad acechando.
	[SerializeField] private float fastSpeed = 6.0f;  // Velocidad persiguiendo al prota.

	[SerializeField] private bool isRepelled = false;  // ¿El chobi esta siendo repelido?
	[SerializeField] private float repelTimer = 0f;  // Tiempo de repelion.

	[SerializeField] private float stateTimer = 0f;  // Tiempo para cambiar el comportamiento del chobi.


	

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;  // Esto evita quilombos con el NavMesh.

        agent.speed = normalSpeed;  // Velocidad inicial.
    }
    
    // Update is called once per frame
    void Update()
    {
        
        

        
        if (IsInSafeZone())  // Si está en la zona segura no te hace nada.
        {
            agent.ResetPath();
            return;
        }

        HandleRepel(); // Maneja el estado del repel.
        HandleMovement(); // decide si te esta persiguiendo o acechando.

    }

    public void HandleRepel()
    {
		if (isRepelled)  // ¿Esta siendo repelido por la linterna?
		{
			repelTimer -= Time.deltaTime;

			if (repelTimer <= 0)
			{
				isRepelled = false;
			}
		}
	}

    public void HandleMovement()
    {
        if (isRepelled) return;  // Si esta repelido, no persigue.
        
        stateTimer -= Time.deltaTime;

        if(stateTimer <= 0)  // Cada cierto tiempo cambia comportamiento.
        {
            stateTimer = Random.Range(2f, 5f);

			if (Random.value > 0.5f)    // Decide aleatoriamente si acecha o persigue.
			{
                agent.speed = fastSpeed;    // Modo persecucion, va rapido directo al prota.
                agent.SetDestination(GetPlayer().position);
			} else
            {
                agent.speed = normalSpeed;  // Modo acecho, va mas lento,

                Vector3 dir = (GetPlayer().position - transform.position).normalized;    // Posicion cercana al prota (no encima)
                Vector3 stalkPos = GetPlayer().position - dir * 3f;

                agent.SetDestination(stalkPos);
            }
		}
    }

    public override void Repel (Vector3 from)  // Llamado de la linterna.
    {
        isRepelled = true;
        repelTimer = 2f;

        Vector3 dir = (transform.position - from).normalized;   // Calcula la direccion opuesta al prota.

        Vector3 target = transform.position + dir * 5f; // Se mueve hacia atras.

        agent.SetDestination(target);
    }

	public void OnCollisionEnter(Collision collision)   // Daño al prota al tocarlo.
	{
        if (collision.gameObject.CompareTag("Prota"))
        {
            collision.gameObject.GetComponent<ProtaHealth>().TakeDamage();

		}
	}


}

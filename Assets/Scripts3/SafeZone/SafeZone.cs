using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
	// Lista global de todas las zonas seguras (para respawn, etc.)
	public static List<SafeZone> allSafeZones = new List<SafeZone>();

	private void Awake()
	{
		// Se agrega a la lista al iniciar
		allSafeZones.Add(this);
	}

	private void OnDestroy()
	{
		// Se elimina si se destruye
		allSafeZones.Remove(this);
	}

	// Cuando un enemigo entra a la zona segura
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			EnemyBase enemy = other.GetComponent<EnemyBase>();

			if (enemy != null)
			{
				// Indica que está en zona segura (deja de perseguir)
				enemy.SetSafeZone(true);

				// Calcula dirección contraria al jugador
				Vector3 dir = (other.transform.position - enemy.GetPlayer().position).normalized;

				// Aplica fuerza o velocidad para que se aleje
				Rigidbody rb = other.GetComponent<Rigidbody>();

				if (rb != null)
				{
					// Mantiene la velocidad vertical (gravedad)
					rb.linearVelocity = new Vector3(dir.x * 5f, rb.linearVelocity.y, dir.z * 5f);
				}
			}
		}
	}

	// Cuando el enemigo sale de la zona segura
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			EnemyBase enemy = other.GetComponent<EnemyBase>();

			if (enemy != null)
			{
				// Vuelve a comportamiento normal
				enemy.SetSafeZone(false);
			}
		}
	}
}
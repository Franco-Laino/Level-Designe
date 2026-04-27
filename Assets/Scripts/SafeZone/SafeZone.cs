using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SafeZone : MonoBehaviour
{

	private static List<SafeZone> allSafeZones = new List<SafeZone>();	// Lista global de todas las zonas seguras.

	private void Awake()
	{
		allSafeZones.Add(this);	// Se agrega a la lista al iniciar.
	}

	private void OnDestroy()
	{
		allSafeZones.Remove(this);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			EnemyBase enemy = other.GetComponent<EnemyBase>();  // Script del enemigo.

			if (enemy != null )	
			{
				enemy.SetSafeZone(true);	// Esto hace que deje de perseguir al prota.

				Vector3 dir = (other.transform.position - enemy.GetPlayer().position).normalized;  // Calcula la direccion contraria al prota.
				
				Vector3 escapePos = other.transform.position + dir * 5f;  // Posicion de escape.

				NavMeshAgent agent = other.GetComponent<NavMeshAgent>();

				if (agent != null )
				{
					agent.isStopped = false;	// Asegura que se mueva.

					agent.SetDestination(escapePos);  // Se aleja de la zona segura;
				}
			}
		}
	}


	private void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Enemy"))
		{
			EnemyBase enemy = other.GetComponent<EnemyBase>();


			if (enemy != null)
			{
				enemy.SetSafeZone(false);  // Puede volver a perseguir al prota.
			}
		}
	}


}

using UnityEngine;
using System;
using System.Collections;

public class ProtaHealth : MonoBehaviour
{

    [SerializeField] private int lives = 3;
    [SerializeField] private float invulnerableTime = 2f;

    [SerializeField] private bool isInvulnerable = false;

    public void TakeDamage()
    {
        if (isInvulnerable) return;

        lives--;


        if (lives > 0)
        {
            Respawn();
            StartCoroutine(Invulnerability());
        }
        else
        {
            GameOver();
        }
    }

	private void Respawn()
	{
		SafeZone closestZone = null;
		float minDistance = Mathf.Infinity;

		
		foreach (SafeZone zone in SafeZone.allSafeZones)	// Recorre todas las zonas seguras
		{
			float distance = Vector3.Distance(transform.position, zone.transform.position);

			if (distance < minDistance)
			{
				minDistance = distance;
				closestZone = zone;
			}
		}

		// Si encontró una zona, respawnea ahí
		if (closestZone != null)
		{
			transform.position = closestZone.transform.position;
		}
		else
		{
			Debug.LogWarning("No hay zonas seguras en la escena");
		}
	}

	private void GameOver()
	{
		Debug.Log("GAME OVER");
		gameObject.SetActive(false);
	}

	private IEnumerator Invulnerability()
	{
		isInvulnerable = true;
		yield return new WaitForSeconds(invulnerableTime);
		isInvulnerable = false;
	}
}

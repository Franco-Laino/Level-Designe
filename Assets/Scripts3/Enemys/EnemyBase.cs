using UnityEngine;

public class EnemyBase : MonoBehaviour
{

    [SerializeField] private Transform player;  // Referencia al prota.

    private bool inSafeZone = false;  // ¿Esta dentro de la zona segura?

    public Transform GetPlayer()    // Getter del player (solo lectura).
    {
        return player;
    }

    public void SetSafeZone (bool value)        // Setter de la zona segura.
    {
        inSafeZone = value;
    }

    public bool IsInSafeZone()  // Getter zona segura
	{
        return inSafeZone;
    }

    public virtual void Repel(Vector3 from)     // Método virtual para repeler
	{
        Debug.Log("Enemy repeled");
    }
}

using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    public Transform teleportTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerCharacter>(out var player))
        {
            if (teleportTarget != null)
            {
                player.SetPosition(teleportTarget.position);
            }
        }
    }
}
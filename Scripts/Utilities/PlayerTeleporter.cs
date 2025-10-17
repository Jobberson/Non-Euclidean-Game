using UnityEngine;
using Snog.Player.PlayerMovement;

public class PlayerTeleporter : MonoBehaviour
{
    public Transform teleportTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerTriggerDetector>(out _))
        {
            if (teleportTarget != null)
            {
                var player = other.GetComponentInParent<PlayerCharacter>();
                player.SetPosition(teleportTarget.position, false);
            }
        }
    }
}
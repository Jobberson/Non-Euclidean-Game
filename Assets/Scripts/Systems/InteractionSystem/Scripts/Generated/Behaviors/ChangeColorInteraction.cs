using UnityEngine;
using Snog.InteractionSystem.Core.Interfaces;

namespace Snog.InteractionSystem.Behaviors
{
    public class ChangeColorInteraction : MonoBehaviour, IInteractionBehavior
    {
        // Edit this to customioze the interation.
        public void Execute(GameObject target)
        {
            // Try to get the MeshRenderer component from the interacted object.
            if (!target.TryGetComponent<MeshRenderer>(out var meshRenderer))
            {
                Debug.LogWarning($"'{target.name}' has no MeshRenderer component.");
                return;
            }

            // Generate a new random color.
            Color randomColor = new
            (
                Random.value, // Red channel
                Random.value, // Green channel
                Random.value  // Blue channel
            );

            // Apply color to the renderer
            meshRenderer.material.color = randomColor;
        }
    }
}




using UnityEngine;
using Snog.InteractionSystem.Core.Interfaces; 
using Snog.InteractionSystem.Factories;      

namespace Snog.InteractionSystem.Core.Runtime
{
    [RequireComponent(typeof(Collider))]
    public class InteractibleObj : MonoBehaviour, IInteractable
    {
        [Header("Interaction Settings")]
        [Tooltip("The name of the interaction type (must match the name in the registry).")]
        [SerializeField] private string interactionTypeName = "Unknown";

        private IInteractionBehavior interactionBehavior;
        private string promptText;

        private void Start()
        {
            interactionBehavior = InteractionBehaviorFactory.GetBehavior(interactionTypeName);
            
            var promptAsset = InteractionPromptFactory.GetPromptAsset(interactionTypeName);

            if (promptAsset != null)
            {
                promptText = promptAsset.GetFormattedPrompt();
            }
            else
            {
                promptText = ""; 
            }
        }

        public void Interact()
        {
            interactionBehavior?.Execute(gameObject);
        }

        public void OnInteractEnter()
        {
            InteractionText.instance.SetText(promptText);
        }

        public void OnInteractExit()
        {
            InteractionText.instance.SetText("");
        }
    }
}
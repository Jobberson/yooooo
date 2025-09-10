using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] Transform interactorSource; // Point of origin for our Interaction source
    [SerializeField] float interactRange; // How far our Interaction can detect;
    private IInteractable currentInteractable; // Track the currently detected interactable object;
    public GameObject detectedObject;
    private void Update()
    {
        // We send a ray to detect all objects;
        Ray r = new(interactorSource.position, interactorSource.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
        {
            detectedObject = hitInfo.collider.gameObject;
            // We check if the object we collided with has the IInteractable interface;
            if (detectedObject.TryGetComponent(out IInteractable interactObj))
            {
                if (currentInteractable != interactObj)
                {
                    // Exit the previous interactable object if exists;
                    currentInteractable?.OnInteractExit();
                    // Enter the new interactable object;
                    interactObj.OnInteractEnter();
                    currentInteractable = interactObj;
                }
                // We call the Interact method from the detected object's IInteractable interface;
                interactObj.Interact();
                // We activate our text component;
                InteractionText.instance.textAppear.gameObject.SetActive(true);
            }
            else
            {
                //  No object detected, exit the previous interactable object if exists
                if (currentInteractable != null)
                {
                    // We deactivate our text component;
                    InteractionText.instance.textAppear.gameObject.SetActive(false);
                    currentInteractable.OnInteractExit();
                    currentInteractable = null;
                }
            }
        }
        else
        {
            InteractionText.instance.textAppear.gameObject.SetActive(false);
        }
    }
}
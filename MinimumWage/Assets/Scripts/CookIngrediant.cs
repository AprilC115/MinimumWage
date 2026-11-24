using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CookIngrediant : MonoBehaviour
{
    public float cookingTime;

    private XRSocketInteractor socketInteractor;
    private XRInteractionManager interactionManager;
    private Coroutine cookingCoroutine;

    private bool isReplacingFood = false;
    private AudioSource cookingSound;

    void Start()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
        interactionManager = FindFirstObjectByType<XRInteractionManager>();
        cookingSound = GetComponent<AudioSource>();
    }

    public void StartCooking(SelectEnterEventArgs args)
    {
        if (isReplacingFood) return;

        if (cookingCoroutine != null) StopCoroutine(cookingCoroutine);

        cookingCoroutine = StartCoroutine(CookingRoutine(args));
        cookingSound.Play();
        
    }

    public void StopCooking(SelectExitEventArgs args)
    {
        if (isReplacingFood) return;

        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
            cookingCoroutine = null;
            cookingSound.Stop();

            Debug.Log("Player removed food");
        }
    }

    private IEnumerator CookingRoutine(SelectEnterEventArgs args)
    {
        IXRSelectInteractable currentFood = args.interactableObject;

        FoodItem foodItem = currentFood.transform.GetComponent<FoodItem>();

        if (foodItem == null)
        {
            Debug.LogError("No fooditem");
            cookingCoroutine = null;
            yield break;
        }

        FoodCookingData recipe = foodItem.cookingData;

        //Stop if no recipe exists
        if (recipe == null)
        {
            Debug.LogError("No matching food recipe found!");
            cookingCoroutine = null;
            yield break;
        }

        //Wait for raw to cook
        yield return new WaitForSeconds(cookingTime);

        if (!IsStillInSocket(currentFood)) yield break;


        //Old pos
        Transform oldpos = currentFood.transform;
        Vector3 position = oldpos.position;
        Quaternion rotation = oldpos.rotation;

        isReplacingFood = true;
        //Destroy old food
        socketInteractor.interactionManager.SelectExit(socketInteractor, currentFood);
        Destroy(currentFood.transform.gameObject);

        // Check cooked prefab assignment
        if (recipe.cookedFood == null)
        {
            Debug.LogError("Cooked food prefab is missing from the recipe!");
            yield break;
        }

        yield return null;

        //Spawn cooked version
        GameObject cooked = Instantiate(recipe.cookedFood, position, rotation);
        Debug.Log("Cooked food instantiated: " + cooked.name);

        XRGrabInteractable cookedGrab = cooked.GetComponent<XRGrabInteractable>();

        if (cookedGrab == null)
        {
            Debug.LogError("Cooked food has no XR Grab Interactable on its root!");
            cookingCoroutine = null;
            yield break;
        }

        //Put cooking food into socket
        interactionManager.SelectEnter((IXRSelectInteractor) socketInteractor, (IXRSelectInteractable) cookedGrab);
        Debug.Log("Attempted to place cooked food into socket");
        currentFood = cookedGrab;

        isReplacingFood = false;

        yield return new WaitForSeconds(cookingTime);

        if (!IsStillInSocket(currentFood)) yield break;

        //replace cooked with burnt
        position = cooked.transform.position;
        rotation = cooked.transform.rotation;

        isReplacingFood = true;
        //Destroy cooked
        // socketInteractor.selectExited(socketInteractor, currentFood);
        socketInteractor.interactionManager.SelectExit(socketInteractor, currentFood);
        Destroy(cooked);

        yield return null;

        //Spawn burnet
        Debug.Log("Oh no it burnt");
        GameObject burnt = Instantiate(recipe.burntFood, position, rotation);

        XRGrabInteractable burntGrab = burnt.GetComponent<XRGrabInteractable>();
        if (cookingSound.isPlaying)
        {
            cookingSound.Stop();
        }

        if(burntGrab != null)
        {
           interactionManager.SelectEnter((IXRSelectInteractor)socketInteractor, (IXRSelectInteractable)burntGrab); 
        }
        isReplacingFood = false;

        
        cookingCoroutine = null;
    }

    private bool IsStillInSocket(IXRSelectInteractable food)
    {
        return socketInteractor.hasSelection && socketInteractor.interactablesSelected[0] == food;
    }
}

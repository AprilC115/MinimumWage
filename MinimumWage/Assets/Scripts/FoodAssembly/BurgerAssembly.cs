
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BurgerAssemblyManager : MonoBehaviour
{
    [Header("Completed Burger")]
    public GameObject burgerPrefab;

    [Header("Hands")]
    public NearFarInteractor leftHand;
    public NearFarInteractor rightHand;

    private bool burgerCompleted = false;

    private BurgerIngrediant bottom;
    private BurgerIngrediant patty;
    private BurgerIngrediant top;

    private NearFarInteractor lastHoldingHand;

    // Call this when a bottom bun is instantiated
    public void RegisterBottomBun(GameObject spawnedBun)
    {
        bottom = spawnedBun.GetComponent<BurgerIngrediant>();

        XRSocketInteractor socket =
            spawnedBun.GetComponent<XRSocketInteractor>();

        if (bottom == null || socket == null)
        {
            Debug.LogError("Bottom bun is missing required components!");
            return;
        }

        socket.selectEntered.AddListener(OnPattyEntered);

        Debug.Log("Bottom bun registered: " + spawnedBun.name);
    }

    private void Update()
    {
        if (burgerCompleted)
            return;

        CheckBurger();
    }

    private void OnPattyEntered(SelectEnterEventArgs args)
    {
        Debug.Log("Patty entered bottom bun socket!");

        patty = args.interactableObject.transform
            .GetComponentInParent<BurgerIngrediant>();

        if (patty == null)
        {
            Debug.LogError("Could not find BurgerIngrediant on patty!");
            return;
        }

        if (patty.type != BurgerIngrediant.Type.Patty)
        {
            Debug.LogWarning("Wrong ingredient entered bottom bun!");
            patty = null;
            return;
        }

        Debug.Log("Correct patty detected!");
    }

    private void CheckBurger()
    {
        if (bottom == null || patty == null)
            return;

        // Get top bun socket from patty
        XRSocketInteractor topSocket = patty.topBunSocket;

        if (topSocket == null)
            return;

        if (!topSocket.hasSelection)
            return;

        // Get top bun
        top = topSocket.GetOldestInteractableSelected()
            .transform.GetComponentInParent<BurgerIngrediant>();

        if (top == null)
            return;

        if (top.type != BurgerIngrediant.Type.TopBun)
            return;

        Debug.Log("ALL THREE INGREDIENTS DETECTED!");

        CompleteBurger();
    }

    public void SetLastHoldingHand(NearFarInteractor hand)
    {
        lastHoldingHand = hand;
    }

    private void CompleteBurger()
    {
        if (burgerPrefab == null)
        {
            Debug.LogError("Burger prefab is missing!");
            return;
        }

        burgerCompleted = true;

        Vector3 spawnPosition = bottom.transform.position;
        Quaternion spawnRotation = bottom.transform.rotation;

        if (lastHoldingHand != null)
        {
            spawnPosition = lastHoldingHand.transform.position;
            spawnRotation = lastHoldingHand.transform.rotation;
        }

        // Spawn completed burger
        GameObject newBurger = Instantiate(
            burgerPrefab,
            spawnPosition,
            spawnRotation
        );

        XRGrabInteractable grab =
            newBurger.GetComponent<XRGrabInteractable>();

        // Destroy original ingredients
        Destroy(bottom.gameObject);
        Destroy(patty.gameObject);
        Destroy(top.gameObject);

        // Automatically grab completed burger
        if (grab != null && lastHoldingHand != null)
        {
            lastHoldingHand.interactionManager.SelectEnter(
                (IXRSelectInteractor)lastHoldingHand,
                (IXRSelectInteractable)grab
            );
        }

        Debug.Log("Burger successfully assembled!");
    }
}

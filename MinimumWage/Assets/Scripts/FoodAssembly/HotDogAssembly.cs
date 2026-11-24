
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HotDogAssemblyManager : MonoBehaviour
{
    [Header("Completed Hot Dog")]
    public GameObject hotDogPrefab;

    [Header("Hands")]
    public NearFarInteractor leftHand;
    public NearFarInteractor rightHand;

    private HotDogIngrediant bun;
    private HotDogIngrediant sausage;

    private NearFarInteractor lastHoldingHand;

    private bool hotDogCompleted = false;

    public void RegisterHotDogBun(GameObject spawnedBun)
    {
        bun = spawnedBun.GetComponent<HotDogIngrediant>();

        XRSocketInteractor socket =
            spawnedBun.GetComponent<XRSocketInteractor>();

        if (bun == null || socket == null)
        {
            Debug.LogError("Hot dog bun is missing required components!");
            return;
        }

        if (bun.type != HotDogIngrediant.Type.HotDogBun)
            return;

        socket.selectEntered.AddListener(OnSausageEntered);

        Debug.Log("Hot dog bun registered!");
    }

    private void OnSausageEntered(SelectEnterEventArgs args)
    {
        if (hotDogCompleted)
            return;

        sausage = args.interactableObject.transform
            .GetComponentInParent<HotDogIngrediant>();

        if (sausage == null)
            return;

        if (sausage.type != HotDogIngrediant.Type.Sausage)
        {
            sausage = null;
            return;
        }

        Debug.Log("Sausage detected!");

        CompleteHotDog();
    }

    public void SetLastHoldingHand(NearFarInteractor hand)
    {
        lastHoldingHand = hand;
    }

    private void CompleteHotDog()
    {
        if (hotDogCompleted || bun == null || sausage == null)
            return;

        if (hotDogPrefab == null)
        {
            Debug.LogError("Hot dog prefab is missing!");
            return;
        }

        hotDogCompleted = true;

        Vector3 spawnPosition = bun.transform.position;
        Quaternion spawnRotation = bun.transform.rotation;

        if (lastHoldingHand != null)
        {
            spawnPosition = lastHoldingHand.transform.position;
            spawnRotation = lastHoldingHand.transform.rotation;
        }

        GameObject completedHotDog = Instantiate(
            hotDogPrefab,
            spawnPosition,
            spawnRotation
        );

        XRGrabInteractable grab =
            completedHotDog.GetComponent<XRGrabInteractable>();

        Destroy(bun.gameObject);
        Destroy(sausage.gameObject);

        if (grab != null && lastHoldingHand != null)
        {
            lastHoldingHand.interactionManager.SelectEnter(
                (IXRSelectInteractor)lastHoldingHand,
                (IXRSelectInteractable)grab
            );
        }

        Debug.Log("HOT DOG SUCCESSFULLY ASSEMBLED!");
    }
}

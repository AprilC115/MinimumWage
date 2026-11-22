using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using static UnityEngine.Rendering.GPUSort;

public class IngrediantCreation : MonoBehaviour
{
    public GameObject ingrediant;
    private XRGrabInteractable grabInteractable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    public void SpawnIngrediant(SelectEnterEventArgs args)
    {
        StartCoroutine(SpawnTimer(args));
    }
    private IEnumerator SpawnTimer(SelectEnterEventArgs args)
    {
        var manager = args.manager;
        var interactor = args.interactorObject;

        //getting hand pos
        Vector3 hand = interactor.transform.position;
        Quaternion handRotation = interactor.transform.rotation;

        // stops creating
        args.manager.SelectExit(interactor, grabInteractable);

        yield return new WaitForFixedUpdate();

        //creates ingrediant in hand
        GameObject ingrediantIns = Instantiate(ingrediant, hand, handRotation);

        yield return new WaitForFixedUpdate();

        // grabs the ingrediant
        var grabIngrediant = ingrediantIns.GetComponent<XRGrabInteractable>();

        if(grabIngrediant != null)
        {
            args.manager.SelectEnter(args.interactorObject, grabIngrediant);
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using static UnityEngine.Rendering.GPUSort;

public class IngrediantCreation : MonoBehaviour
{
    public GameObject ingrediant;
    public BurgerAssemblyManager burgerAssemblyManager;
    public HotDogAssemblyManager hotDogAssemblyManager;
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

        BurgerIngrediant burgerIngrediant = ingrediantIns.GetComponent<BurgerIngrediant>();

        

        if (burgerIngrediant != null && burgerIngrediant.type == BurgerIngrediant.Type.BottomBun)
        {
            burgerAssemblyManager.RegisterBottomBun(ingrediantIns);
        }

        HotDogIngrediant hotDogIngrediant = ingrediantIns.GetComponent<HotDogIngrediant>();
        if (hotDogIngrediant != null && hotDogIngrediant.type == HotDogIngrediant.Type.HotDogBun)
        {
            hotDogAssemblyManager.RegisterHotDogBun(ingrediantIns);
        }

        yield return new WaitForFixedUpdate();

        // grabs the ingrediant
        var grabIngrediant = ingrediantIns.GetComponent<XRGrabInteractable>();

        if(grabIngrediant != null)
        {
            args.manager.SelectEnter(args.interactorObject, grabIngrediant);
        }
    }
}

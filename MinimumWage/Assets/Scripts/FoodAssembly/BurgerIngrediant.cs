using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BurgerIngrediant : MonoBehaviour
{
    public enum Type
    {
        BottomBun,
        Patty,
        TopBun
    }

    [Header("Sockets")]
    public XRSocketInteractor topBunSocket;

    public Type type;
}

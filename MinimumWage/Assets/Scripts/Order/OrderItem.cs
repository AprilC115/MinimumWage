using UnityEngine;

public class OrderItem : MonoBehaviour
{
    public enum ItemType
    {
        Side,
        Main,
        Drink
    }
    public ItemType type;

    public string itemName;
}

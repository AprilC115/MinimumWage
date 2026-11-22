using UnityEngine;

[CreateAssetMenu(fileName = "Food Cooked Versions", menuName = "Cooking/Food Cooking")]
public class FoodCookingData : ScriptableObject
{
    public GameObject rawFood;
    public GameObject cookedFood;
    public GameObject burntFood;
}

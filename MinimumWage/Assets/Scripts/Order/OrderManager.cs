
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class OrderManager : MonoBehaviour
{
    [Header("Tray Sockets")]
    public XRSocketInteractor sideSocket;
    public XRSocketInteractor mainSocket;
    public XRSocketInteractor drinkSocket;

    [Header("Order Canvas")]
    public TMP_Text sideText;
    public TMP_Text mainText;
    public TMP_Text drinkText;

    [Header("Score")]
    public TMP_Text scoreText;

    [Header("Available Orders")]
    public OrderItem[] sides;
    public OrderItem[] mains;
    public OrderItem[] drinks;

    private OrderItem requiredSide;
    private OrderItem requiredMain;
    private OrderItem requiredDrink;

    private int score = 0;

    private string previousOrder = "";
    private AudioSource audio;

    private void Start()
    {
        UpdateScore();
        GenerateOrder();
        audio = GetComponent<AudioSource>();
    }

    private void GenerateOrder()
    {
        if (sides.Length == 0 ||
            mains.Length == 0 ||
            drinks.Length == 0)
        {
            Debug.LogError("Order arrays are empty!");
            return;
        }

        // Generate a new combination
        int attempts = 0;

        do
        {
            requiredSide = sides[Random.Range(0, sides.Length)];
            requiredMain = mains[Random.Range(0, mains.Length)];
            requiredDrink = drinks[Random.Range(0, drinks.Length)];

            attempts++;

        } while (
            requiredSide.itemName + requiredMain.itemName +
            requiredDrink.itemName == previousOrder
            && attempts < 30
        );

        previousOrder = requiredSide.itemName +
                        requiredMain.itemName +
                        requiredDrink.itemName;

        // Display the order
        sideText.text = "Side: " + requiredSide.itemName;
        mainText.text = "Main: " + requiredMain.itemName;
        drinkText.text = "Drink: " + requiredDrink.itemName;

        Debug.Log("New order generated!");
    }

    public void SubmitOrder()
    {
        // Make sure all sockets contain something
        if (!sideSocket.hasSelection ||
            !mainSocket.hasSelection ||
            !drinkSocket.hasSelection)
        {
            Debug.Log("Order incomplete!");
            return;
        }

        // Get ingredients from sockets
        OrderItem side = GetItem(sideSocket);
        OrderItem main = GetItem(mainSocket);
        OrderItem drink = GetItem(drinkSocket);

        if (side == null || main == null || drink == null)
        {
            Debug.Log("Could not identify all items!");
            return;
        }

        // Check whether the correct items were placed
        bool correctOrder =
            side.itemName == requiredSide.itemName &&
            main.itemName == requiredMain.itemName &&
            drink.itemName == requiredDrink.itemName;

        if (!correctOrder)
        {
            Debug.Log("Incorrect order! Try again.");
            return;
        }

        Debug.Log("Correct order!");

        // Increase score
        score += 10;
        UpdateScore();
        audio.Play();


        // Remove original food objects
        Destroy(side.gameObject);
        Destroy(main.gameObject);
        Destroy(drink.gameObject);

        // Generate next order
        GenerateOrder();

        Debug.Log("Order completed! +10 points!");
    }

    private OrderItem GetItem(XRSocketInteractor socket)
    {
        var interactable = socket.GetOldestInteractableSelected();

        if (interactable == null)
            return null;

        return interactable.transform
            .GetComponentInParent<OrderItem>();
    }

    private void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }
}

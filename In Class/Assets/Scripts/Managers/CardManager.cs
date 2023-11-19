using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public CardDataObject cardList;
    public GameObject cardPrefab;

    private void Awake()
    {
        // Read in avaiable cards
        if (!cardList)
            throw new System.Exception("List of Cards not found.");
    }

    public void ChooseCard(int index)
    {
        //SpawnNextPlayerCard();
    }

    public void SpawnNextPlayerCard(int index)
    {
        int playerCount = PlayerManager.instance.GetPlayerCount();
        if (playerCount == index)
            Debug.Log("All player cards chosen");

        ulong clientId = PlayerManager.instance.GetPlayerId(index);
        int[] cardIndex = GenerateRandomIndexes();
    }

    public int[] GenerateRandomIndexes()
    {
        List<int> activeCardIndex = new List<int>();
        for (int i = 0; i < cardCount; i++)
        {
            bool newIndexFound = false;
            while (!newIndexFound)
            {
                int rIndex = GetRandomAvailableCard();

                // If index already found
                if (activeCardIndex.Contains(rIndex))
                    continue;

                activeCardIndex.Add(rIndex);
                newIndexFound = true;
            }
        }

        return activeCardIndex.ToArray();
    }

    private int GetRandomAvailableCard()
    {
        int max = cardList.cardName.Length;
        return Random.Range(0, max);
    }
    private GameObject CreateCard(int rIndex1)
    {
        GameObject card = Instantiate(cardPrefab);
        card.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = cardList.cardName[rIndex1];

        return card;
    }


    private int cardCount = 3;
    public RectTransform centerRect;
    private float spacing = 4f;
    private void PositionCards(List<GameObject> cardsActive)
    {
        int count = cardCount ;
        Vector3 centerPosition = centerRect.position;
        if (count == 0)
        {
            cardsActive[0].GetComponent<RectTransform>().position = centerPosition;
            return;
        }
        float totalWidth = 0f;
        float halfTotalWidth = 0f;
        foreach (var card in cardsActive)
        {
            totalWidth += card.GetComponent<RectTransform>().rect.width / 100;
        }

        halfTotalWidth = (totalWidth + (spacing * (count - 1))) / 2;

        int i = 0;
        foreach (var card in cardsActive)
        {
            float xOffset = (i * (card.GetComponent<RectTransform>().rect.width / 100 + spacing)) - halfTotalWidth;
            Vector3 cardPosition = new Vector3(centerPosition.x + xOffset, centerPosition.y, centerPosition.z);
            card.GetComponent<RectTransform>().position = cardPosition;
            i++;
        }
    }
}

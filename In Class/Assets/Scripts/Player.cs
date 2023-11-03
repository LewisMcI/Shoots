using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode;
using UnityEngine;
public struct PlayerData
{
    public string playerName;
    public int characterSpriteIndex;
    public int accessorySpriteIndex;
}

public class Player : NetworkBehaviour
{
    PlayerData playerData = new PlayerData
    {
        playerName = "",
        characterSpriteIndex = 0,
        accessorySpriteIndex = 0
    };

    public string PlayerName { get => playerData.playerName;  }

    public GameObject defaultPlayerCard;
    public TMP_InputField playerInputField;

    private GameObject playerCard;
    public static Player Instance;

    public SpriteLookup spriteLookup;
    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.Log("Destroying other Player Instance");
            Destroy(this);
        }

        if (PlayerName == "")
            UpdateName(RandomNameGenerator.GenerateRandomName());
        playerInputField.text = PlayerName;
        //Debug.Log("Done: " + PlayerName);
    }


    public GameObject CreatePlayerCard(PlayerData newPlayerData)
    {
        //Debug.Log("Instantiating Player Card with Name: " + newPlayerData.playerName);
        // Create Default Player Card
        playerCard = Instantiate(defaultPlayerCard);
        // Set Player Name
        playerCard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newPlayerData.playerName;

        // Get Sprites
        Sprite characterSprite = spriteLookup.GetCharacterSprite(newPlayerData.characterSpriteIndex);
        Sprite accessorySprite = spriteLookup.GetAccessoriesSprite(newPlayerData.accessorySpriteIndex);
        playerCard.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = characterSprite;
        playerCard.transform.GetChild(1).GetChild(1).GetComponent<SpriteRenderer>().sprite = accessorySprite;

        return playerCard;
    }

    public void UpdateName(string name)
    {
        // Regex for removing double spaces
        playerData.playerName = Regex.Replace(name.Trim(), @"\s+", " ");
    }
    public void UpdateCharacterSprite(int spriteIndex)
    {
        Debug.Log("Update Character Sprite to Sprite: " + spriteIndex);
        playerData.characterSpriteIndex = spriteIndex;
    }
    public void UpdateAccessoriesSprite(int spriteIndex)
    {
        Debug.Log("Update Accessory Sprite to Sprite: " + spriteIndex);
        playerData.accessorySpriteIndex = spriteIndex;
    }

    /* Serialize current player data
     * 
     */
    public string SerializePlayerData()
    {
        return JsonUtility.ToJson(playerData);
    }

    /* Deserialize new player data
     * 
     */
    public PlayerData DeserializePlayerData(string jsonData)
    {
        return JsonUtility.FromJson<PlayerData>(jsonData);
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public Sprite GetCharacerSprite()
    {
        return spriteLookup.GetCharacterSprite(playerData.characterSpriteIndex);
    }
    public Sprite GetAccessorySprite()
    {
        return spriteLookup.GetAccessoriesSprite(playerData.accessorySpriteIndex);
    }
}

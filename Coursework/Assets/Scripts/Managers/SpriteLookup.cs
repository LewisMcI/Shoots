using System;
using System.Collections.Generic;
using UnityEngine;


public class SpriteLookup : MonoBehaviour
{
    // Character
    private Dictionary<int, Sprite> characterDataTable = new Dictionary<int, Sprite>();
    public SpritesDataObject characterData;

    // Accessories
    private Dictionary<int, Sprite> accessoriesDataTable = new Dictionary<int, Sprite>();
    public SpritesDataObject accessoriesData;


    // Populate the lookup table from the Sprite Data asset.
    private void Awake()
    {
        InitializeSpriteDictionary(characterDataTable, characterData);
        InitializeSpriteDictionary(accessoriesDataTable, accessoriesData);
    }

    private void InitializeSpriteDictionary(Dictionary<int, Sprite> dataTable, SpritesDataObject data)
    {
        if (data == null)
        {
            Debug.LogError("Sprite Data asset is not assigned.");
            return;
        }


        Sprite[] sprites = data.sprites;
        for (int i = 0; i < sprites.Length; i++)
        {
            dataTable.Add(i, sprites[i]);
        }

    }

    public Sprite GetCharacterSprite(int identifier)
    {
        if (characterDataTable.ContainsKey(identifier))
        {
            return characterDataTable[identifier];
        }
        else
        {
            Debug.LogError("Sprite not found for identifier: " + identifier);
            return null;
        }
    }

    public Sprite GetAccessoriesSprite(int identifier)
    {
        if (accessoriesDataTable.ContainsKey(identifier))
        {
            return accessoriesDataTable[identifier];
        }
        else
        {
            Debug.LogError("Sprite not found for identifier: " + identifier);
            return accessoriesDataTable[0];
        }
    }
}

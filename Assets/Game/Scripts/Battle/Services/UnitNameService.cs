using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitNameService
{
    private Dictionary<string, char> _unitNameDictionary = new Dictionary<string, char>();

    public char GetPostfix(string characterName)
    {
        if (this._unitNameDictionary.ContainsKey(characterName))
        {
            if (this._unitNameDictionary[characterName] == 'Z')
            {
                this._unitNameDictionary[characterName] = 'A';
            }
            else
            {
                this._unitNameDictionary[characterName]++;
            }
        }
        else
        {
            this._unitNameDictionary.Add(characterName, 'A');
        }
        return this._unitNameDictionary[characterName];
    }
}

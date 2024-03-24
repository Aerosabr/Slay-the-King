using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    
    //TEMP
    public List<string> characterName = new List<string>();
    public int counter;
    public int slot;

    public Text characterNameText;
    void Start()
    {
        //PlayerManager.instance.Players[slot].Class = characterName[counter];
    }

    public void nextName()
    {
        counter += 1;
        if (counter == characterName.Count)
            counter = 0;
        characterNameText.text = characterName[counter];
        PlayerManager.instance.ChangePlayerClass(slot, characterName[counter]);
    }

    public void prevName()
    {
        counter -= 1;
        if(counter < 0)
            counter = characterName.Count - 1;
        characterNameText.text = characterName[counter];
        PlayerManager.instance.ChangePlayerClass(slot, characterName[counter]);
    }

}

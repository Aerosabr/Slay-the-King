using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using UnityEngine.SceneManagement;
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public List<GameObject> Players = new List<GameObject>();
    public int NumPlayers = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    
    public void AddPlayer(int slot)
    {
        if (NumPlayers == 4)
            return;

        GameObject temp = Instantiate(Resources.Load<GameObject>("Prefabs/Classes/Berserker"), transform.GetChild(slot - 1));
        Transform position = GameObject.Find("CharacterSlotPositions").transform.GetChild(slot - 1).transform;
        temp.transform.position = position.transform.position;
        temp.transform.localScale = position.transform.localScale;
        temp.name = "Player" + slot;

        NumPlayers++;
    }

    public void ChangePlayerClass(int slot, string Class)
    {
        Destroy(transform.GetChild(slot).GetChild(0).gameObject);
        GameObject temp = Instantiate(Resources.Load<GameObject>("Prefabs/Classes/" + Class), transform.GetChild(slot));
        Transform position = GameObject.Find("CharacterSlotPositions").transform.GetChild(slot).transform;
        temp.transform.position = position.transform.position;
        temp.transform.localScale = position.transform.localScale;
        temp.name = "Player" + (slot + 1);
        temp.SetActive(true);
    }

    public void RemovePlayer(int slot)
    {
        Destroy(transform.GetChild(slot).GetChild(0).gameObject);
    }

    public void StartGame()
    {
        
    }

}

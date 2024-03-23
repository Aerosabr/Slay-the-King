using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using UnityEngine.SceneManagement;
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public List<Player> Players = new List<Player>();

    public List<CharacterSelector> slots = new List<CharacterSelector>();

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

    
    public void AddPlayer()
    {
        if (Players.Count == 4)
            return;
        List<Vector3> positions = new List<Vector3> { new Vector3(-6, 1, 0), new Vector3(-2, 1, 0), new Vector3(2.2f, 1, 0), new Vector3(6.2f, 1, 0) };
        Player player = new Player(Random.Range(1, 100));
        GameObject temp = Instantiate(Resources.Load<GameObject>("Prefabs/Classes/Mage"), transform);
        temp.gameObject.transform.position = positions[Players.Count];
        temp.gameObject.transform.localScale = new Vector3(0.75f, 0.75f, 0);
        temp.SetActive(true);
        Players.Add(temp.GetComponent<Player>());

        temp.name = "Player" + Players.Count;
    }

    public void RemovePlayer(int slot)
    {
        Players.RemoveAt(slot);
        Destroy(transform.GetChild(slot));
    }

    public void StartGame()
    {
        for (int i = 0; i < Players.Count; ++i)
        {
            GameObject temp = transform.GetChild(i).gameObject;
            GameObject temp2 = Resources.Load<GameObject>("Prefabs/Classes/" + temp.GetComponent<Player>().Class);

            for (int j = 0; j < 7; j++)
            {
                Destroy(temp.transform.GetChild(j).gameObject);
                temp2.transform.GetChild(j).gameObject.transform.SetParent(temp.transform);
            }

        }
    }

}

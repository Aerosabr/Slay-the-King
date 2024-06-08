using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager instance;
    [SerializeField] private GameObject Map;

    private void Awake()
    {
        instance = this;
    }

    public void LoadNextStage(string current)
    {
        GameManager.instance.canEquip = true;
        int stage = GameManager.instance.Stage;
        Map.SetActive(false);
        if (stage >= 1 && stage <= 3)
        {
            //If on stage 1-3, open Battle + 2 others
            GameObject map = Instantiate(Resources.Load<GameObject>("Prefabs/Maps/" + current + "/MapComplete3"), transform);
            map.transform.GetChild(0).GetComponent<StageTeleport>().SetTeleport("Battle");
            map.transform.GetChild(1).GetComponent<StageTeleport>().SetTeleport("Random");
            map.transform.GetChild(2).GetComponent<StageTeleport>().SetTeleport("Random", map.transform.GetChild(1).GetComponent<StageTeleport>().Stage);
        }
        else if (stage == 4)
        {
            //If on stage 4, open Elite
            GameObject map = Instantiate(Resources.Load<GameObject>("Prefabs/Maps/" + current + "/MapComplete1"), transform);
            map.transform.GetChild(0).GetComponent<StageTeleport>().SetTeleport("Elite");
        }
        else if (stage == 7)
        {
            //If on stage 7, open Battle + 2 others
            GameObject map = Instantiate(Resources.Load<GameObject>("Prefabs/Maps/" + current + "/MapComplete3"), transform);
            map.transform.GetChild(0).GetComponent<StageTeleport>().SetTeleport("Battle");
            map.transform.GetChild(1).GetComponent<StageTeleport>().SetTeleport("Random");
            map.transform.GetChild(2).GetComponent<StageTeleport>().SetTeleport("Random", map.transform.GetChild(1).GetComponent<StageTeleport>().Stage);
        }
        else if (stage == 8)
        {
            //If on stage 8, open Shop + 1 other
            GameObject map = Instantiate(Resources.Load<GameObject>("Prefabs/Maps/" + current + "/MapComplete2"), transform);
            map.transform.GetChild(0).GetComponent<StageTeleport>().SetTeleport("Shop");
            map.transform.GetChild(1).GetComponent<StageTeleport>().SetTeleport("Random");
        }

        if (stage == 10)
        {
            GameManager.instance.Stage = 1;
            GameManager.instance.Floor++;
        }
        else
            GameManager.instance.Stage++;
    }
}

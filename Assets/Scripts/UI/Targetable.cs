using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    public static Targetable instance;
    public List<GameObject> Target = new List<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    public void RemoveTargets()
    {
        foreach (GameObject target in Target)
        {
            target.SetActive(false);
        }
    }

    public void FindTargettable()
    {
        int count = 0;

        for (int i = 0; i < 3; i++)
        {
            if (BattleManager.instance.EnemyPositions[i, 0].hasCharacter)
            {
                count++;
                Target[i].SetActive(true);
            }
        }

        if (count == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                if (BattleManager.instance.EnemyPositions[i, 1].hasCharacter)
                {
                    count++;
                    Target[i + 3].SetActive(true);
                }
            }

            if (count == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (BattleManager.instance.EnemyPositions[i, 2].hasCharacter)
                    {
                        count++;
                        Target[i + 6].SetActive(true);
                    }
                }
            }
        }
    }
}

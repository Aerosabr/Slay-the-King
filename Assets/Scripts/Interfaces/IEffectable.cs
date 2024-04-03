using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectable
{
    public void ApplyBuff(Buff buff);
    public void RemoveBuff(Buff buff);
    public void HandleBuff();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth {

    public int CurrentHP { get; set; }
    public int MaxHP { get; set; }
    public void ReduceHp(int value);
}

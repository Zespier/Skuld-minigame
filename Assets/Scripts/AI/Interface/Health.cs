using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth {

    int currentHP { get; set; }
    int maxHP { get; set; }
    void Set(int value);
}

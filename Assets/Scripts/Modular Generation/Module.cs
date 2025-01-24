using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour {

    public ModuleHeight entrance;
    public ModuleHeight exit;
    public int ID;

    private void OnEnable() {
        ModuleContainer.instance.AddModule(this);
    }

    private void OnDisable() {
        ModuleContainer.instance.RemoveModule(this);
    }

    public void ResetSpecificVariables() {
        //In case the module has something like breaking platforms
    }
}

public enum ModuleHeight {
    Top,
    Midle,
    Bottom,
}
public enum ModuleType {
    TopToTop,
    TopToMiddle,
    TopToBottom,
    MiddleToTop,
    MiddleToMiddle,
    MiddleToBottom,
    BottomToTop,
    BottomToMiddle,
    BottomToBottom,
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour {

    private void Awake() {
        ForceIDOnPrefabs();
    }

    private void ForceIDOnPrefabs() {
        for (int i = 0; i < ModuleContainer.instance.modulePrefabs.Count; i++) {
            ModuleContainer.instance.modulePrefabs[i].ID = i;
        }
    }
}

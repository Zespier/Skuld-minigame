using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleContainer : MonoBehaviour {

    public List<Module> activeModules = new List<Module>(capacity: 8);
    public Camera mainCamera;
    public Module initialModule;
    public List<Module> modulePrefabs = new();
    public List<int> initialPoolAmount = new();
    public List<Module> _modulePool = new(capacity: 64);
    public List<int> modulesUsedInOrder = new List<int>(capacity: 64);

    public bool IsInIdleSide => PlayerController.instance.transform.position.y < -1.5f;

    public static ModuleContainer instance;
    private void Awake() {
        if (!instance) { instance = this; }

        ForceIDOnPrefabs();

        SetInitialPoolAmountByDefaultIfNotSet();

        InitializePool();
    }

    private void Update() {
        if (IsInIdleSide) {
            for (int i = 0; i < activeModules.Count; i++) {
                if (activeModules[i].entrance == ModuleHeight.Top || activeModules[i].entrance == ModuleHeight.Midle || activeModules[i].entrance == ModuleHeight.Bottom)
                    StoreModuleInPool(activeModules[i]);
            }
        }
    }

    #region Initialization

    private void ForceIDOnPrefabs() {
        for (int i = 0; i < modulePrefabs.Count; i++) {
            modulePrefabs[i].ID = i;
        }
    }

    public void SetInitialPoolAmountByDefaultIfNotSet() {
        if (initialPoolAmount.Count != modulePrefabs.Count) {
            initialPoolAmount.Clear();
            for (int i = 0; i < modulePrefabs.Count; i++) {
                initialPoolAmount.Add(2);
            }
        }
    }

    public void InitializePool() {
        for (int i = 0; i < modulePrefabs.Count; i++) {
            for (int j = 0; j < initialPoolAmount[i]; j++) {
                NewModule(i);
            }
        }
    }

    public void NewModule(int index) {
        Module newModule = Instantiate(modulePrefabs[index], transform);
        _modulePool.Add(newModule);

        newModule.gameObject.SetActive(false);
        return;
    }
    public void NewModule(ModuleHeight entranceHeight) {
        for (int i = 0; i < modulePrefabs.Count; i++) {
            if (modulePrefabs[i].entrance == entranceHeight) {

                Module newModule = Instantiate(modulePrefabs[i], transform);
                _modulePool.Add(newModule);

                newModule.gameObject.SetActive(false);
                return;
            }
        }

        Debug.LogError("Module not defined");
    }
    public void NewRandomModule(ModuleHeight entranceHeight) {
        Debug.LogWarning("Pensaba que no era posible que se llamara nunca");
        List<Module> allModulesWithThatHeight = new List<Module>();

        for (int i = 0; i < modulePrefabs.Count; i++) {
            if (modulePrefabs[i].entrance == entranceHeight) {
                allModulesWithThatHeight.Add(modulePrefabs[i]);
            }
        }

        if (allModulesWithThatHeight.Count > 0) {

            int randomIndex = Random.Range(0, allModulesWithThatHeight.Count);

            Module newModule = Instantiate(modulePrefabs[randomIndex], transform);
            _modulePool.Add(newModule);

            newModule.gameObject.SetActive(false);
            return;
        }

        Debug.LogError("Module not defined");
    }

    #endregion

    public Module GetInitialModule() {
        Module newModule = Instantiate(initialModule, transform);
        _modulePool.Add(newModule);

        newModule.transform.position = new Vector3(PlayerController.instance.transform.position.x + 3, 0, 0);
        newModule.mainCamera = mainCamera;
        return newModule;
    }

    public Module GetRandomModule(ModuleHeight moduleHeight) {

        List<Module> allModulesWithThatHeight = new List<Module>();

        for (int i = 0; i < _modulePool.Count; i++) {
            if (_modulePool[i].entrance == moduleHeight) {
                allModulesWithThatHeight.Add(_modulePool[i]);
            }
        }

        if (allModulesWithThatHeight.Count > 0) {
            int randomIndex = Random.Range(0, allModulesWithThatHeight.Count);

            return RetrieveModuleFromPool(_modulePool.FindIndex(m => m.ID == allModulesWithThatHeight[randomIndex].ID));
        }

        NewRandomModule(moduleHeight);

        return RetrieveModuleFromPool(_modulePool.Count - 1);
    }

    public Module RetrieveModuleFromPool(int index) {
        Module desiredModule = _modulePool[index];     
        _modulePool[index] = _modulePool[^1];        
        _modulePool.RemoveAt(_modulePool.Count - 1); 

        desiredModule.ResetSpecificVariables();
        desiredModule.gameObject.SetActive(true);
        if (desiredModule.ID != 0 && desiredModule.ID != 1) {
            modulesUsedInOrder.Add(desiredModule.ID);
        }
        return desiredModule;
    }

    public void StoreModuleInPool(Module module) {
        _modulePool.Add(module);
        module.gameObject.SetActive(false);
    }

    #region ActiveModules

    public void AddModule(Module module) {
        activeModules.Add(module);
    }

    public void RemoveModule(Module module) {
        activeModules.Remove(module);
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleContainer : MonoBehaviour {

    public List<Module> activeModules = new List<Module>(capacity: 8);
    public List<Module> modulePrefabs = new();
    public List<int> initialPoolAmount = new();
    public List<Module> _modulePool = new(capacity: 64);
    public List<int> modulesUsedInOrder = new List<int>(capacity: 64);

    public static ModuleContainer instance;
    private void Awake() {
        if (!instance) { instance = this; }

        SetInitialPoolAmountByDefaultIfNotSet();

        InitializePool();
    }

    #region Initialization

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

    public Module GetRandomModule(ModuleHeight moduleHeight) {

        List<Module> allModulesWithThatHeight = new List<Module>();

        for (int i = 0; i < _modulePool.Count; i++) {
            if (_modulePool[i].entrance == moduleHeight) {
                allModulesWithThatHeight.Add(_modulePool[i]);
            }
        }

        if (allModulesWithThatHeight.Count > 0) {
            int randomIndex = Random.Range(0, allModulesWithThatHeight.Count);

            return RetrieveModuleFromPool(randomIndex);
        }

        NewRandomModule(moduleHeight);

        return RetrieveModuleFromPool(_modulePool.Count - 1);
    }

    public Module RetrieveModuleFromPool(int index) {
        Module desiredModule = _modulePool[index];     //We save the value of the desired module
        _modulePool[index] = _modulePool[^1];         //Now that we saved it, we duplicate the last item
        _modulePool.RemoveAt(_modulePool.Count - 1);  //The last item, that is duplicated in 'index', is removed, so in the end only the desired module is out of the list, without the default swapping in lists => efficient

        desiredModule.gameObject.SetActive(true);
        desiredModule.ResetSpecificVariables();
        modulesUsedInOrder.Add(desiredModule.ID);
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

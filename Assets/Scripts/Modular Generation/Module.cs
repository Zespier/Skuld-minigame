using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour {

    public int ID;
    public ModuleHeight entrance;
    public ModuleHeight exitHeight;
    public float width = 1;
    public float height = 1;
    public Camera mainCamera;

    [Header("Reset")]
    [Tooltip("Referencia a todos los enemigos que researemos en la plataforma")]
    public List<Enemy> enemiesRef;
    [Tooltip("Layer de los objetos que recojeremos en la lista")]
    public LayerMask resetObjectLayer;

    private bool _spawnedRight;

    public float RightCameraLimit => mainCamera.transform.position.x + mainCamera.orthographicSize * mainCamera.aspect;
    public float LeftCameraLimit => mainCamera.transform.position.x - mainCamera.orthographicSize * mainCamera.aspect;

    private void OnEnable() {
        ModuleContainer.instance.AddModule(this);
    }

    private void OnDisable() {
        ModuleContainer.instance.RemoveModule(this);
    }

    private void Start() {
        enemiesRef = new List<Enemy>();
        List<GameObject> objects = GetFilteredChildren(transform, resetObjectLayer);
        for (int i = 0; i < objects.Count; i++) {
            enemiesRef.Add(objects[i].GetComponent<Enemy>());
        }
    }

    protected virtual void Update() {
        CheckDeactivation();

        if (!_spawnedRight && RightCameraLimit > transform.position.x + width) {

            Module newModule = ModuleContainer.instance.GetRandomModule(exitHeight);
            newModule.transform.position = transform.position + Vector3.right * width;
            newModule.mainCamera = mainCamera;

            _spawnedRight = true;
        }
    }

    private List<GameObject> GetFilteredChildren(Transform parent, LayerMask layer) {
        List<GameObject> filteredObjects = new List<GameObject>();

        foreach (Transform child in parent) {
            if (child.gameObject.layer == Mathf.Log(layer.value, 2)) {
                filteredObjects.Add(child.gameObject);
            }
        }

        return filteredObjects;
    }

    public virtual void ResetSpecificVariables() {
        _spawnedRight = false;

        foreach (Enemy resetObj in enemiesRef) {
            resetObj.ResetEnemy();
            break;

        }
    }

    private void CheckDeactivation() {

        if (LeftCameraLimit > transform.position.x + width) {
            ModuleContainer.instance.StoreModuleInPool(this);
        }
    }

    public void InitializeSection(Camera mainCamera) {
    }
}

public enum ModuleHeight {
    Top = 0,
    TopVanaheim = 6,
    Midle = 1,
    MidleVanaheim = 7,
    Bottom = 2,
    BottomVanaheim = 8,
    BaseModule = 3,
    BaseModuleVanaheim = 9,
    BackGround = 4,
    BackGroundVanaheim = 10,
    Initial = 5,
    InitialVanaheim = 11, //Último
}
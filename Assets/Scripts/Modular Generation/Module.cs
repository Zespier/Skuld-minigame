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
    public List<GameObject> enemiesRef;
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
        enemiesRef = GetFilteredChildren(transform, resetObjectLayer);
    }

    private List<GameObject> GetFilteredChildren(Transform parent, LayerMask layer) {
        List<GameObject> filteredObjects = new List<GameObject>();

        // Recorrer todos los hijos del objeto padre
        foreach (Transform child in parent) {
            // Comprobar si el hijo est� en la capa especificada
            if (child.gameObject.layer == Mathf.Log(layer.value, 2)) {
                // Comprobar si contiene el script espec�fico (si es necesario)
                //var script = child.GetComponent(scriptName); // scriptName es opcional
                //if (script != null)
                //{
                filteredObjects.Add(child.gameObject);
                //}
            }
        }

        return filteredObjects;
    }

    public virtual void ResetSpecificVariables() {
        _spawnedRight = false;

        foreach (GameObject resetObj in enemiesRef) {
            resetObj.SetActive(true);

            break;

        }
    }

    protected virtual void Update() {
        CheckDeactivation();

        if (!_spawnedRight && RightCameraLimit /*+ SectionManager.sectionSize*/ > transform.position.x + width/*+ SectionManager.sectionSize / 2f*/) {
            Module newModule = ModuleContainer.instance.GetRandomModule(exitHeight);
            newModule.transform.position = transform.position + Vector3.right * width;
            newModule.mainCamera = mainCamera;
            _spawnedRight = true;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + new Vector3(width * 0.5f, height * 0.5f, 0), new Vector3(width, height, 0));

        Gizmos.color = Color.blue;
        Gizmos.DrawCube(new Vector3(LeftCameraLimit, 0, 0), new Vector3(0.05f, height, 0));
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
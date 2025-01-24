using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Module : MonoBehaviour {

    public ModuleHeight entrance;
    public ModuleHeight exitHeight;
    public float width = 1;
    public float height = 1;
    public Camera mainCamera;

    [HideInInspector] public int ID;
    private bool _spawnedRight;

    public float RightCameraLimit => mainCamera.transform.position.x + mainCamera.orthographicSize * mainCamera.aspect;
    public float LeftCameraLimit => mainCamera.transform.position.x - mainCamera.orthographicSize * mainCamera.aspect;

    //private void OnEnable() {
    //    ModuleContainer.instance.AddModule(this);
    //}

    //private void OnDisable() {
    //    ModuleContainer.instance.RemoveModule(this);
    //}

    public void ResetSpecificVariables() {
        _spawnedRight = false;
    }

    private void Update() {
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

    /// <summary>
    /// Checks whether the section should deactivate relative to the camera
    /// </summary>
    private void CheckDeactivation() {

        if (LeftCameraLimit > transform.position.x + width) {
            gameObject.SetActive(false);
            ModuleContainer.instance.StoreModuleInPool(this);
        }
    }

    /// <summary>
    /// Inicializa la seccion dandole las referencias y la coordenada en la que se encuentra
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="mainCamera"></param>
    /// <param name="coordinate"></param>
    public void InitializeSection(Camera mainCamera) {
    }
}

public enum ModuleHeight {
    Top,
    Midle,
    Bottom,
    BaseModule,
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
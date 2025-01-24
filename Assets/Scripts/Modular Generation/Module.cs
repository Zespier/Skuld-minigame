using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Module : MonoBehaviour {

    public ModuleHeight entrance;
    public ModuleHeight exitHeight;
    public float width = 1;
    public float height = 1;
    public Camera _mainCamera;

    [HideInInspector] public int ID;
    private bool _spawnedRight;

    public float RightCameraLimit { get => _mainCamera.transform.position.x + _mainCamera.orthographicSize * _mainCamera.aspect; }

    private void OnEnable() {
        ModuleContainer.instance.AddModule(this);
    }

    private void OnDisable() {
        ModuleContainer.instance.RemoveModule(this);
    }

    public void ResetSpecificVariables() {
        //In case the module has something like breaking platforms
    }

    private void Update() {
        CheckDeactivation();

        if (!_spawnedRight && RightCameraLimit /*+ SectionManager.sectionSize*/ > transform.position.x /*+ SectionManager.sectionSize / 2f*/) {
            Module newModule = ModuleContainer.instance.GetRandomModule(exitHeight);
            newModule.transform.position = transform.position + Vector3.right * width;
            _spawnedRight = true;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));
    }

    /// <summary>
    /// Checks whether the section should deactivate relative to the camera
    /// </summary>
    private void CheckDeactivation() {

        if (RightCameraLimit - _mainCamera.orthographicSize * _mainCamera.aspect * 3 > transform.position.x /*+ SectionManager.sectionSize / 2f*/) {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Inicializa la seccion dandole las referencias y la coordenada en la que se encuentra
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="mainCamera"></param>
    /// <param name="coordinate"></param>
    public void InitializeSection(Camera mainCamera) {
        _mainCamera = mainCamera;
        _spawnedRight = false;
        gameObject.SetActive(true);
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
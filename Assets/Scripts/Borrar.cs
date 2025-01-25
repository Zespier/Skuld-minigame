using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Borrar : MonoBehaviour {

    public SpriteRenderer spriteRenderer;

    private void Update() {
        Debug.Log(spriteRenderer.bounds.size);
    }
}

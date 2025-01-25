using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCollectable : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Economy.instance.AddSoulCollectable();
            Destroy(gameObject);
        }
    }
}

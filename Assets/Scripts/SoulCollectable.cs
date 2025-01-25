using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SoulCollectable : MonoBehaviour {

    public Animator animator;
    public AnimationClip ploofClip;

    private async void Start() {
        await Task.Delay(Random.Range(0, 500));
        animator.Play("Idle");
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Economy.instance.AddSoulCollectable();
            animator.Play("Idle");

            Destroy(gameObject, ploofClip.length + 0.05f);
        }
    }
}

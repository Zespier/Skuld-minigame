using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SoulCollectable : MonoBehaviour {

    public Animator animator;
    public AnimationClip ploofClip;

    private async void OnEnable() {
        await Task.Delay(Random.Range(0, 500));
        animator.Play("Idle");
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Economy.instance.AddSoulCollectable();
            animator.Play("Ploof");

            StartCoroutine(C_WaitToDeactivate());
        }
    }

    private IEnumerator C_WaitToDeactivate() {
        float timer = ploofClip.length + 0.05f;

        while (timer >= 0) {
            timer -= Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}

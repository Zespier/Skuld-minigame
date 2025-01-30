using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class FlyingEnemySpawner : MonoBehaviour {

    public GameObject enemyPrefab;
    public List<GameObject> enemies;
    public Transform spawnPoint;
    public SpriteRenderer redAlert;
    public Transform parentFlyingEnemies;

    public float approximatelyCooldown;
    public float approximatelyCooldownBetweenEnemies;
    public int approximatelyMaxEnemies;

    private float timer;
    private float timerBetweenEnemies;
    private int enemyAmount;
    private float _xOffset = 2.2f;
    private List<GameObject> inactiveEnemies = new List<GameObject>();
    private GameObject actualEnemy;

    public bool IsInIdleSide => PlayerController.instance.transform.position.y < -1.5f;

    void Start() {
        timer = approximatelyCooldown;
        enemyAmount = Random.Range(1, approximatelyMaxEnemies + 1);
        timer = approximatelyCooldown + Random.Range(-0.5f, 0.5f);

        for (int i = 0; i < 10; i++) {
            GameObject enemy = Instantiate(enemyPrefab, parentFlyingEnemies);
            enemies.Add(enemy);
            inactiveEnemies.Add(enemy);
            Debug.Log("adadad");
        }
    }

    void Update() {
        if (IsInIdleSide) { return; }

        SpawnEnemy();
        redAlert.gameObject.SetActive(ActivateMark());
        if (actualEnemy != null) {
            redAlert.transform.position = CameraController.instance.transform.position + Vector3.right * _xOffset;
        }
    }

    void SpawnEnemy() {

        if (enemyAmount == 0 && timer <= 0) {
            enemyAmount = Random.Range(1, approximatelyMaxEnemies + 1);
            timer = approximatelyCooldown + Random.Range(-0.5f, 0.5f);
        } else if (timerBetweenEnemies <= 0 && enemyAmount > 0 && timer <= 0) {

            Vector3 posToInstantiate = spawnPoint.position;

            inactiveEnemies = enemies.FindAll(e => !e.activeInHierarchy);
            if (inactiveEnemies.Count == 0) {
                Debug.LogWarning("No hay enemigos inactivos disponibles.");
                return;
            }

            actualEnemy = inactiveEnemies[Random.Range(0, inactiveEnemies.Count)];


            actualEnemy.transform.position = posToInstantiate;
            actualEnemy.SetActive(true);

            timerBetweenEnemies = approximatelyCooldownBetweenEnemies + Random.Range(-0.5f, 0.5f);
            enemyAmount--;

        } else if (timerBetweenEnemies > 0 && timer <= 0) {
            timerBetweenEnemies -= Time.deltaTime;
            redAlert.gameObject.SetActive(true);
            StartCoroutine(C_IntermitentRedAlert());

        } else {
            timer -= Time.deltaTime;
        }
    }

    private bool ActivateMark() {
        for (int i = 0; i < inactiveEnemies.Count; i++) {
            if (inactiveEnemies[i].activeInHierarchy) {
                return true;
            }
        }
        return false;
    }

    private IEnumerator C_IntermitentRedAlert() {

        redAlert.color = Color.red;

        yield return StartCoroutine(C_IntermitentSpriteAfterHit(Color.clear));
        yield return StartCoroutine(C_IntermitentSpriteAfterHit(Color.red));
        yield return StartCoroutine(C_IntermitentSpriteAfterHit(Color.clear));
        yield return StartCoroutine(C_IntermitentSpriteAfterHit(Color.red));
        yield return StartCoroutine(C_IntermitentSpriteAfterHit(Color.clear));
    }

    private IEnumerator C_IntermitentSpriteAfterHit(Color color) {

        float timer = 0.15f;

        while (timer >= 0) {
            timer -= Time.deltaTime;
            yield return null;
        }

        redAlert.color = color;
    }
}

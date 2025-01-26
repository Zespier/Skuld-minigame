using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class FlyingEnemySpawner : MonoBehaviour {
    public List<GameObject> enemies;
    public Transform spawnPoint;
    public Transform enemyMark;
    public float xOffset;

    public float approximatelyCooldown;
    public float approximatelyCooldownBetweenEnemies;
    public int approximatelyMaxEnemies;

    private float timer;
    private float timerBetweenEnemies;
    private int enemyAmount;

    private List<GameObject> inactiveEnemies;
    GameObject actualEnemy;
    private void Awake() {
    }
    void Start() {
        timer = approximatelyCooldown;
        enemyAmount = Random.Range(1, approximatelyMaxEnemies + 1);
        timer = approximatelyCooldown + Random.Range(-0.5f, 0.5f);
        inactiveEnemies = enemies.FindAll(e => !e.activeInHierarchy);
    }

    void Update() {
        SpawnEnemy();
        enemyMark.gameObject.SetActive(ActivateMark());
        if (actualEnemy != null)
            enemyMark.position = new Vector2(spawnPoint.position.x, actualEnemy.transform.position.y);
    }

    void SpawnEnemy() {

        if (enemyAmount == 0 && timer <= 0) {
            enemyAmount = Random.Range(1, approximatelyMaxEnemies + 1);
            timer = approximatelyCooldown + Random.Range(-0.5f, 0.5f);
        } else if (timerBetweenEnemies <= 0 && enemyAmount > 0 && timer <= 0) {

            Vector3 posToInstantiate = spawnPoint.position;

            posToInstantiate.x += xOffset;

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
            enemyMark.gameObject.SetActive(true);

        } else {
            timer -= Time.deltaTime;
        }
    }
    bool ActivateMark() {
        for (int i = 0; i < inactiveEnemies.Count; i++) {
            if (inactiveEnemies[i].activeInHierarchy) {
                return true;
            }
        }
        return false;
    }
}

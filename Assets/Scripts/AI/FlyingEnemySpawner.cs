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
    public float timeBetweenBirds = 7f;
    public float _xOffsett = 1.8f;

    private Transform lastEnemySpawned;

    //public float approximatelyCooldown;
    //public float approximatelyCooldownBetweenEnemies;
    //public int approximatelyMaxEnemies;

    private float timer;
    //private float timerBetweenEnemies;
    //private int enemyAmount;

    public bool IsInIdleSide => PlayerController.instance.transform.position.y < -1.5f;
    public float RightCameraSide => CameraController.instance.transform.position.x + ModuleContainer.instance.mainCamera.orthographicSize * ModuleContainer.instance.mainCamera.aspect;

    void Start() {
        timer = timeBetweenBirds + Random.Range(-0.5f, 0.5f);

        for (int i = 0; i < 1; i++) {
            GameObject enemy = Instantiate(enemyPrefab, parentFlyingEnemies);
            enemies.Add(enemy);
        }
    }

    void Update() {
        if (IsInIdleSide) { redAlert.gameObject.SetActive(false); return; }

        Vector3 newPosition = CameraController.instance.transform.position + Vector3.right * _xOffsett + Vector3.down * CameraController.instance.gameplayHeightOffset + Vector3.forward * 8;
        newPosition.y = lastEnemySpawned != null ? lastEnemySpawned.position.y : 0;

        redAlert.transform.position = newPosition;

        timer -= Time.deltaTime;
        if (timer < 0) {
            SpawnEnemy();
        }

        if (lastEnemySpawned != null) {
            redAlert.gameObject.SetActive(lastEnemySpawned.position.x >= RightCameraSide);
        } 
    }

    void SpawnEnemy() {

        Debug.Log("Spawning");

        timer = timeBetweenBirds;

        Vector3 posToInstantiate = spawnPoint.position;

        GameObject enemy = enemies.Find(e => !e.activeInHierarchy);

        enemy.transform.position = posToInstantiate;
        enemy.SetActive(true);

        redAlert.gameObject.SetActive(true);

        lastEnemySpawned = enemy.transform;
    }
}

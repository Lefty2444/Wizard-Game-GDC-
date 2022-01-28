using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SpawnableEnemy {
    public string PoolName;
    public float Difficulty;
}

public class EnemySpawnManager : MonoBehaviour {
    public SceneBounds Bounds;
    public List<SpawnableEnemy> Enemies;
    public GameObject Player;

    private float _waveDifficulty = 4;

    private void Update() {
        //TODO: This should probably be replaced by whatever handles the game loop in the future
        if (Input.GetButtonDown("Next Wave")) {
            SpawnWave(_waveDifficulty);
            _waveDifficulty = Mathf.Pow(_waveDifficulty, 1.2f);
        }
    }

    //Spawns a wave of random enemies
    //The total difficulty of all spawned enemies should be as close to the given difficulty as possible
    public void SpawnWave(float difficulty) {
        int totalSpawned = 0;

        while (difficulty >= 1f) {
            SpawnableEnemy enemyInfo = PickEnemy(difficulty);
            GameObject enemy = ObjectPool.SharedInstance.GetReadyObject(enemyInfo.PoolName);

            difficulty -= enemyInfo.Difficulty;

            //Move on to the next enemy if we can't find a spawn location
            if (!TryFindSpawnLocation(out Vector2 spawnLocation)) {
                continue;
            }

            enemy.transform.position = spawnLocation;
            enemy.SetActive(true);

            //Safety check; Never spawn more than 200 enemies in one wave
            totalSpawned++;
            if (totalSpawned > 200)
                return;
        }
    }

    //This chooses an enemy to spawn using a weighted randomness algorithm
    //Enemies with a difficulty closer to the given difficulty value are more likely to be chosen
    private SpawnableEnemy PickEnemy(float difficulty) {
        //Collect a list of all enemies within the difficulty range
        //As well as their likeliness to be chosen
        List<(SpawnableEnemy Enemy, float Weight)> possibleEnemies = new List<(SpawnableEnemy, float)>();
        foreach (SpawnableEnemy enemy in Enemies) {
            if (enemy.Difficulty > difficulty)
                continue;

            possibleEnemies.Add( (enemy, enemy.Difficulty / difficulty) );
        }

        float weightSum = possibleEnemies.Sum(item => item.Weight);
        float value = Random.Range(0f, weightSum);
        foreach ((SpawnableEnemy enemy, float weight) in possibleEnemies) {
            value -= weight;
            if (!(value < 0f))
                continue;

            return enemy;
        }

        //Mathematically, this should not happen unless the possibleEnemies list is empty
        throw new System.Exception("Could not pick an enemy to spawn");
    }

    private bool IsValidSpawnLocation(Vector2 location) {
        //Enemies cannot spawn outside of bounds
        if (location.x < Bounds.left || location.x > Bounds.right ||
            location.y < Bounds.lower || location.y > Bounds.upper) {
            return false;
        }

        //Enemies cannot spawn within 3 units of the player
        const float range = 3f;
        Vector2 playerPos = Player.transform.position;
        Vector2 playerSize = Player.transform.lossyScale;
        if (location.x >= (playerPos.x - range) && location.x <= (playerPos.x + playerSize.x + range) &&
            location.y >= (playerPos.y - range) && location.y <= (playerPos.y + playerSize.y + range)) {
            return false;
        }

        return true;
    }

    private bool TryFindSpawnLocation(out Vector2 spawnLocation) {
        for (int i = 0; i < 10; i++) { //Maximum 10 attempts at finding a valid location before giving up
            spawnLocation = Bounds.GetRandomPos();

            if (IsValidSpawnLocation(spawnLocation)) {
                return true;
            }
        }

        spawnLocation = Vector2.zero;
        return false;
    }
}
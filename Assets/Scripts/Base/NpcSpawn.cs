using UnityEngine;

namespace Base
{
    public class NpcSpawn : MonoBehaviour
    {
        [SerializeField] private Transform spawnLimitNw;
        [SerializeField] private Transform spawnLimitNe;
        [SerializeField] private Transform spawnLimitSw;
        [SerializeField] private Transform spawnLimitSe;

        [SerializeField] private GameObject npcPrefab;
 
        [SerializeField] private int spawnLimitCount = 5;
        [SerializeField] private float spawnFrequency = 2f;

        [SerializeField] private LayerMask characterMask;

        private int _currentlySpawned = 0;
        private float _spawnNextCountdown;

        private void Start()
        {
            _spawnNextCountdown = spawnFrequency;
        }

        private void Update()
        {
            // Does nothing if the limit of spawned NPCs has been reached
            if (_currentlySpawned >= spawnLimitCount) 
                return;
            
            if (_spawnNextCountdown > 0f)
            {
                _spawnNextCountdown -= Time.deltaTime;
            }
            else
            {
                SpawnNpc();
                _spawnNextCountdown = spawnFrequency;
            }
        }

        private void SpawnNpc()
        {
            var spawnPosition = new Vector3(
                Random.Range(spawnLimitNw.position.x, spawnLimitNe.position.x),
                spawnLimitNw.position.y,
                Random.Range(spawnLimitSw.position.z, spawnLimitNe.position.z));

            var hitResults = new RaycastHit[1];

            var hitCount = Physics.RaycastNonAlloc(spawnPosition, Vector3.up * 10, hitResults, 10f, characterMask);
            
            // Character exists within boundaries, try again next frame
            if (hitCount > 0)
            {
                Debug.Log("NPC detected within boundary, skipping spawn");
                return;
            }
                

            var newNpc = Instantiate(npcPrefab, spawnPosition, Quaternion.identity);
            _currentlySpawned++;
        }

        private void DecreaseNpcCount()
        {
            _currentlySpawned--;
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VikingCrewTools.Sidescroller {
    /// <summary>
    /// The responsibility of this class is to keep track of team data, not team AI.
    /// It will make sure to spawn new guys for each team.
    /// </summary>
    public class TeamBehaviour : MonoBehaviour {
        [System.Serializable]
        public class CharacterSelectedEvent : UnityEngine.Events.UnityEvent<CharacterController2D> { }

        public string teamName = "Team";
        public List<Transform> spawnPoints;
        public bool doKeepSpawning = true;
        [Header("If using AI you should set this to false otherwise they may get stuck against each other")]
        public bool doCollideWithTeammates = false;
        public int maxNoOfTeammates = 3;
        public float timeBetweenSpawns = 5;
        public GameObject characterPrefab;
        public Transform teammatesParent;
        public List<Transform> teammates;
        public TeamBehaviour enemyTeam;
        public Material teamColors;
        public int totalGenerated = 0;
        public FirearmData[] weaponsToChoseFrom;
        public Transform playerControlledCharacter;
        public AIControls.AIState nextSpawnAIState;
        public Color minimapColor = Color.white;
        public CharacterSelectedEvent OnPlayerCharacterChanged;

        // Use this for initialization
        void Start() {
            StartCoroutine(ContinousSpawn());
            if (playerControlledCharacter != null)
                SetPlayerControl(playerControlledCharacter.gameObject);
        }

        // Update is called once per frame
        void Update() {

        }

        IEnumerator ContinousSpawn() {
            while (doKeepSpawning) {
                yield return new WaitForSeconds(timeBetweenSpawns);
                if (teammates.Count < maxNoOfTeammates && spawnPoints.Count > 0) {
                    Spawn();
                    nextSpawnAIState = AIControls.GetRandomAIState();
                }
            }
        }

        private void Spawn() {
            totalGenerated++;
            GameObject teammate = (GameObject)GameObject.Instantiate(characterPrefab, GetRandomSpawnPoint().position, Quaternion.identity);
            teammate.name = teamName + " " + characterPrefab.name + " " + totalGenerated.ToString();
            teammate.transform.SetParent(teammatesParent);
            HandleTeammateGenerated(teammate);
        }

        Transform GetRandomSpawnPoint() {
            return spawnPoints[Random.Range(0, spawnPoints.Count)];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">Data.</param>
        public void HandleTeammateGenerated(GameObject teammate) {
            CharacterController2D character = teammate.GetComponent<CharacterController2D>();
            character.Setup();

            TakeDamageBehaviour health = teammate.GetComponentInChildren<TakeDamageBehaviour>();
            health.OnDeath.AddListener(TeammateKilledCallback);
            teammates.Add(teammate.transform);

            AIControls ai = teammate.GetComponent<AIControls>();
            ai.enemies = enemyTeam.teammates;
            ai.allies = teammates;
            ai.EnterAIState(nextSpawnAIState);

            teammate.GetComponent<PlayerControls>().enabled = false;

            teammate.GetComponent<InventoryBehaviour>().GiveItem(weaponsToChoseFrom[Random.Range(0, weaponsToChoseFrom.Length)]);

            if (teamColors != null) {
                SkinnedMeshRenderer renderer = teammate.GetComponentInChildren<SkinnedMeshRenderer>();
                renderer.material = teamColors;
            }

            //Set teammates to not collide with each other. This is mainly due to AI being hard to program correctly otherwise
            //as they would then need local avoidance
            if (!doCollideWithTeammates) {
                SetTeammemberToIgnoreCollisionsWithTeammates(character);
            }

            //Setup minimap 
            SpriteRenderer sprite = teammate.GetComponentInChildren<SpriteRenderer>();
            sprite.color = minimapColor;

            Debug.Log(teammate + " was spawned");
        }

        private void SetTeammemberToIgnoreCollisionsWithTeammates(CharacterController2D character) {
            foreach (var teammate in teammates) {
                if (teammate == null) continue;
                foreach (var teammateBodypart in teammate.GetComponent<CharacterController2D>().bodyParts) {
                    foreach (var bodypart in character.bodyParts) {
                        teammateBodypart.SetToIgnoreCollision(bodypart);
                    }
                }
            }
        }

        public void TeammateKilledCallback(TakeDamageBehaviour teammateHealth) {
            Transform teammate = teammateHealth.transform;
            teammates.Remove(teammate);

            if (playerControlledCharacter != null && teammate.transform == playerControlledCharacter)
                Invoke("SetPlayerControl", timeBetweenSpawns);
        }

        public void SetPlayerControl() {
            if (teammates.Count > 0)
                SetPlayerControl(teammates[0].gameObject);
            else
                Invoke("SetPlayerControl", timeBetweenSpawns);
        }

        public void SetPlayerControl(GameObject teammate) {
            playerControlledCharacter = teammate.transform;
            AIControls ai = teammate.GetComponent<AIControls>();
            if (ai != null)
                ai.enabled = false;
            teammate.GetComponent<PlayerControls>().enabled = true;
            Camera.main.GetComponent<SmoothFollow2D>().target = teammate.GetComponent<PlayerControls>();
            Debug.Log("player controls new guy");
            OnPlayerCharacterChanged.Invoke(teammate.GetComponent<CharacterController2D>());
        }
    }
}
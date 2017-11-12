using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour {

    public const int PLANET_SIZE_MIN = 1;
    public const int PLANET_SIZE_MAX = 100;

    public static PlanetSpawner Instance;

    public static List<MassObject> PlanetsInPlay = new List<MassObject>();
    public List<MassObject> PlanetPrefabs = new List<MassObject>();

    public float SpawnChanceMin;
    public float SpawnChanceMod;
    public int MaxAmount = 20;


    void Awake(){
        Instance = this;
    }

    void Start () {
		StartCoroutine(_SpawnLoop());
    }
	
	void Update () {
		for (int i = 0; i < PlanetsInPlay.Count; i++){
			if(PlanetsInPlay[i].JustStarted)
				continue;
			if(PlanetsInPlay[i].CurrentState != MassObject.StateEnum.Default)
                return;
			if(PlanetsInPlay[i].MyMouth.ThingsBeingEaten.Count > 0)
                return;

            if (!PlanetsInPlay[i].IsVisibleOnScreen()) {
                RemovePlanetFromGame(PlanetsInPlay[i]);
            }
        }
	}
    public void RemovePlanetFromGame(MassObject _planet) {
        if (_planet == MassObject.CharacterInstance) {
            Debug.LogError("Something tried to delete the player!");
            return;
        }

        Destroy(_planet.gameObject);
        PlanetsInPlay.Remove(_planet);
	}

    IEnumerator _SpawnLoop() {
        float _spawnChance = 0;
        while (true){
            if (PlanetsInPlay.Count >= MaxAmount) {
                yield return null;
                continue;
			}

            if(PlanetsInPlay.Count == 0)
                _spawnChance = 1;
            else
            	_spawnChance = Mathf.Max(SpawnChanceMin, SpawnChanceMod / PlanetsInPlay.Count);

            if (Random.value < _spawnChance) {
                Vector3 _spawnPos = new Vector3(Mathf.Round(Random.value), Random.value, 0);
                float _sideOfCamera = Mathf.Sign(_spawnPos.x - 1);

                GameObject _obj = Instantiate(PlanetPrefabs[Random.Range(1, PlanetPrefabs.Count)].gameObject, Camera.main.ViewportToWorldPoint(_spawnPos), Quaternion.identity) as GameObject;
                _obj.transform.position = new Vector3(_obj.transform.position.x, _obj.transform.position.y, 0);

                MassObject _mo = _obj.GetComponent<MassObject>();
                PlanetsInPlay.Add(_mo);
                _mo.MyRigidBody.velocity += new Vector2(-_sideOfCamera, 0);

                float _mass = 1;
                if (Random.value > 0.5f) // 50% of smaller
                    _mass = Random.Range(PLANET_SIZE_MIN, MassObject.CharacterInstance.Mass);
				else
                    _mass = Random.Range(MassObject.CharacterInstance.Mass, Mathf.Min(PLANET_SIZE_MAX, MassObject.CharacterInstance.Mass * 1.25f));
                _mo.SetMass(_mass);
                _obj.transform.position += new Vector3((_mo.Mass * 0.5f) * _sideOfCamera, 0, 0);

            }

            yield return new WaitForSeconds(1);
        }
	}
}

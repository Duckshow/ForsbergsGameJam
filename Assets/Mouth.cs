using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouth : MonoBehaviour {

    public MassObject MyMassObject;
    public List<MassObject> ThingsBeingEaten = new List<MassObject>();

    public Transform MouthRenderer;
    public float MouthMinSize = 0.1f;
    public float MouthMaxSize = 0.8f;


    void Awake() {
        MyMassObject = GetComponent<MassObject>();
    }

	void Update () {
		if (MyMassObject.CurrentState == MassObject.StateEnum.BeingEaten)
            return;
		if (MyMassObject.CurrentState == MassObject.StateEnum.WaitCounterAttack)
            return;
		if (MyMassObject.CurrentState == MassObject.StateEnum.Volcano)
            return;

        bool _shouldHaveOpenMouth = false;
        float _closest = 10000;
        float _maxDist = MyMassObject.Mass * 2;
        for (int i = 0; i < PlanetSpawner.PlanetsInPlay.Count; i++){
            if(PlanetSpawner.PlanetsInPlay[i] == MyMassObject)
                continue;
			if(PlanetSpawner.PlanetsInPlay[i].Mass > MyMassObject.Mass)
                continue;
			if (PlanetSpawner.PlanetsInPlay[i].CurrentState == MassObject.StateEnum.BeingEaten)
                continue;
			if (PlanetSpawner.PlanetsInPlay[i].CurrentState == MassObject.StateEnum.WaitCounterAttack)
                continue;

            // in mouth range
            float _dist = (PlanetSpawner.PlanetsInPlay[i].transform.position - transform.position).magnitude;
            if (_dist < _maxDist) {
				_shouldHaveOpenMouth = true;
				if(_dist < _closest)
                    _closest = _dist;

				if (PlanetSpawner.PlanetsInPlay[i].CurrentState == MassObject.StateEnum.Eating)
                    continue;
				if (PlanetSpawner.PlanetsInPlay[i].CurrentState == MassObject.StateEnum.Volcano)
                    continue;

                // in swallow range
                if ((PlanetSpawner.PlanetsInPlay[i].transform.position - transform.position).magnitude < MyMassObject.Mass * 0.5f){
                    MyMassObject.CurrentState = MassObject.StateEnum.Eating;

                    PlanetSpawner.PlanetsInPlay[i].Eat(this);
                    ThingsBeingEaten.Add(PlanetSpawner.PlanetsInPlay[i]);
                }
			}
        }
		MouthRenderer.gameObject.SetActive(_shouldHaveOpenMouth);

        // unless something's going into the mouth, adjust the size of it
        if (_shouldHaveOpenMouth && ThingsBeingEaten.Count == 0){
            float _lerp = Mathf.Lerp(MouthMaxSize, MouthMinSize, _closest / _maxDist);
            Vector3 _size = new Vector3(_lerp, 1, _lerp);
            MouthRenderer.transform.localScale = _size;
        }
	}

    public void FinishedEating(MassObject _mo) {
        ThingsBeingEaten.Remove(_mo);
        MyMassObject.SetMass(MyMassObject.Mass + _mo.Mass);

		if (ThingsBeingEaten.Count == 0){
			MouthRenderer.transform.localScale = new Vector3(MouthMinSize, 1, MouthMinSize);
			MouthRenderer.gameObject.SetActive(false);
            MyMassObject.CurrentState = MassObject.StateEnum.Default;
        }
    }
}

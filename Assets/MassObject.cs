using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MassObject : MonoBehaviour {

    public const float VOLCANO_SPEED = 10;

    public static MassObject CharacterInstance;

    private static List<MassObject> AllMassObjects = new List<MassObject>();
    public Rigidbody2D MyRigidBody;
    public Renderer MyRenderer;
    public Mouth MyMouth;

    public bool JustStarted = false;
    public float Mass = 1f;


    public enum StateEnum { Default, Eating, BeingEaten, WaitCounterAttack, Volcano }
    public StateEnum CurrentState = StateEnum.Default;
    private Mouth MouthEatingThis;
    private float StartEatTime = 0;
    private Transform EatenTarget;
    private Vector3 EatenScaleStart;
    private Vector3 EatenPositionStart;

    void Awake() {
		if(CharacterInstance == null && GetComponent<CharacterMover>() != null)
            CharacterInstance = this;

        MyRigidBody = GetComponent<Rigidbody2D>();
        MyRenderer = GetComponent<Renderer>();
        MyMouth = GetComponent<Mouth>();
    }
    void OnEnable() {
        AllMassObjects.Add(this);

    }
    void OnDisable() {
        AllMassObjects.Remove(this);
    }

    void Start() {
        JustStarted = true;

        if (this == CharacterInstance) {
            PlanetSpawner.PlanetsInPlay.Add(this);
            SetMass(Mass);
		}
    }
    float RandomFloat(float _mod) {
        return (Random.value * _mod) * Mathf.Sign(Random.value * 2 - 1);
    }

    void Update() { 
		if(transform == null)
            return;

        if (Input.GetKeyDown(KeyCode.Space)){
            SetMass(Mass + 1);
        }
		if (Input.GetKeyDown(KeyCode.P)){
            Debug.Break();
        }

		if(IsVisibleOnScreen())
            JustStarted = false;

		if (CurrentState == StateEnum.WaitCounterAttack){
            transform.position = EatenTarget.position;

            Debug.Log(Time.time - timeAtStartWaitCounterAttack + "/" + WAIT_COUNTERATTACK_TIME);
            if (Time.time - timeAtStartWaitCounterAttack > WAIT_COUNTERATTACK_TIME)
                StopWaitCounterAttack();
			else if(Input.GetKeyDown(KeyCode.Space))
                CounterAttack();
        }
		else if (CurrentState == StateEnum.Volcano){
            if (Time.time - timeAtStartWaitVolcano > WAIT_VOLCANO_TIME)
                CurrentState = StateEnum.Default;
        }
		else if (CurrentState == StateEnum.BeingEaten){
			float t = Time.time - StartEatTime;
            Vector3 _targetScale = EatenTarget.localScale.magnitude > EatenScaleStart.magnitude ? EatenScaleStart : EatenTarget.localScale;
            Vector3 _scale = Vector3.Lerp(EatenScaleStart, _targetScale, t);
            _scale.z = 1;
			transform.localScale = _scale;
			transform.position = Vector2.Lerp(EatenPositionStart, EatenTarget.position, t);

			if (t >= 1) {
				MouthEatingThis.FinishedEating(this);
				if (this == CharacterInstance) {
					StartWaitCounterAttack();
				}
				else
					PlanetSpawner.Instance.RemovePlanetFromGame(this);
			}
        }
    }
    private const float WAIT_COUNTERATTACK_TIME = 0.5f;
    private float timeAtStartWaitCounterAttack = 0;
    void StartWaitCounterAttack() {
        CurrentState = StateEnum.WaitCounterAttack;
        Time.timeScale = 0.25f;

        timeAtStartWaitCounterAttack = Time.time;
    }
	private const float WAIT_VOLCANO_TIME = 1f;
    private float timeAtStartWaitVolcano = 0;
	void StopWaitCounterAttack(){
        CurrentState = StateEnum.Default;
        Time.timeScale = 1;

		MouthEatingThis = null;
        MyRigidBody.simulated = true;
        SetMass(Mathf.Max(PlanetSpawner.PLANET_SIZE_MIN, Mass * 0.5f));

        // volcano!
        MyRigidBody.velocity = new Vector2(VOLCANO_SPEED * (Random.value * 2 - 1), VOLCANO_SPEED * (Random.value * 2 - 1));
        CurrentState = StateEnum.Volcano;
        timeAtStartWaitVolcano = Time.time;
    }
    void CounterAttack() {
        CurrentState = StateEnum.Default;
        Time.timeScale = 1;

        MyRigidBody.simulated = true;
        SetMass(Mathf.Max(PlanetSpawner.PLANET_SIZE_MIN, Mass * 0.5f));

        PlanetSpawner.Instance.RemovePlanetFromGame(MouthEatingThis.MyMassObject);
        MouthEatingThis = null;
    }
    public void Eat(Mouth _mouth){
        MouthEatingThis = _mouth;
        CurrentState = StateEnum.BeingEaten;
        StartEatTime = Time.time;
        EatenScaleStart = new Vector2(Mass, Mass);
        EatenPositionStart = transform.position;
        EatenTarget = _mouth.transform;
        MyRigidBody.simulated = false;
    }
    public bool IsVisibleOnScreen() { 
		float _viewportPosL = Camera.main.WorldToViewportPoint(transform.position + new Vector3(-Mass * 0.5f, 0, 0)).x;
        float _viewportPosR = Camera.main.WorldToViewportPoint(transform.position + new Vector3(Mass * 0.5f, 0, 0)).x;
        float _viewportPosT = Camera.main.WorldToViewportPoint(transform.position + new Vector3(0, Mass * 0.5f, 0)).y;
        float _viewportPosB = Camera.main.WorldToViewportPoint(transform.position + new Vector3(0, -Mass * 0.5f, 0)).y;
		return !(_viewportPosR < 0 || _viewportPosL > 1 || _viewportPosT < 0 || _viewportPosB > 1);
	}
    public void SetMass(float _mass) {
		if(CurrentState == StateEnum.BeingEaten)
            return;
		if(CurrentState == StateEnum.WaitCounterAttack)
            return;

        Mass = _mass;
        transform.localScale = new Vector3(_mass, _mass, 1);
        MyRigidBody.mass = _mass;

        if (this == CharacterInstance) {
            BoundaryManager.Instance.SetCameraSize(Mathf.RoundToInt(Mass));
        }
    }

    void FixedUpdate () {
		if (CurrentState == StateEnum.BeingEaten)
            return;
		if (CurrentState == StateEnum.WaitCounterAttack)
            return;
		if (CurrentState == StateEnum.Volcano)
            return;

		for (int i = 0; i < AllMassObjects.Count; i++){
			if(AllMassObjects[i] == this)
                continue;
			if (AllMassObjects[i].CurrentState == StateEnum.WaitCounterAttack)
                continue;

            MyRigidBody.velocity += GAcceleration(AllMassObjects[i].transform.position, AllMassObjects[i].MyRigidBody.mass, MyRigidBody);
        }
	}

    private const float gravitationalConstant = 6.672e-1f;

    /// <summary>
    /// Calculates gravitational acceleration for a Rigidbody under the influence of a large "mass" point at a "position".
    /// Use this in each FixedUpdate(): rigidbody.velocity += GAcceleration(planetPosition, planetMass, rigidbody);
    /// </summary>
    /// <returns>The acceleration.</returns>
    /// <param name="position">Position of the planet's center of mass.</param>
    /// <param name="mass">Mass of the planet (kg). Use large values. </param>
    /// <param name="r">The Rigidbody to accelerate.</param>
    public static Vector2 GAcceleration(Vector2 position, float mass, Rigidbody2D r){
        Vector2 direction = position - r.position;
        float gravityForce = gravitationalConstant * ((mass * r.mass) / Mathf.Max(0.1f, direction.sqrMagnitude));
        gravityForce /= r.mass;

        return direction.normalized * gravityForce * Time.fixedDeltaTime;
    }
}

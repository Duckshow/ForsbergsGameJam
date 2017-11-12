using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryManager : MonoBehaviour {

    public static BoundaryManager Instance;
    private Bounds currentBoundary;

    public ParticleSystem StarsCenter;
    public ParticleSystem StarsL;
    public ParticleSystem StarsR;
    public ParticleSystem StarsT;
    public ParticleSystem StarsB;
    public ParticleSystem StarsTL;
    public ParticleSystem StarsTR;
    public ParticleSystem StarsBL;
    public ParticleSystem StarsBR;


    void Awake() {
        Instance = this;
    }

	void Update () {
        if (MassObject.CharacterInstance.transform.position.x > currentBoundary.center.x + currentBoundary.extents.x ||
            MassObject.CharacterInstance.transform.position.x < currentBoundary.center.x - currentBoundary.extents.x){
            OnExitBoundaryX();
        }
        if (MassObject.CharacterInstance.transform.position.y > currentBoundary.center.y + currentBoundary.extents.y ||
            MassObject.CharacterInstance.transform.position.y < currentBoundary.center.y - currentBoundary.extents.y){
            OnExitBoundaryY();
        }
    }

    void OnExitBoundaryX(){
        for (int i = 0; i < PlanetSpawner.PlanetsInPlay.Count; i++){
            float _x = PlanetSpawner.PlanetsInPlay[i].transform.position.x;
            float _y = PlanetSpawner.PlanetsInPlay[i].transform.position.y;
            Vector3 _corr = new Vector3(_x - (currentBoundary.size.x * Mathf.Sign(_x)), _y, 0);
            PlanetSpawner.PlanetsInPlay[i].transform.position = _corr;
        }
    }
    void OnExitBoundaryY() { 
        for (int i = 0; i < PlanetSpawner.PlanetsInPlay.Count; i++){
            float _x = PlanetSpawner.PlanetsInPlay[i].transform.position.x;
            float _y = PlanetSpawner.PlanetsInPlay[i].transform.position.y;
            Vector3 _corr = new Vector3(_x, _y - (currentBoundary.size.y * Mathf.Sign(_y)), 0);
            PlanetSpawner.PlanetsInPlay[i].transform.position = _corr;
        }
    }

    public void SetBoundary() {
		float _boundaryL = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0)).x;
        float _boundaryR = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, 0)).x;
        float _boundaryT = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1, 0)).y;
        float _boundaryB = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, 0)).y;

        currentBoundary = new Bounds(Vector2.zero, new Vector2(_boundaryR - _boundaryL, _boundaryT - _boundaryB));

        Debug.DrawLine(new Vector3(_boundaryL, _boundaryT, 0), new Vector3(_boundaryR, _boundaryT, 0), Color.red, Mathf.Infinity);
        Debug.DrawLine(new Vector3(_boundaryR, _boundaryT, 0), new Vector3(_boundaryR, _boundaryB, 0), Color.red, Mathf.Infinity);
        Debug.DrawLine(new Vector3(_boundaryR, _boundaryB, 0), new Vector3(_boundaryL, _boundaryB, 0), Color.red, Mathf.Infinity);
        Debug.DrawLine(new Vector3(_boundaryL, _boundaryB, 0), new Vector3(_boundaryL, _boundaryT, 0), Color.red, Mathf.Infinity);
    }

    public void SetCameraSize(int _size) { 
        if(_size == Camera.main.orthographicSize * 5)
            return;

        Camera.main.orthographicSize = _size * 4;
        SetBoundary();

        Vector3 _cachedPos = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(0, 0, _cachedPos.z);
        Vector4 _boundaries = new Vector4(
            Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, 0)).x,
            Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1, 0)).y,
            Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0)).x,
            Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, 0)).y
        );
        Camera.main.transform.position = _cachedPos;

        SetParticleShapeAndPos(StarsCenter, Vector3.zero, _boundaries);
        SetParticleShapeAndPos(StarsL, new Vector3(_boundaries.z * 2, 0, 0), _boundaries);
        SetParticleShapeAndPos(StarsR, new Vector3(_boundaries.x * 2, 0, 0), _boundaries);
        SetParticleShapeAndPos(StarsT, new Vector3(0, _boundaries.y * 2, 0), _boundaries);
        SetParticleShapeAndPos(StarsB, new Vector3(0, _boundaries.w * 2, 0), _boundaries);
        SetParticleShapeAndPos(StarsTL, new Vector3(_boundaries.z * 2, _boundaries.y * 2, 0), _boundaries);
        SetParticleShapeAndPos(StarsTR, new Vector3(_boundaries.x * 2, _boundaries.y * 2, 0), _boundaries);
        SetParticleShapeAndPos(StarsBR, new Vector3(_boundaries.x * 2, _boundaries.w * 2, 0), _boundaries);
        SetParticleShapeAndPos(StarsBL, new Vector3(_boundaries.z * 2, _boundaries.w * 2, 0), _boundaries);


    }
    void SetParticleShapeAndPos(ParticleSystem _sys, Vector3 _pos, Vector4 _boundaries) { 
        ParticleSystem.ShapeModule _shape = _sys.shape;
        _shape.box = new Vector3(_boundaries.x - _boundaries.z, _boundaries.y - _boundaries.w, 0);
        _sys.transform.position = _pos;

        _sys.Stop();
        _sys.Play();
    }
}

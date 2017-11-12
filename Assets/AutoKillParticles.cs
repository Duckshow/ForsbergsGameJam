using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoKillParticles : MonoBehaviour {

    ParticleSystem particles;


    void Awake() {
        particles = GetComponent<ParticleSystem>();
    }

	void Update () {
		if(!particles.IsAlive())
            Destroy(gameObject);
    }
}

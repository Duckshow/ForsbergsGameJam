using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour {

    public float MoveStrength = 1;
    private Rigidbody2D myRigidBody;


    void Awake(){
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate(){
		if(MassObject.CharacterInstance.CurrentState == MassObject.StateEnum.BeingEaten)
            return;
		if (MassObject.CharacterInstance.CurrentState == MassObject.StateEnum.WaitCounterAttack)
            return;
		if (MassObject.CharacterInstance.CurrentState == MassObject.StateEnum.Volcano)
            return;

        if (Input.GetMouseButton(0)) {
            myRigidBody.velocity += (Vector2)(Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized * MoveStrength;
        }
    }
}

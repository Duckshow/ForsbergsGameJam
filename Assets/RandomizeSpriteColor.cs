using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeSpriteColor : MonoBehaviour {

    public Color[] Colors;
    public bool RandomAlpha = false;

    private SpriteRenderer rend;


    void Start () {
        rend = GetComponent<SpriteRenderer>();

        if(Colors.Length > 0)
	        rend.color = Colors[Random.Range(0, Colors.Length)];
        if (RandomAlpha)
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, Random.value);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour {

    public SpriteRenderer RendererBody;
    public SpriteRenderer RendererEyes;
    public Transform TransformMouth;

	[Space]
    public Transform TransformShine;
    public Transform TransformSpots;
    public Transform TransformStripes;

	[Space]
    public Sprite Eyes_Default;
    public Sprite Eyes_OpenMouth;
    public Sprite Body;

    public AudioClip[] OpenMouthSounds;
    private AudioSource audio;


    void Awake() {
        audio = GetComponent<AudioSource>();
    }

    void Start(){
        RendererBody.sprite = Body;
        RendererEyes.sprite = Eyes_Default;

		if(TransformShine != null)
	        TransformShine.gameObject.SetActive(Random.value > 0.5f ? true : false);
        if(TransformSpots != null)
			TransformSpots.gameObject.SetActive(Random.value > 0.5f ? true : false);
		if(TransformStripes != null)
		    TransformStripes.gameObject.SetActive(Random.value > 0.5f ? true : false);
    }

    public void OpenMouth(bool b) {
		if (b){
    	    RendererEyes.sprite = Eyes_OpenMouth;
	        TransformMouth.gameObject.SetActive(true);

			if (!audio.isPlaying){
    	        audio.clip = OpenMouthSounds[Random.Range(0, OpenMouthSounds.Length)];
	            audio.Play();
			}
        }
		else{
			RendererEyes.sprite = Eyes_Default;
            TransformMouth.gameObject.SetActive(false);
		}
    }
}

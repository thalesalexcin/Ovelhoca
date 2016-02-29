using UnityEngine;
using System.Collections;

public class MovingCamera : MonoBehaviour {

    public Vector3 Velocity;
    public float ShowAfter;
    private float _Timer;
    private MeshRenderer _Renderer;

	// Use this for initialization
	void Start () 
    {
        _Renderer = GetComponent<MeshRenderer>();
        if (ShowAfter > 0 && _Renderer != null)
            _Renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        transform.Translate(Velocity * Time.deltaTime);

        _Timer += Time.deltaTime;
        if (_Timer >= ShowAfter && _Renderer != null)
            _Renderer.enabled = true;
	}
}

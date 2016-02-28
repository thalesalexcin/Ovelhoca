using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour 
{
    public Vector3 Velocity { get; set; }
	
	// Update is called once per frame
	void Update () 
    {
        transform.Translate(Velocity * Time.deltaTime);
	}

    void OnTriggerEnter(Collider collider)
    {

        var sheep = collider.GetComponent<SheepController>();
        
        if ((collider.tag == "Player" || tag == "Player") && sheep != null)
            sheep.Kill();

        Destroy(this.gameObject);
    }

}

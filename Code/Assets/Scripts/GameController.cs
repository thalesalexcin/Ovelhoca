using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public SheepController Player;
    public Canvas InfoCanvas;

	// Update is called once per frame
	void Update () 
    {
        var hazards = GameObject.FindGameObjectsWithTag("Hazard");

        var sheeps = hazards.Where(e => e.GetComponent<SheepController>() != null);
        if (sheeps.Count(s => !s.GetComponent<SheepController>().IsDead) == 0)
        {
            InfoCanvas.GetComponent<Animator>().SetTrigger("Winner");

            if (Input.GetKeyDown(KeyCode.Return))
            {
                var scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }
        }

        if (Player.IsDead)
        {
            InfoCanvas.GetComponent<Animator>().SetTrigger("GameOver");

            if (Input.GetKeyDown(KeyCode.Return))
            {
                var scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }
        }
	}
}

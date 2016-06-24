using UnityEngine;
using System.Collections;

public class GameObjectActivation : MonoBehaviour {
	public void Toggle() {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    public void Activation() {
        gameObject.SetActive(true);
    }

    public void Deactivation() {
        gameObject.SetActive(false);
    }
}

using UnityEngine;
using System.Collections;

public class URLHandler : MonoBehaviour {
    public void OpenURL(string URL) {
        Application.OpenURL(URL);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class WhenSetActivePressRRestart : MonoBehaviour
{
    void Update()
    {
        if (gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

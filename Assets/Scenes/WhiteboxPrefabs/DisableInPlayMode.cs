using UnityEngine;

public class DisableInPlayMode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }
}

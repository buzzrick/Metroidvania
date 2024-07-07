#nullable enable
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metroidvania.GameCore
{
    public class AutoSceneSwapper : MonoBehaviour
    {
        [SerializeField] private string _sceneName = default!;

        private void Start()
        {
            SceneManager.LoadScene(_sceneName);
        }

    }
}
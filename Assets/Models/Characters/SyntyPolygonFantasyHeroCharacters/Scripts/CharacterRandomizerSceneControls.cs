using PsychoticLab;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Models.Characters.SyntyPolygonFantasyHeroCharacters.Scripts
{
    public class CharacterRandomizerSceneControls : MonoBehaviour
    {
        private CharacterRandomizer[] _allCharacters;

        // reference to camera transform, used for rotation around the model during or after a randomization (this is sourced from Camera.main, so the main camera must be in the scene for this to work)
        private Transform camHolder;

        // cam rotation x
        float x = 16;

        // cam rotation y
        float y = -30;

        private void Awake()
        {
            _allCharacters = GameObject.FindObjectsOfType<CharacterRandomizer>();
        }

        private void Start()
        {
            // setting up the camera position, rotation, and reference for use
            Transform cam = Camera.main.transform;
            if (cam)
            {
                cam.position = transform.position + new Vector3(0, 0.3f, 2);
                cam.rotation = Quaternion.Euler(0, -180, 0);
                camHolder = new GameObject().transform;
                camHolder.position = transform.position + new Vector3(0, 1, 0);
                cam.LookAt(camHolder);
                cam.SetParent(camHolder);
            }
        }

        // randomize character creating button
        void OnGUI()
        {

            if (GUI.Button(new Rect(10, 100, 150, 50), "Randomize Characters"))
            {
                // call randomization method
                foreach (var character in _allCharacters)
                {
                    character.Randomize();
                }
            }

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 24;
            GUI.Label(new Rect(10, 10, 150, 50), "Hold Right Mouse Button Down\nor use W A S D To Rotate.", style);
        }



        private void Update()
        {
            if (camHolder)
            {
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    x += 1 * Input.GetAxis("Mouse X");
                    y -= 1 * Input.GetAxis("Mouse Y");
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    x -= 1 * Input.GetAxis("Horizontal");
                    y -= 1 * Input.GetAxis("Vertical");
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }

        void LateUpdate()
        {
            // method for handling the camera rotation around the character
            if (camHolder)
            {
                y = Mathf.Clamp(y, -45, 15);
                camHolder.eulerAngles = new Vector3(y, x, 0.0f);
            }
        }
    }
}
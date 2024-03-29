﻿using Buzzrick.UnityLibs.Attributes;
using KinematicCharacterController.Examples;
using UnityEngine;

namespace Metroidvania.Characters.NPC
{
    public class NPCCharacterAI : MonoBehaviour 
    {
        [SerializeField, RequiredField] private NPCCharacterController _npcCharacterController;
        private AICharacterInputs _inputs;

        private float MoveAngle = 0f;
        private float MoveVelocity = 0f;
        public float MaxVelocity = 0.5f;
        
        private void Awake()
        {
            _npcCharacterController.Enable(true);
            _inputs.MoveVector = Vector3.forward;
            _inputs.LookVector = Vector3.forward;
        }

        private void Update()
        {
            CheckForOutOfBounds();
            MoveAngle  += UnityEngine.Random.Range(-10f, 10f);
            MoveVelocity += UnityEngine.Random.Range(-0.05f, 0.05f);
            MoveVelocity = Mathf.Clamp(MoveVelocity, 0f, MaxVelocity);
            _inputs.MoveVector = Quaternion.Euler(0f, MoveAngle, 0f) * Vector3.forward * MoveVelocity;
            _inputs.LookVector = _inputs.MoveVector;    //  look the way that we're moving
            
            _npcCharacterController.SetInputs(ref _inputs);
        }

        private void CheckForOutOfBounds()
        {
            Vector3 position = transform.position;
            if (Mathf.Abs(position.x) > 45 || Mathf.Abs(position.y) > 45)
            {
                MoveAngle += 180;
            }
        }
    }
}
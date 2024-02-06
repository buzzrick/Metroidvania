using Cysharp.Threading.Tasks;
using Metroidvania.Player;
using System.Collections;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityEngine;
using System.Linq;
using PsychoticLab;
using Metroidvania.Configuration;
using System;

namespace Assets.Metroidvania.Debugging.DebugMenu
{
    public class DebugPlayerMenu : DebugPageBase
    {
        private PlayerMovementStatsSO[] _movementStats;
        private PlayerCore _playerCore;
        private GameConfiguration _gameConfiguration;
        private CharacterRandomizer _characterRandomiser;
        private PlayerMovementController_NoIK _characterToControl;

        protected override string Title { get; } = "Set Movement Type";

        public override IEnumerator Initialize()
        {
            AddButton("Randomize Character", clicked: () =>
            {
                _characterRandomiser.Randomize();
                DebugSheet.Instance.Hide();
            });


            //  obsolete with new playercontroller:

            //PickerCellModel movementPicker = new PickerCellModel();
            //movementPicker.Text = "Movement Type";
            //movementPicker.SetOptions(GetMovementTitlesList(), GetCurrentMovementTypeNum());
            //movementPicker.Clicked += HandleMovementPicker_Clicked;
            //movementPicker.Confirmed += HandleMovementPicker_Confirmed;
            //movementPicker.ActiveOptionChanged += HandleMovementPicker_ActiveOptionChanged;
            //AddPicker(movementPicker);


            AddSwitch(_gameConfiguration.FreeWorldUnlocksDebugging, "Free Unlocks", null, null, null, null, null, ToggleFreeWorldUnlocks);


            ////  as a list of buttons
            //foreach (var movementType in _movementStats)
            //{
            //    // Add a button to this page.
            //    AddButton(movementType.name, clicked: () => {
            //        Debug.Log($"Selected {movementType.name} Movement Type");
            //        _characterToControl.PlayerMovementStats = movementType;
            //    });
            //}

            // when you added any item, call Reload(). 
            Reload();

            yield break;
        }

        private void ToggleFreeWorldUnlocks(bool newValue)
        {
            _gameConfiguration.FreeWorldUnlocksDebugging = newValue;
            //_gameConfiguration.FreeWorldUnlocksDebugging = !_gameConfiguration.FreeWorldUnlocksDebugging;
        }

        private void HandleMovementPicker_ActiveOptionChanged(int movementID)
        {

            Debug.Log($"movement Picker {movementID}/{_movementStats.Length}");
            if (_movementStats.Length > movementID)
            {
                Debug.Log($"Selected movement type {_movementStats[movementID].name}");
                _characterToControl.PlayerMovementStats = _movementStats[movementID];
            }
        }

        private void HandleMovementPicker_Confirmed()
        {
            Debug.Log($"Selected movement type");
        }

        private void HandleMovementPicker_Clicked()
        {
            Debug.Log($"Selecting movement type");
        }

        private string[] GetMovementTitlesList()
        {
            return _movementStats.Select(i => i.name).ToArray();
        }

        private int GetCurrentMovementTypeNum()
        {
            for (int i = 0; i < _movementStats.Length; i++)
            {
                if (_movementStats[i] == _characterToControl.PlayerMovementStats)
                {
                    return i;
                }
            }
            return 0;
        }

        public void Setup(PlayerMovementStatsSO[] movementStats, PlayerCore playerCore, GameConfiguration gameConfiguration)
        {
            _movementStats = movementStats;
            _playerCore = playerCore;
            _gameConfiguration = gameConfiguration;
            _characterRandomiser = playerCore.GetPlayerRoot().GetComponentInChildren<CharacterRandomizer>();
            _characterToControl = playerCore.GetPlayerRoot().GetComponent<PlayerMovementController_NoIK>();
        }
    }
}
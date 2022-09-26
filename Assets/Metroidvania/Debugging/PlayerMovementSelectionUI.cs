using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Metroidvania.Debugging
{
    public class PlayerMovementSelectionUI : MonoBehaviour, ICore
    {
        public PlayerMovementStatsSO[] MovementStats;
        public RectTransform SelectionPanel;
        public ListButton ListButtonPrefab;

        private List<Button> _listButtons = new List<Button>();
        private PlayerRoot _playerRoot;
        private PlayerMovementController_NoIK CharacterToControl;

        [Inject]
        private void Initialise(PlayerCore playerCore)
        {
            _playerRoot = playerCore.GetPlayerRoot();
        }
        public UniTask StartCore()
        {
            RenderButtons();
            CharacterToControl = _playerRoot.GetComponent<PlayerMovementController_NoIK>();
            return UniTask.CompletedTask;
        }

        private void RenderButtons()
        {
            float buttonHeight = 0f;
            float buttonPosition = 0f;
            for (int i = 0; i < MovementStats.Length; i++)
            {
                var movementStat = MovementStats[i];
                ListButton newButton = Instantiate(ListButtonPrefab);
                newButton.transform.SetParent(SelectionPanel, false);
                RectTransform rect = newButton.GetComponent<RectTransform>();

                if (buttonHeight == 0f)
                {
                    buttonHeight = rect.rect.height;
                }
                buttonPosition += buttonHeight;
                Vector2 position = rect.position;
                position.y = buttonPosition;
                rect.position = position;

                newButton.SetButtonText(movementStat.name);
                newButton.OnButtonClick += SelectMovementStats;
            }
        }

        public void SelectMovementStats(string movementStatsName)
        {
            CharacterToControl.PlayerMovementStats = GetMovementStatsByName(movementStatsName);
        }

        private PlayerMovementStatsSO GetMovementStatsByName(string movementStatsName)
        {
            for (int i = 0; i < MovementStats.Length; i++)
            {
                var movementStat = MovementStats[i];
                if (string.Equals(movementStat.name, movementStatsName, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return movementStat;
                }
            }
            //  return the defaults
            return ScriptableObject.CreateInstance<PlayerMovementStatsSO>();
        }
    }
}
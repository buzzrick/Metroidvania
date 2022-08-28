using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementSelectionUI : MonoBehaviour
{
    public PlayerMovementStatsSO[] MovementStats;
    public RectTransform SelectionPanel;
    public ListButton ListButtonPrefab;

    public ThirdPersonMovement CharacterToControl;

    private List<Button> _listButtons = new List<Button>();

    private void Awake()
    {
        RenderButtons();
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

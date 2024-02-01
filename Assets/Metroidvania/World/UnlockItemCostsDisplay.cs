#nullable enable

using Buzzrick.UnityLibs.Attributes;
using Metroidvania.ResourceTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania.World
{
    /// <summary>
    /// Fill this out 
    /// </summary>
    public class UnlockItemCostsDisplay : MonoBehaviour
    {
        [SerializeField, RequiredField] private Image _resourceIcon = default!; 
        [SerializeField, RequiredField] private TMP_Text _resourceCountsText = default!;
        
        public void SetResourceCosts(ResourceTypeSO resourceType, int requiredAmount, int paidAmount)
        {
            _resourceIcon.sprite = resourceType.ResourceSprite;
            _resourceCountsText.text = $"<align=center>{paidAmount}/{requiredAmount}</align>";
        }

        private void Reset()
        {
            _resourceIcon = transform.Find("ResourceIcon")?.GetComponent<Image>() ?? default!;
            _resourceCountsText = transform.Find("ResourceCountsText")?.GetComponent<TMP_Text>() ?? default!;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using FancyScrollView;
using TMPro;

namespace RogueNaraka.ScrollScripts
{
    public class SkillScrollViewCell : FancyScrollViewCell<SkillData, SkillScrollViewContext>
    {
        [SerializeField]
        Animator animator;

        [SerializeField]
        TextMeshProUGUI skillName;
        [SerializeField]
        TextMeshProUGUI skillDesc;
        [SerializeField]
        TextMeshProUGUI cost;
        [SerializeField]
        Image image;
        [SerializeField]
        Button button;
        [SerializeField]
        Button buyBtn;
        [SerializeField]
        TextMeshProUGUI buyTxt;

        static readonly int scrollTriggerHash = Animator.StringToHash("scroll");

        void Start()
        {
            var rectTransform = transform as RectTransform;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchoredPosition3D = Vector3.zero;

            button.onClick.AddListener(OnPressedCell);
        }

        /// <summary>
        /// Updates the content.
        /// </summary>
        /// <param name="itemData">Item data.</param>
        public override void UpdateContent(SkillData itemData)
        {

            skillName.text = itemData.GetName();
            skillDesc.text = itemData.GetDescription();
            cost.text = string.Format("{0} Soul", itemData.cost.ToString());
            image.sprite = itemData.spr;
            buyBtn.onClick.RemoveAllListeners();
            buyBtn.onClick.AddListener(delegate { SoulShopManager.instance.BuySkill(buyBtn, buyTxt, itemData.id); });

            if (Context != null)
            {
                var isSelected = Context.SelectedIndex == DataIndex;
                //선택 했을 때
            }
        }

        /// <summary>
        /// Updates the position.
        /// </summary>
        /// <param name="position">Position.</param>
        public override void UpdatePosition(float position)
        {
            currentPosition = position;
            if(gameObject.activeSelf)
                animator.Play(scrollTriggerHash, -1, position);
            animator.speed = 0;
        }

        void OnPressedCell()
        {
            if (Context != null)
            {
                Context.OnPressedCell(this);
            }
        }

        
        float currentPosition = 0;
        void OnEnable()
        {
            UpdatePosition(currentPosition);
        }
    }
}

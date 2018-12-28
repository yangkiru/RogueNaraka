using System.Collections.Generic;
using UnityEngine;
using FancyScrollView;

namespace RogueNaraka.ScrollScripts
{
    public class SkillScrollView : FancyScrollView<SkillData, SkillScrollViewContext>
    {
        [SerializeField]
        ScrollPositionController scrollPositionController;

        void Awake()
        {
            scrollPositionController.OnUpdatePosition(p => UpdatePosition(p));
            SetContext(new SkillScrollViewContext { OnPressedCell = OnPressedCell });
        }

        public void UpdateData(List<SkillData> data)
        {
            cellData = data;
            scrollPositionController.SetDataCount(cellData.Count);
            UpdateContents();
        }

        void OnPressedCell(SkillScrollViewCell cell)
        {
            scrollPositionController.ScrollTo(cell.DataIndex, 0.4f);
            Context.SelectedIndex = cell.DataIndex;
            UpdateContents();
        }
    }
}

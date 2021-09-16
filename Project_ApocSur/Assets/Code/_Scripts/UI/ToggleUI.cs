namespace Projapocsur.Scripts
{
    public class ToggleUI : SelectableUI
    {
        public static readonly new string CompName = nameof(ToggleUI);

        public override void OnPointerLeftClick()
        {
            Toggle();
        }

        public void Toggle()
        {
            if (IsSelected)
            {
                OnDeselect();
            }
            else
            {
                OnSelect();
            }
        }
    }
}

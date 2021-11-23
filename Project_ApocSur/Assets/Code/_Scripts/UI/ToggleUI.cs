namespace Projapocsur.Scripts
{
    public class ToggleUI : SelectableUI
    {
        public static readonly new string CompName = nameof(ToggleUI);

        public override void OnLeftClick()
        {
            this.Toggle();
        }

        public void Toggle()
        {
            if (this.IsSelected)
            {
                this.OnDeselect();
            }
            else
            {
                this.OnSelect();
            }
        }
    }
}

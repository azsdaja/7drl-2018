namespace Assets.Scripts.GameLogic.GameCore
{
	public interface IInputHolder
	{
		PlayerInput PlayerInput { get; set; }
		PlayerInputModifier PlayerInputModifier { get; set; }
	}
}
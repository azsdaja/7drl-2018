using Assets.Scripts.Configuration;

namespace Assets.Scripts.GameLogic.GameCore
{
	public interface IGameConfig
	{
		NeedConfig NeedConfig { get; }
		ModeConfig ModeConfig { get; }
		SmellConfig SmellConfig { get; }
		int RngSeed { get; }
	}
}
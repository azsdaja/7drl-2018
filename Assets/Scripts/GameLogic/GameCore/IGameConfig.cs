using Assets.Cinemachine.Base.Runtime.Behaviours;
using Assets.Scripts.Configuration;

namespace Assets.Scripts.GameLogic.GameCore
{
	public interface IGameConfig
	{
		NeedConfig NeedConfig { get; }
		ModeConfig ModeConfig { get; }
		SmellConfig SmellConfig { get; }
		ActorConfig ActorConfig { get; }
		CinemachineVirtualCamera FollowPlayerCamera { get; }
		int RngSeed { get; }
	}
}
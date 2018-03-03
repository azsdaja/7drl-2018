using Assets.Scripts.GameLogic.GameCore;
using Zenject;

namespace Assets.Scripts.IoC
{
	public class MainInstaller : MonoInstaller
	{
		public GameContext GameContext;
		public GameConfig GameConfig;
		public UiConfig UiConfig;

		public override void InstallBindings()
		{
			GameGlobalsInstaller.Install(Container, GameContext, GameConfig, UiConfig);
		}
	}
}

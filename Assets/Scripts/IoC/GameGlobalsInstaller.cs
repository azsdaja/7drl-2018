using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.RNG;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.IoC
{
	public class GameGlobalsInstaller : Installer<GameContext, GameConfig, UiConfig, GameGlobalsInstaller>
	{
		[Inject]
		private readonly GameContext _gameContext;
		[Inject]
		private readonly GameConfig _gameConfig;
		[Inject]
		private readonly UiConfig _uiConfig;

		public override void InstallBindings()
		{
			// create default bindings
			Container.Bind(x => x.AllInterfaces()
					.Where(i =>
						i != typeof(IGameContext)
						&& i != typeof(IGameConfig)
						&& i != typeof(IUiConfig)
						&& i != typeof(IRandomNumberGenerator)
						&& i != typeof(IInputHolder)
					)
				)
				.To(x => x.AllNonAbstractClasses().InNamespaces("Assets.Scripts"))
				.AsSingle();

			Container.Bind<IGameContext>().FromInstance(_gameContext).AsSingle();
			Container.Bind<IGameConfig>().FromInstance(_gameConfig).AsSingle();
			Container.Bind<IUiConfig>().FromInstance(_uiConfig).AsSingle();
			int rngSeed = _gameConfig.RngSeed == 0 ? Random.Range(0, int.MaxValue) : _gameConfig.RngSeed;
			Container.Bind<IRandomNumberGenerator>().FromInstance(new RandomNumberGenerator(rngSeed)).AsSingle();
			Container.Bind<IInputHolder>().To<InputHolder>().AsSingle();

			ItemBehaviour itemPrefab = Resources.Load<ItemBehaviour>("Prefabs/Item");
			ActorBehaviour actorPrefab = Resources.Load<ActorBehaviour>("Prefabs/Enemy");
			Container.BindFactory<ItemBehaviour, ItemBehaviour.Factory>().FromComponentInNewPrefab(itemPrefab);
			Container.BindFactory<ActorBehaviour, ActorBehaviour.Factory>().FromComponentInNewPrefab(actorPrefab);

			Container.Bind<Material>().FromInstance(Resources.Load<Material>("Materials/Plain"))
				.WhenInjectedInto<PathRenderer>();
		}
	}
}

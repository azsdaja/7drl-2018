using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.ActionLoop;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.ActionLoop.AI;
using Assets.Scripts.GameLogic.Configuration;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.RNG;
using Assets.Scripts.UI;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Kafelki.Tests.GameLogic.ActionLoop
{
	[TestFixture]
	public class ActorControllerIntegrationTests
	{
		[Test]
		public void FriendlyActorsStaySideBySide_PlayerActorMovesTowardsOtherActor_TheyGetDisplacedAndAreUpdatedCorrectly()
		{
			var playerInput = PlayerInput.MoveLeft;
			var initialPlayerPosition = new Vector2Int(5,10);
			var initialOtherActorPosition = new Vector2Int(4,10);
			var playerActor = new ActorData
			{
				ControlledByPlayer = true,
				HasFreshFieldOfView = false,
				Energy = 1f,
				EnergyGain = .1f,
				LogicalPosition = initialPlayerPosition,
				Team = Team.Beasts,
			};
			var otherActor = new ActorData
			{
				LogicalPosition = initialOtherActorPosition,
				Team = Team.Beasts
			};
			
			var objectDetector = Mock.Of<IEntityDetector>(d => d.DetectActors(otherActor.LogicalPosition) == new[]{otherActor});
			var actionEffectFactoryMock = Mock.Get(Mock.Of<IActionEffectFactory>(f =>
				f.CreateMoveEffect(It.IsAny<ActorData>(), It.IsAny<Vector2Int>()) == Mock.Of<IActionEffect>()
			));
			IActionFactory actionFactory = new ActionFactory(
				Mock.Of<IGridInfoProvider>(), actionEffectFactoryMock.Object, Mock.Of<ISmeller>(), Mock.Of<ITextEffectPresenter>(), 
				Mock.Of<INeedHandler>(), Mock.Of<IRandomNumberGenerator>(), Mock.Of<IDeathHandler>(), Mock.Of<IEntityRemover>());

			IPlayerActionResolver playerActionResolver
				= new PlayerActionResolver(objectDetector, new InputHolder{PlayerInput = playerInput}, actionFactory );
			ITileVisibilityUpdater tileVisibilityUpdater = Mock.Of<ITileVisibilityUpdater>();
			var controller = new ActorController(It.IsAny<IAiActionResolver>(), playerActionResolver, tileVisibilityUpdater,
				 Mock.Of<INeedHandler>());

			bool passControl = controller.GiveControl(playerActor);

			// assertions:

			passControl.Should().BeTrue();

			// actors' positions should be swapped
			playerActor.LogicalPosition.Should().Be(initialOtherActorPosition);
			otherActor.LogicalPosition.Should().Be(initialPlayerPosition);

			// player energy should equal 0.1 which is initial energy (1) plus gain (0.1) minus action cost (1)
			bool energyIsCloseToExpected = Mathf.Abs(playerActor.Energy - 0.1f) < .000001f;
			Assert.That(energyIsCloseToExpected);

			// player should no longer have fresh field of view since he performed an action
			playerActor.HasFreshFieldOfView.Should().BeFalse();

			// correct MoveEffects have been created for both players 
			actionEffectFactoryMock.Verify(f => f.CreateMoveEffect(playerActor, initialPlayerPosition));
			actionEffectFactoryMock.Verify(f => f.CreateMoveEffect(otherActor, initialOtherActorPosition));
		}
	}
}
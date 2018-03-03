using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.ActionLoop;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.Configuration;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated;
using Assets.Scripts.RNG;
using Assets.Scripts.UI;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Kafelki.Tests.GameLogic.ActionLoop
{
	[TestFixture]
	public class PlayerActionResolverTests
	{
		[Test]
		public void GetAction_PlayerInputIsEmpty_ReturnsNull()
		{
			IInputHolder inputHolder = Mock.Of<IInputHolder>(h => h.PlayerInput == PlayerInput.None);
			var resolver = new PlayerActionResolver(It.IsAny<IEntityDetector>(), inputHolder, Mock.Of<IActionFactory>());

			IGameAction gameAction = resolver.GetAction(It.IsAny<ActorData>());

			gameAction.Should().BeNull();
		}

		[TestCase(PlayerInput.MoveUp, 0,1)]
		[TestCase(PlayerInput.MoveLeft, -1,0)]
		[TestCase(PlayerInput.MoveDown, 0,-1)]
		[TestCase(PlayerInput.MoveRight, 1,0)]
		public void GetAction_PlayerInputIsSetToMoveAndThereIsNoActorAtTarget_ReturnsCorrectMoveAction
																(PlayerInput input, int expectedXDelta, int expectedYDelta)
		{
			IInputHolder inputHolder = Mock.Of<IInputHolder>(h => h.PlayerInput == input);
			var resolver = new PlayerActionResolver(Mock.Of<IEntityDetector>(), inputHolder, 
				new ActionFactory(null, null, null, Mock.Of<ITextEffectPresenter>(), Mock.Of<INeedHandler>(), Mock.Of<IRandomNumberGenerator>(),
				Mock.Of<IDeathHandler>(), Mock.Of<IEntityRemover>()));
			float expectedEnergyCost = 1.0f;

			var actionData = (MoveAction)resolver.GetAction(new ActorData());

			actionData.Direction.Should().Be(new Vector2Int(expectedXDelta, expectedYDelta));
			actionData.EnergyCost.Should().Be(expectedEnergyCost);
		}

		[TestCase(PlayerInput.MoveUp, 0,1)]
		[TestCase(PlayerInput.MoveLeft, -1,0)]
		[TestCase(PlayerInput.MoveDown, 0,-1)]
		[TestCase(PlayerInput.MoveRight, 1,0)]
		public void GetAction_PlayerInputIsSetToMoveAndThereIsFriendlyActorAtTarget_ReturnsCorrectDisplaceAction
			(PlayerInput input, int expectedXDelta, int expectedYDelta)
		{
			IInputHolder inputHolder = Mock.Of<IInputHolder>(h => h.PlayerInput == input);
			ActorData actor = new ActorData{Team = Team.Beasts};
			ActorData otherActor = Mock.Of<ActorData>(d => d.Team == Team.Beasts);
			IEntityDetector entityDetector = Mock.Of<IEntityDetector>(d => d.DetectActors(It.IsAny<Vector2Int>()) == new[]{otherActor});
			var resolver = new PlayerActionResolver(entityDetector, inputHolder, 
				new ActionFactory(null, null, null, Mock.Of<ITextEffectPresenter>(), Mock.Of<INeedHandler>(), 
				Mock.Of<IRandomNumberGenerator>(), Mock.Of<IDeathHandler>(), Mock.Of<IEntityRemover>()));
			float expectedEnergyCost = 1.0f;

			var actionData = (DisplaceAction)resolver.GetAction(actor);

			actionData.DisplacedActor.Should().Be(otherActor);
			actionData.EnergyCost.Should().Be(expectedEnergyCost);
		}

		[TestCase(PlayerInput.MoveUp, 0,1)]
		[TestCase(PlayerInput.MoveLeft, -1,0)]
		[TestCase(PlayerInput.MoveDown, 0,-1)]
		[TestCase(PlayerInput.MoveRight, 1,0)]
		public void GetAction_PlayerInputIsSetToMoveAndThereIsHostileActorAtTarget_ReturnsAttackAction
			(PlayerInput input, int expectedXDelta, int expectedYDelta)
		{
			IInputHolder inputHolder = Mock.Of<IInputHolder>(h => h.PlayerInput == input);
			var actor = new ActorData{Team = Team.Beasts};
			var otherActor = new ActorData{Team = Team.Humans};
			IEntityDetector entityDetector = Mock.Of<IEntityDetector>(d => d.DetectActors(It.IsAny<Vector2Int>()) == new[]{ otherActor });
			var resolver = new PlayerActionResolver(entityDetector, inputHolder, 
				new ActionFactory(null, null, null, Mock.Of<ITextEffectPresenter>(), Mock.Of<INeedHandler>(), Mock.Of<IRandomNumberGenerator>(), 
				Mock.Of<IDeathHandler>(), Mock.Of<IEntityRemover>()));
			float expectedEnergyCost = 1.0f;

			var actionData = (AttackAction)resolver.GetAction(actor);

			actionData.ActorData.Should().Be(actor);
			actionData.AttackedActor.Should().Be(otherActor);
			actionData.EnergyCost.Should().Be(expectedEnergyCost);
		}

		[Test]
		public void GetAction_GettingActionClearsInput()
		{
			var inputHolderMock = new Mock<IInputHolder>();
			inputHolderMock.SetupGet(h => h.PlayerInput).Returns(PlayerInput.MoveDown);
			var resolver = new PlayerActionResolver(Mock.Of<IEntityDetector>(), inputHolderMock.Object, 
				new ActionFactory(null, null, null, Mock.Of<ITextEffectPresenter>(), Mock.Of<INeedHandler>(), Mock.Of<IRandomNumberGenerator>(), 
				Mock.Of<IDeathHandler>(), Mock.Of<IEntityRemover>()));

			resolver.GetAction(new ActorData());

			inputHolderMock.VerifySet(h => h.PlayerInput = PlayerInput.None);
		}
	}
}
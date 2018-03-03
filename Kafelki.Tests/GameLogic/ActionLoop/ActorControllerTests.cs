using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.ActionLoop;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.ActionLoop.AI;
using Assets.Scripts.GridRelated;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Kafelki.Tests.GameLogic.ActionLoop
{
	[TestFixture]
	class ActorControllerTests
	{
		class TestGameAction : GameAction
		{
			public TestGameAction(ActorData actorData, float energyCost) : base(actorData, energyCost, Mock.Of<IActionEffectFactory>())
			{
			}

			public override IEnumerable<IActionEffect> Execute() { return Enumerable.Empty<IActionEffect>(); }
		}

		[Test]
		public void GiveControl_ActorHasNotEnoughEnergy_PassesControlAndDoesNotResolveAction()
		{
			var aiActionResolverMock = new Mock<IAiActionResolver>();
			var playerActionResolverMock = new Mock<IPlayerActionResolver>();
			var actorController = new ActorController(aiActionResolverMock.Object, playerActionResolverMock.Object, 
				It.IsAny<ITileVisibilityUpdater>(), Mock.Of<INeedHandler>());
			ActorData actorData = new ActorData {Energy = 0.1f};

			bool result = actorController.GiveControl(actorData);

			result.Should().BeTrue();
		}

		[Test]
		public void GiveControl_ActorHasNotEnoughEnergy_EnergyIsIncreasedByEnergyGain()
		{
			var actorController = new ActorController(It.IsAny<IAiActionResolver>(), It.IsAny<IPlayerActionResolver>(), 
				It.IsAny<ITileVisibilityUpdater>(), Mock.Of<INeedHandler>());
			ActorData actorData = new ActorData
			{
				ControlledByPlayer = true,
				Energy = 0.1f,
				EnergyGain = 0.2f
			};
			float expectedEnergy = .3f;

			actorController.GiveControl(actorData);

			actorData.Energy.Should().Be(expectedEnergy);
		}
		
		[Test]
		public void GiveControl_ActorHasEnoughEnergyAndActionHasNotBeenResolved_ActorHoldsControl()
		{
			var actorController = new ActorController(Mock.Of<IAiActionResolver>(), Mock.Of<IPlayerActionResolver>(),
				Mock.Of<ITileVisibilityUpdater>(), Mock.Of<INeedHandler>());
			ActorData actorData = new ActorData
			{
				ControlledByPlayer = true,
				Energy = 1
			};

			bool result = actorController.GiveControl(actorData);

			result.Should().BeFalse();
		}
		
		[Test]
		public void GiveControl_ActorHasEnoughEnergyAndDoesNotHaveFreshFieldOfView_FieldOfViewIsRecalculated()
		{
			var tileVisibilityUpdaterMock = new Mock<ITileVisibilityUpdater>();
			var actorController = new ActorController(Mock.Of<IAiActionResolver>(), Mock.Of<IPlayerActionResolver>(),
				tileVisibilityUpdaterMock.Object, Mock.Of<INeedHandler>());
			ActorData actorData = new ActorData
			{
				ControlledByPlayer = true,
				Energy = 1,
				HasFreshFieldOfView = false,
			};

			actorController.GiveControl(actorData);

			tileVisibilityUpdaterMock.Verify(c => c.UpdateTileVisibility(It.IsAny<Vector2Int>(), It.IsAny<int>()));
		}
		
		[Test]
		public void GiveControl_ActorHasEnoughEnergyAndHasFreshFieldOfView_FieldOfViewIsNotRecalculated()
		{
			var tileVisibilityUpdaterMock = new Mock<ITileVisibilityUpdater>();
			var actorController = new ActorController(Mock.Of<IAiActionResolver>(), Mock.Of<IPlayerActionResolver>(),
				tileVisibilityUpdaterMock.Object, Mock.Of<INeedHandler>());
			ActorData actorData = new ActorData
			{
				ControlledByPlayer = true,
				Energy = 1,
				HasFreshFieldOfView = true
			};

			actorController.GiveControl(actorData);

			tileVisibilityUpdaterMock.Verify(c => c.UpdateTileVisibility(It.IsAny<Vector2Int>(), It.IsAny<int>()), Times.Never);
		}

		[Test]
		public void GiveControl_ActorHasEnoughEnergyAndActionIsResolved_EnergyIsIncreasedByGain()
		{
			float initialEnergy = 1.0f;
			float energyGain = .1f;
			float actionEnergyCost = .5f;
			float expectedEnergy = .6f;
			var actorData = new ActorData { ControlledByPlayer = false, Energy = initialEnergy, EnergyGain = energyGain };
			IAiActionResolver resolver = Mock.Of<IAiActionResolver>(r => r.GetAction(It.IsAny<ActorData>()) 
				== new TestGameAction(actorData, actionEnergyCost));
			var actorController = new ActorController(resolver, It.IsAny<PlayerActionResolver>(),
				It.IsAny<ITileVisibilityUpdater>(), Mock.Of<INeedHandler>());
			
			actorController.GiveControl(actorData);

			actorData.Energy.Should().Be(expectedEnergy);
		}

		[Test]
		public void GiveControl_ActorHasEnoughEnergyAndActionIsResolved_FieldOfViewIsMarkedAsNotFresh()
		{
			float initialEnergy = 1.0f;
			float energyGain = .1f;
			float actionEnergyCost = .5f;
			var actorData = new ActorData { ControlledByPlayer = false, Energy = initialEnergy, EnergyGain = energyGain };
			IAiActionResolver resolver = Mock.Of<IAiActionResolver>(r => r.GetAction(It.IsAny<ActorData>()) 
				== new TestGameAction(actorData, actionEnergyCost));
			var actorController = new ActorController(resolver, It.IsAny<PlayerActionResolver>(),
				It.IsAny<ITileVisibilityUpdater>(), Mock.Of<INeedHandler>());
			
			actorController.GiveControl(actorData);

			actorData.HasFreshFieldOfView.Should().Be(false);
		}

		[Test]
		public void GiveControl_ActorHasEnoughEnergyAndActionIsResolved_ActionIsExecuted()
		{
			float initialEnergy = 1.0f;
			float energyGain = .1f;
			float actionEnergyCost = .5f;
			var actorData = new ActorData { ControlledByPlayer = false, Energy = initialEnergy, EnergyGain = energyGain };
			var actionToExecuteMock = new Mock<GameAction>(actorData, actionEnergyCost, Mock.Of<IActionEffectFactory>());
			IAiActionResolver resolver = Mock.Of<IAiActionResolver>(r => r.GetAction(It.IsAny<ActorData>()) 
				== actionToExecuteMock.Object);
			var actorController = new ActorController(resolver, It.IsAny<PlayerActionResolver>(), It.IsAny<ITileVisibilityUpdater>(), Mock.Of<INeedHandler>());
			
			actorController.GiveControl(actorData);

			actionToExecuteMock.Verify(e => e.Execute());
		}

		[Test]
		public void GiveControl_ActorHasEnoughEnergyAndActionIsResolved_ActionEffectIsProcessed()
		{
			float initialEnergy = 1.0f;
			float energyGain = .1f;
			var actorData = new ActorData { ControlledByPlayer = false, Energy = initialEnergy, EnergyGain = energyGain };
			var effectToProcessMock = new Mock<IActionEffect>();
			var actionToExecute = Mock.Of<IGameAction>(a => a.Execute() == new[]{effectToProcessMock.Object});
			IAiActionResolver resolver = Mock.Of<IAiActionResolver>(r => r.GetAction(It.IsAny<ActorData>()) 
				== actionToExecute);
			var actorController = new ActorController(resolver, It.IsAny<PlayerActionResolver>(),
				It.IsAny<ITileVisibilityUpdater>(), Mock.Of<INeedHandler>());
			
			actorController.GiveControl(actorData);

			effectToProcessMock.Verify(p => p.Process());
		}

		[Test]
		public void GiveControl_ActorHasEnoughEnergyAndActionIsResolved_PassesControl()
		{
			IAiActionResolver resolver 
				= Mock.Of<IAiActionResolver>(r => r.GetAction(It.IsAny<ActorData>()) == new TestGameAction(null, 0f));
			ActorData actorData = new ActorData {Energy = 1};
			var actorController = new ActorController(resolver, It.IsAny<PlayerActionResolver>(),
				It.IsAny<ITileVisibilityUpdater>(), Mock.Of<INeedHandler>());

			bool result = actorController.GiveControl(actorData);

			result.Should().BeTrue();
		}

		[Test]
		public void GiveControl_ActorHasEnoughEnergyAndActionHasNotBeenResolved_EnergyIsNotIncreasedByGain()
		{
			float initialEnergy = 1.0f;
			float energyGain = .1f;
			float expectedEnergy = initialEnergy;
			var actorData = new ActorData { Energy = initialEnergy, EnergyGain = energyGain };
			IAiActionResolver resolver = Mock.Of<IAiActionResolver>();
			var actorController = new ActorController(resolver, It.IsAny<PlayerActionResolver>(),
				It.IsAny<ITileVisibilityUpdater>(), Mock.Of<INeedHandler>());

			actorController.GiveControl(actorData);

			actorData.Energy.Should().Be(expectedEnergy);
		}

		[Test]
		public void GiveControl_AiShouldBeUsed_AiActionResolverIsUsed()
		{
			var aiActionResolver = new Mock<IAiActionResolver>();
			var playerActionResolver = new Mock<IPlayerActionResolver>();
			var actorController = new ActorController(aiActionResolver.Object, playerActionResolver.Object,
				It.IsAny<ITileVisibilityUpdater>(), Mock.Of<INeedHandler>());
			var aiActorData = Mock.Of<ActorData>(d => d.ControlledByPlayer == false && d.Energy == 1f);

			actorController.GiveControl(aiActorData);

			aiActionResolver.Verify(r => r.GetAction(It.IsAny<ActorData>()));
			playerActionResolver.Verify(r => r.GetAction(It.IsAny<ActorData>()), Times.Never);
		}

		[Test]
		public void GiveControl_AiShouldNotBeUsed_PlayerActionResolverIsUsed()
		{
			var aiActionResolver = new Mock<IAiActionResolver>();
			var playerActionResolver = new Mock<IPlayerActionResolver>();
			var actorController = new ActorController(aiActionResolver.Object, playerActionResolver.Object,
				Mock.Of<ITileVisibilityUpdater>(), Mock.Of<INeedHandler>());
			ActorData playerActorData = new ActorData
			{
				ControlledByPlayer = true,
				Energy = 1
			};

			actorController.GiveControl(playerActorData);

			aiActionResolver.Verify(r => r.GetAction(It.IsAny<ActorData>()), Times.Never);
			playerActionResolver.Verify(r => r.GetAction(It.IsAny<ActorData>()));
		}
	}
}

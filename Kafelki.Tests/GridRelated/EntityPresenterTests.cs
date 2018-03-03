using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GridRelated;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Kafelki.Tests.GridRelated
{
	[TestFixture]
	public class EntityPresenterTests
	{
		[Test]
		public void GetVisibleEntities_NoPotentiallyVisibleEntities_ReturnsEmptyCollection()
		{
			var presenter = new EntityPresenter();

			IEnumerable<IGameEntity> result = presenter.GetVisibleEntities(It.IsAny<HashSet<Vector2Int>>(), Enumerable.Empty<IGameEntity>());

			result.Should().BeEmpty();
		}

		[Test]
		public void GetVisibleEntities_ThereArePotentiallyVisibleEntitiesButNoVisibleTiles_ReturnsEmptyCollection()
		{
			var presenter = new EntityPresenter();
			IGameEntity entity = Mock.Of<IGameEntity>(e => e.EntityData.LogicalPosition == new Vector2Int(3,5));

			IEnumerable<IGameEntity> result = presenter.GetVisibleEntities(new HashSet<Vector2Int>(), new []{entity});

			result.Should().BeEmpty();
		}

		[Test]
		public void GetVisibleEntities_ThereArePotentiallyVisibleEntitiesAtSomeMatchingTiles_ReturnsEntitiesOnVisibleTiles()
		{
			var presenter = new EntityPresenter();
			IGameEntity entity1 = Mock.Of<IGameEntity>(e => e.EntityData.LogicalPosition == new Vector2Int(1,1));
			IGameEntity entity2 = Mock.Of<IGameEntity>(e => e.EntityData.LogicalPosition == new Vector2Int(2,2));
			IGameEntity entity3 = Mock.Of<IGameEntity>(e => e.EntityData.LogicalPosition == new Vector2Int(3,3));
			HashSet<Vector2Int> visibleTiles = new HashSet<Vector2Int>{new Vector2Int(1,1), new Vector2Int(2,2)};

			IEnumerable<IGameEntity> result = presenter.GetVisibleEntities(visibleTiles, new []{ entity1 , entity2, entity3});

			result.Should().BeEquivalentTo(entity1, entity2);
		}

		[Test]
		public void Illuminate_ThereArePotentiallyVisibleEntitiesAndSomeAreOnVisibleTiles_EntitiesOnVisibleTilesGetShown()
		{
			var presenter = new EntityPresenter();
			IGameEntity entity1 = Mock.Of<IGameEntity>(e => e.EntityData.LogicalPosition == new Vector2Int(1, 1));
			Mock<IGameEntity> entity1Mock = Mock.Get(entity1);
			IGameEntity entity2 = Mock.Of<IGameEntity>(e => e.EntityData.LogicalPosition == new Vector2Int(2, 2));
			Mock<IGameEntity> entity2Mock = Mock.Get(entity2);
			IGameEntity entity3 = Mock.Of<IGameEntity>(e => e.EntityData.LogicalPosition == new Vector2Int(3, 3));
			Mock<IGameEntity> entity3Mock = Mock.Get(entity3);
			HashSet<Vector2Int> visibleTiles = new HashSet<Vector2Int> { new Vector2Int(1, 1), new Vector2Int(2, 2) };

			presenter.Illuminate(visibleTiles, new[] { entity1Mock.Object, entity2Mock.Object, entity3Mock.Object });

			entity1Mock.Verify(e => e.Show(), Times.Once);
			entity2Mock.Verify(e => e.Show(), Times.Once);
			entity3Mock.Verify(e => e.Show(), Times.Never);
		}

		[Test]
		public void Illuminate_SomeEntitiesGetShownWithSameIllumination_TheyGetShownOnlyOnceAndDontGetHidden()
		{
			var presenter = new EntityPresenter();
			IGameEntity entity1 = Mock.Of<IGameEntity>(e => e.EntityData.LogicalPosition == new Vector2Int(1, 1));
			Mock<IGameEntity> entity1Mock = Mock.Get(entity1);
			IGameEntity entity2 = Mock.Of<IGameEntity>(e => e.EntityData.LogicalPosition == new Vector2Int(2, 2));
			Mock<IGameEntity> entity2Mock = Mock.Get(entity2);
			IGameEntity entity3 = Mock.Of<IGameEntity>(e => e.EntityData.LogicalPosition == new Vector2Int(3, 3));
			Mock<IGameEntity> entity3Mock = Mock.Get(entity3);
			HashSet<Vector2Int> visibleTiles = new HashSet<Vector2Int> { new Vector2Int(1, 1), new Vector2Int(2, 2) };

			presenter.Illuminate(visibleTiles, new[] { entity1Mock.Object, entity2Mock.Object, entity3Mock.Object });
			presenter.Illuminate(visibleTiles, new[] { entity1Mock.Object, entity2Mock.Object, entity3Mock.Object });

			entity1Mock.Verify(e => e.Show(), Times.Once);
			entity1Mock.Verify(e => e.Hide(), Times.Never);
			
			entity2Mock.Verify(e => e.Show(), Times.Once);
			entity2Mock.Verify(e => e.Hide(), Times.Never);

			entity3Mock.Verify(e => e.Show(), Times.Never);
			entity3Mock.Verify(e => e.Hide(), Times.Never);
		}
	}
}
using System.Collections.Generic;
using Assets.Scripts.GridRelated.TilemapAffecting;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Kafelki.Tests.GridRelated.TilemapAffecting
{
	[TestFixture]
	public class TilePresenterTests
	{
		// naming explanation for upcoming tests:
		// lit tile - a tile that has been specially marked (e.g. by color) due to its visibility
		// visible tile - tile that Field of View algorithm returned

		[Test]
		public void Illuminate_NoTilesWereLitAndNoTilesBecomeVisible_NoTilesAreLitAfterwardsAndNoTilesGetAffected()
		{
			var affector = new Mock<ITilemapAffector>();
			var presenter = new TilePresenter(affector.Object) {LitPositionsSaved = new HashSet<Vector2Int>()};

			presenter.Illuminate(new HashSet<Vector2Int>());

			presenter.LitPositionsSaved.Should().BeEmpty();
			affector.Verify(a => a.SetColorOnTilemap(It.IsAny<TilemapType>(), It.IsAny<Vector2Int>(), It.IsAny<Color>()), Times.Never);
		}

		[Test]
		public void Illuminate_SomeTilesWereLitButNoTilesAreVisible_NoTilesAreLitAfterwards()
		{
			var affector = new Mock<ITilemapAffector>();
			var presenter = new TilePresenter(affector.Object) {LitPositionsSaved = new HashSet<Vector2Int>()};

			presenter.Illuminate(new HashSet<Vector2Int>());

			presenter.LitPositionsSaved.Should().BeEmpty();
		}

		[Test]
		public void Illuminate_SomeTilesWereLitButNoTilesAreVisible_PreviouslyLitTilesBecomeUnlit()
		{
			var affector = new Mock<ITilemapAffector>();
			var firstLitTile = new Vector2Int(3,3);
			var secondLitTile = new Vector2Int(4,4);
			var presenter = new TilePresenter(affector.Object) {LitPositionsSaved = new HashSet<Vector2Int> {firstLitTile, secondLitTile}};

			presenter.Illuminate(new HashSet<Vector2Int>());

			affector.Verify(a => a.SetColorOnTilemap(It.IsAny<TilemapType>(), It.IsAny<Vector2Int>(), It.IsAny<Color>()), Times.Exactly(2));
			affector.Verify(a => a.SetColorOnTilemap(It.IsAny<TilemapType>(), firstLitTile, TileColors.UnlitColor), Times.Once);
			affector.Verify(a => a.SetColorOnTilemap(It.IsAny<TilemapType>(), secondLitTile, TileColors.UnlitColor), Times.Once);
		}

		[Test]
		public void Illuminate_NoTilesWereLitAndSomeTilesBecomeVisible_VisibleTilesAreAffectedAndLitAfterwards()
		{
			var affector = new Mock<ITilemapAffector>();
			var presenter = new TilePresenter(affector.Object) {LitPositionsSaved = new HashSet<Vector2Int>()};
			var firstVisibleTile = new Vector2Int(1, 1);
			var secondVisibleTile = new Vector2Int(2, 2);
			var visibleTiles = new HashSet<Vector2Int>{ firstVisibleTile, secondVisibleTile };

			presenter.Illuminate(visibleTiles);

			presenter.LitPositionsSaved.ShouldBeEquivalentTo(visibleTiles);
			affector.Verify(a => a.SetColorOnTilemap(It.IsAny<TilemapType>(), It.IsAny<Vector2Int>(), It.IsAny<Color>()), Times.Exactly(2));
			affector.Verify(a => a.SetColorOnTilemap(It.IsAny<TilemapType>(), firstVisibleTile, TileColors.LitColor), Times.Once);
			affector.Verify(a => a.SetColorOnTilemap(It.IsAny<TilemapType>(), secondVisibleTile, TileColors.LitColor), Times.Once);
		}

		[Test]
		public void Illuminate_TwoTilesAreVisibleAndOneOfThemWasAlreadyLit_NewlyLitTileBecomesLitAndIsAffected()
		{
			var affector = new Mock<ITilemapAffector>();
			var presenter = new TilePresenter(affector.Object);
			var firstVisibleTile = new Vector2Int(1, 1);
			var secondVisibleTile = new Vector2Int(2, 2);
			var initiallyLitTiles = new HashSet<Vector2Int>{firstVisibleTile};
			var visibleTiles = new HashSet<Vector2Int>{ firstVisibleTile, secondVisibleTile };
			presenter.LitPositionsSaved = initiallyLitTiles;

			presenter.Illuminate(visibleTiles);

			presenter.LitPositionsSaved.ShouldBeEquivalentTo(visibleTiles);
			affector.Verify(a => a.SetColorOnTilemap(It.IsAny<TilemapType>(), It.IsAny<Vector2Int>(), It.IsAny<Color>()), Times.Once);
			affector.Verify(a => a.SetColorOnTilemap(It.IsAny<TilemapType>(), secondVisibleTile, TileColors.LitColor), Times.Once);
		}

		[Test]
		public void Illuminate_TwoTilesWereLitButOnlyOneIsVisible_OtherTileBecomesUnlitAndIsAffected()
		{
			var affector = new Mock<ITilemapAffector>();
			var presenter = new TilePresenter(affector.Object);
			var firstVisibleTile = new Vector2Int(1, 1);
			var secondVisibleTile = new Vector2Int(2, 2);
			var initiallyLitTiles = new HashSet<Vector2Int>{firstVisibleTile, secondVisibleTile };
			var visibleTiles = new HashSet<Vector2Int>{ secondVisibleTile };
			presenter.LitPositionsSaved = initiallyLitTiles;

			presenter.Illuminate(visibleTiles);

			presenter.LitPositionsSaved.ShouldBeEquivalentTo(visibleTiles);
			affector.Verify(a => a.SetColorOnTilemap(It.IsAny<TilemapType>(), It.IsAny<Vector2Int>(), It.IsAny<Color>()), Times.Once);
			affector.Verify(a => a.SetColorOnTilemap(It.IsAny<TilemapType>(), firstVisibleTile, TileColors.UnlitColor), Times.Once);
		}
	}
}
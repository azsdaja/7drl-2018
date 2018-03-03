namespace Assets.Scripts.GameLogic.ActionLoop.ActionEffects
{
	public class CatchEffect : IActionEffect
	{
		private readonly GameEntity _caughtActorEntity;
		private readonly GameEntity _actorDataEntity;

		public CatchEffect(GameEntity caughtActorEntity, GameEntity actorDataEntity)
		{
			_caughtActorEntity = caughtActorEntity;
			_actorDataEntity = actorDataEntity;
		}

		public void Process()
		{
			_caughtActorEntity.EntityAnimator.FallIn(_caughtActorEntity.EntityData.LogicalPosition, _actorDataEntity);
		}
	}
}
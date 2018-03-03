using Assets.Scripts.Configuration;
using UnityEngine;

namespace Assets.Scripts.GameLogic.GameCore
{
	public class GameConfig : MonoBehaviour, IGameConfig
	{
		[Header("RNG seed. 0 seeds with random value.")]
		[SerializeField] private int _rngSeed = 1;

		public NeedConfig NeedConfig;
		public ModeConfig ModeConfig;
		public SmellConfig SmellConfig;

		public int RngSeed
		{
			get { return _rngSeed; }
		}

		NeedConfig IGameConfig.NeedConfig { get { return NeedConfig; }}
		ModeConfig IGameConfig.ModeConfig { get { return ModeConfig; }}
		SmellConfig IGameConfig.SmellConfig { get { return SmellConfig; } }
	}
}
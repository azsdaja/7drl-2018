using System.Collections.Generic;
using Assets.Cinemachine.Base.Runtime.Behaviours;
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
		public ActorConfig ActorConfig;
		public DungeonConfig[] DungeonConfigs;
		public CinemachineVirtualCamera FollowPlayerCamera;
		public int[] XpForLevels;

		public int RngSeed
		{
			get { return _rngSeed; }
		}


		NeedConfig IGameConfig.NeedConfig { get { return NeedConfig; }}
		ModeConfig IGameConfig.ModeConfig { get { return ModeConfig; }}
		SmellConfig IGameConfig.SmellConfig { get { return SmellConfig; } }
		ActorConfig IGameConfig.ActorConfig { get { return ActorConfig; } }
		DungeonConfig[] IGameConfig.DungeonConfigs { get { return DungeonConfigs; } }
		CinemachineVirtualCamera IGameConfig.FollowPlayerCamera { get { return FollowPlayerCamera; } }
		int[] IGameConfig.XpForLevels { get { return XpForLevels; }}
	}
}
using System;
using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.ActionLoop.AI;
using Assets.Scripts.GameLogic.Configuration;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public class ActorData : EntityData
	{

		[SerializeField, HideInInspector] private Weapon _weapon;
		[SerializeField, HideInInspector] private int _swordsFromSkill;
		[SerializeField, HideInInspector] private ActorType _actorType;
		[SerializeField, HideInInspector] private int _swords;
		[SerializeField, HideInInspector] private int _visionRayLength;
		[SerializeField, HideInInspector] private bool _controlledByPlayer;
		[SerializeField, HideInInspector] private bool _isSmelling;
		[SerializeField, HideInInspector] private float _energy;
		[SerializeField, HideInInspector] private float _energyGain;
		[SerializeField, HideInInspector] private bool _hasFreshFieldOfView;
		[SerializeField, HideInInspector] private bool _hasFreshHeartbeat;
		[SerializeField, HideInInspector] private Team _team;
		[SerializeField, HideInInspector] private List<ItemData> _items;
		[SerializeField, HideInInspector] private NavigationData _navigationData;
		[SerializeField, HideInInspector] private AiData _aiData;
		[SerializeField, HideInInspector] private int _health;
		[SerializeField, HideInInspector] private int _maxHealth;
		[SerializeField, HideInInspector] private int _roundsCount;
		[SerializeField, HideInInspector] private float _accuracy;
		[SerializeField, HideInInspector] private bool _hasLittleSpace;
		[SerializeField, HideInInspector] private int _xpGiven;
		[SerializeField, HideInInspector] private int _xp;
		[SerializeField, HideInInspector] private int _level;
		[SerializeField, HideInInspector] private List<Trait> _traits;

		[ShowInInspector]
		public ActorType ActorType
		{
			get { return _actorType; }
			set { _actorType = value; }
		}

		[ShowInInspector]
		public bool ControlledByPlayer
		{
			get { return _controlledByPlayer; }
			set { _controlledByPlayer = value; }
		}

		[ShowInInspector]
		public float Energy
		{
			get { return _energy; }
			set { _energy = value; }
		}

		[ShowInInspector]
		public float EnergyGain
		{
			get { return _energyGain; }
			set { _energyGain = value; }
		}

		[ShowInInspector]
		public int SwordsFromSkill
		{
			get { return _swordsFromSkill; }
			set { _swordsFromSkill = value; }
		}

		[ShowInInspector]
		public int VisionRayLength
		{
			get { return _visionRayLength; } 
			set
			{
				_visionRayLength = value; 
			}
		}

		public bool HasFreshFieldOfView
		{
			get { return _hasFreshFieldOfView; }
			set { _hasFreshFieldOfView = value; }
		}

		[ShowInInspector]
		public bool HasFreshHeartbeat
		{
			get { return _hasFreshHeartbeat; }
			set { _hasFreshHeartbeat = value; }
		}

		[ShowInInspector]
		public bool IsSmelling
		{
			get { return _isSmelling; }
			set { _isSmelling = value; }
		}

		[ShowInInspector]
		public Team Team
		{
			get { return _team; }
			set { _team = value; }
		}

		[ShowInInspector]
		public List<ItemData> Items
		{
			get { return _items; }
			set { _items = value; }
		}

		public ActorData CaughtActor { get; set; }

		public NavigationData NavigationData
		{
			get { return _navigationData; }
			set { _navigationData = value; }
		}

		[ShowInInspector]
		public int Health
		{
			get { return _health; }
			set { _health = value; }
		}

		[ShowInInspector]
		public float HealthProgress
		{
			get { return (float)_health / _maxHealth; }
		}

		[ShowInInspector]
		public int MaxHealth
		{
			get { return _maxHealth; }
			set { _maxHealth = value; }
		}

		[ShowInInspector]
		public AiData AiData
		{
			get { return _aiData; }
			set { _aiData = value; }
		}

		[ShowInInspector]
		public int Swords
		{
			get { return _swords; }
			set { _swords = value; }
		}

		public DateTime BlockedUntil { get; set; }

		[ShowInInspector]
		public Weapon Weapon
		{
			get { return _weapon; }
			set { _weapon = value; }
		}

		public int RoundsCount
		{
			get { return _roundsCount; }
			set { _roundsCount = value; }
		}

		[ShowInInspector]
		public bool IsInCloseCombat { get; set; }

		[ShowInInspector]
		public int MaxSwords { get; set; }

		public IGameAction StoredAction { get; set; }

		[ShowInInspector]
		public float Accuracy
		{
			get { return _accuracy; }
			set { _accuracy = value; }
		}

		[ShowInInspector]
		public bool HasLittleSpace
		{
			get { return _hasLittleSpace; }
			set { _hasLittleSpace = value; }
		}

		[ShowInInspector]
		public int XpGiven
		{
			get { return _xpGiven; }
			set { _xpGiven = value; }
		}

		[ShowInInspector]
		public int Xp
		{
			get { return _xp; }
			set { _xp = value; }
		}

		[ShowInInspector]
		public int Level
		{
			get { return _level; }
			set { _level = value; }
		}

		[ShowInInspector]
		public List<Trait> Traits
		{
			get { return _traits; }
			set { _traits = value; }
		}
	}
}
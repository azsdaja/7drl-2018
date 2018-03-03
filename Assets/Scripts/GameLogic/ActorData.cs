﻿using System;
using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.AI;
using Assets.Scripts.GameLogic.Configuration;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public class ActorData : EntityData
	{

		[SerializeField, HideInInspector] private ActorType _actorType;
		[SerializeField, HideInInspector] private int _visionRayLength;
		[SerializeField, HideInInspector] private bool _controlledByPlayer;
		[SerializeField, HideInInspector] private bool _isSmelling;
		[SerializeField, HideInInspector] private float _energy;
		[SerializeField, HideInInspector] private float _energyGain;
		[SerializeField, HideInInspector] private bool _hasFreshFieldOfView;
		[SerializeField, HideInInspector] private Team _team;
		[SerializeField, HideInInspector] private List<ItemData> _items;
		[SerializeField, HideInInspector] private NavigationData _navigationData;
		[SerializeField, HideInInspector] private AiData _aiData;
		[SerializeField, HideInInspector] private int _health;
		[SerializeField, HideInInspector] private int _maxHealth;
		[SerializeField, HideInInspector] private int _maxDamage;

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
		public int MaxDamage
		{
			get { return _maxDamage; }
			set { _maxDamage = value; }
		}
	}
}
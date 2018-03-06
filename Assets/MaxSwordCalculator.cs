using Assets.Scripts.GameLogic;

public class MaxSwordCalculator
{
	public int Calculate(ActorData actorData)
	{
		int sum = 0;
		sum += actorData.SwordsFromSkill;
		sum += actorData.Weapon.Swords;
		if (actorData.Health < actorData.MaxHealth * .5f)
			sum -= 1;
		if (actorData.IsInCloseCombat)
		{
			sum += actorData.Weapon.CloseCombatModifier;
		}
		return sum;
	}
}
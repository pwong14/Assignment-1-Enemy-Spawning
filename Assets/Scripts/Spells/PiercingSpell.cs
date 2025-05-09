using System.Collections;
using UnityEngine;

public class PiercingSpell : ModifierSpell
{
    public PiercingSpell(SpellCaster owner, Spell inner) : base(owner, inner) { }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;

        Vector3 direction = (target - where).normalized;
        float speed = inner.GetSpeed();

        int pierceCount = 1;

        GameManager.Instance.projectileManager.CreateProjectile(
            inner.GetIcon(),
            inner is BaseSpell bs ? bs.GetTrajectory() : "straight",
            where,
            direction,
            speed,
            (hittable, impact) => OnHitPiercing(hittable, impact, ref pierceCount)
        );

        yield return new WaitForEndOfFrame();
    }

    void OnHitPiercing(Hittable other, Vector3 impact, ref int remainingPierces)
    {
        if (other.team != team)
        {
            other.Damage(new Damage(GetDamage(), Damage.Type.ARCANE));
            remainingPierces--;

            if (remainingPierces < 0)
            {
            }
        }
    }
}
using System.Collections;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    [SerializeField] private GameObject damageVFX;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DealDamageGA>();
    }
    private IEnumerator DealDamagePerformer(DealDamageGA dealDamageGA)
    {
        //Loop through each target and deal damage
        foreach (var target in dealDamageGA.Targets)
        {
            target.Damage(dealDamageGA.Amount);

            Instantiate(damageVFX, target.transform.position, Quaternion.identity); //HIT MARKERS
            yield return new WaitForSeconds(0.15f); //WAIT AFTER THE ATTACK. CHANGE LATER

            if (target.CurrentHealth <= 0)  //If no health
            {
                if (target is EnemyView enemyView)
                {
                    //Spawn KillEnemyGA
                    KillEnemyGA killEnemyGA = new(enemyView);
                    ActionSystem.Instance.AddReaction(killEnemyGA);
                }

                else 
                {
                    //Player hero view death
                    //If no more hero views trigger game over
                }
            }
        }
    }
}

using UnityEngine;

//One enemy's turn. EnemyTurnGA still means every enemy acts; this is the single enemy the action bar selected.
public class EnemyActGA : GameAction
{
    public EnemyView Enemy { get; private set; }

    public EnemyActGA(EnemyView enemy)
    {
        Enemy = enemy;
    }
}

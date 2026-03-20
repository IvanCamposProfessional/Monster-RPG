using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public MonsterDatabase monsterDatabase;
    //Variable para poder modificar el tamaño de la party
    int partySize = 10;
    public List<Monster> party;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //Inicializo la party con el partySize
        party = new List<Monster>(partySize);

        //Inicializamos toda la lista a null para que se generen las 10 posiciones de la lista vacias
        for(int i = 0; i < partySize; i++){
            party.Add(null);
        }

        //Inicializas la data de los monsters que quieras
        MonsterData ghostData = monsterDatabase.GetMonsterByID("1");
        MonsterData skeletonData = monsterDatabase.GetMonsterByID("2");
        MonsterData slimeData = monsterDatabase.GetMonsterByID("3");
        MonsterData snakeData = monsterDatabase.GetMonsterByID("4");
        MonsterData zombieData = monsterDatabase.GetMonsterByID("5");

        //Creamos el objeto Monster para modificarlo con la data y lo añadimos a la party (esto es modo guarro, habria que hacer un bucle para saber si el slot esta a null y
        //ahi añadir el monster)
        Monster ghostMonster = new Monster(ghostData, 1, 5, 10);
        party[0]= ghostMonster;
        ghostMonster.learnedMoves.Add(ghostData.LerneableMoves[0].Move);
        ghostMonster.learnedMoves.Add(ghostData.LerneableMoves[1].Move);
        party[0].enemyAI = Resources.Load<EnemyAI>("Monsters/EnemyAI/GenericEnemyAI");
        if(party[0].learnedMoves[0] != null && party[0].learnedMoves[1] != null && party[0].enemyAI != null)
        {
            Debug.Log("Enemy " + party[0].data.name + " ataques y AI cargados correctamente");
        }

        Monster skeletonMonster = new Monster(skeletonData, 1, 5, 10);
        party[1] = skeletonMonster;
        skeletonMonster.learnedMoves.Add(skeletonData.LerneableMoves[0].Move);
        skeletonMonster.learnedMoves.Add(skeletonData.LerneableMoves[1].Move);
        party[1].enemyAI = Resources.Load<EnemyAI>("Monsters/EnemyAI/GenericEnemyAI");
        if(party[1].learnedMoves[0] != null && party[1].learnedMoves[1] != null && party[1].enemyAI != null)
        {
            Debug.Log("Enemy " + party[1].data.name + " ataques y AI cargados correctamente");
        }

        Monster slimeMonster = new Monster(slimeData, 1, 5, 10);
        party[2] = slimeMonster;
        slimeMonster.learnedMoves.Add(slimeData.LerneableMoves[0].Move);
        slimeMonster.learnedMoves.Add(slimeData.LerneableMoves[1].Move);
        party[2].enemyAI = Resources.Load<EnemyAI>("Monsters/EnemyAI/GenericEnemyAI");
        if(party[2].learnedMoves[0] != null && party[2].learnedMoves[1] != null && party[2].enemyAI != null)
        {
            Debug.Log("Enemy " + party[2].data.name + " ataques y AI cargados correctamente");
        }

        Monster snakeMonster = new Monster(snakeData, 1, 5, 10);
        party[3] = snakeMonster;
        snakeMonster.learnedMoves.Add(snakeData.LerneableMoves[0].Move);
        snakeMonster.learnedMoves.Add(snakeData.LerneableMoves[1].Move);
        party[3].enemyAI = Resources.Load<EnemyAI>("Monsters/EnemyAI/GenericEnemyAI");
        if(party[3].learnedMoves[0] != null && party[3].learnedMoves[1] != null && party[3].enemyAI != null)
        {
            Debug.Log("Enemy " + party[1].data.name + " ataques y AI cargados correctamente");
        }

        Monster zombieMonster = new Monster(zombieData, 1, 5, 10);
        party[4] = zombieMonster;
        zombieMonster.learnedMoves.Add(zombieData.LerneableMoves[0].Move);
        zombieMonster.learnedMoves.Add(zombieData.LerneableMoves[1].Move);
        party[4].enemyAI = Resources.Load<EnemyAI>("Monsters/EnemyAI/GenericEnemyAI");
        if(party[4].learnedMoves[0] != null && party[4].learnedMoves[1] != null && party[4].enemyAI != null)
        {
            Debug.Log("Enemy " + party[4].data.name + " ataques y AI cargados correctamente");
        }

        /*for(int i = 0; i < party.Count; i++){
            if(party[i] == null){
                continue;
            }else{
                Debug.Log("Enemy: " + party[i].data.MonsterName);
            }
        }*/
    }
}

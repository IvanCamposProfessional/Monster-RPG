using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public MonsterDatabase monsterDatabase;
    //Variable para poder modificar el tamaño de la party
    int partySize = 10;
    public List<Monster> party;

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
        //Hardcodeamos los ataques que sabe el monster
        ghostMonster.learnedMoves.Add(ghostData.LerneableMoves[0].Move);
        ghostMonster.learnedMoves.Add(ghostData.LerneableMoves[1].Move);
        //Debug.Log(ghostMonster.learnedMoves[0].MoveName);
        //Debug.Log(ghostMonster.learnedMoves[1].MoveName);

        Monster skeletonMonster = new Monster(skeletonData, 1, 5, 10);
        party[1] = skeletonMonster;
        //Hardcodeamos los ataques que sabe el monster
        skeletonMonster.learnedMoves.Add(skeletonData.LerneableMoves[0].Move);
        skeletonMonster.learnedMoves.Add(skeletonData.LerneableMoves[1].Move);
        //Debug.Log(skeletonMonster.learnedMoves[0].MoveName);
        //Debug.Log(skeletonMonster.learnedMoves[1].MoveName);

        Monster slimeMonster = new Monster(slimeData, 1, 5, 10);
        party[2] = slimeMonster;
        //Hardcodeamos los ataques que sabe el monster
        slimeMonster.learnedMoves.Add(slimeData.LerneableMoves[0].Move);
        slimeMonster.learnedMoves.Add(slimeData.LerneableMoves[1].Move);
        //Debug.Log(slimeMonster.learnedMoves[0].MoveName);
        //Debug.Log(slimeMonster.learnedMoves[1].MoveName);

        Monster snakeMonster = new Monster(snakeData, 1, 5, 10);
        party[3] = snakeMonster;
        //Hardcodeamos los ataques que sabe el monster
        snakeMonster.learnedMoves.Add(snakeData.LerneableMoves[0].Move);
        snakeMonster.learnedMoves.Add(snakeData.LerneableMoves[1].Move);
        //Debug.Log(snakeMonster.learnedMoves[0].MoveName);
        //Debug.Log(snakeMonster.learnedMoves[1].MoveName);

        Monster zombieMonster = new Monster(zombieData, 1, 5, 10);
        party[4] = zombieMonster;
        //Hardcodeamos los ataques que sabe el monster
        zombieMonster.learnedMoves.Add(zombieData.LerneableMoves[0].Move);
        zombieMonster.learnedMoves.Add(zombieData.LerneableMoves[1].Move);
        //Debug.Log(zombieMonster.learnedMoves[0].MoveName);
        //Debug.Log(zombieMonster.learnedMoves[1].MoveName);

        /*for(int i = 0; i < party.Count; i++){
            if(party[i] == null){
                continue;
            }else{
                Debug.Log(party[i].data.MonsterName);
            }
        }*/

        /*Monster slimeMonster = new Monster(slimeData, 1, 5, 10);
        party[2] = slimeMonster;*/
    }
}

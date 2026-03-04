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
        Monster skeletonMonster = new Monster(skeletonData, 1, 5, 10);
        party[1] = skeletonMonster;
        Monster slimeMonster = new Monster(slimeData, 1, 5, 10);
        party[2] = slimeMonster;
        Monster snakeMonster = new Monster(snakeData, 1, 5, 10);
        party[3] = snakeMonster;
        Monster zombieMonster = new Monster(zombieData, 1, 5, 10);
        party[4] = zombieMonster;

        /*for(int i = 0; i < party.Count; i++){
            if(party[i] == null){
                continue;
            }else{
                Debug.Log("Enemy: " + party[i].data.MonsterName);
            }
        }*/
    }
}

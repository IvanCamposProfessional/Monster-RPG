using System.Collections.Generic;
using UnityEngine;

public static class TypeChart
{
    private static readonly Dictionary<MonsterType, Dictionary<MonsterType, float>> chart =
        new Dictionary<MonsterType, Dictionary<MonsterType, float>>()
    {
            //Atacante
        { MonsterType.Techno, new Dictionary<MonsterType, float> {
                //Defensor     //Multiplicador
            { MonsterType.Flesh,    0f  },  // No afecta
            { MonsterType.Wildwood, 0.5f },
            { MonsterType.Ancient,  0.5f },
            { MonsterType.Fae,      2f  }
        }},
        { MonsterType.Flesh, new Dictionary<MonsterType, float> {
            { MonsterType.Techno,   2f  },
            { MonsterType.Glare,    0f  },  // No afecta
            { MonsterType.Shady,    0f  },  // No afecta
            { MonsterType.Fae,      0.5f }
        }},
        { MonsterType.Glare, new Dictionary<MonsterType, float> {
            { MonsterType.Flesh,    2f  },
            { MonsterType.Shady,    2f  }
        }},
        { MonsterType.Shady, new Dictionary<MonsterType, float> {
            { MonsterType.Flesh,    2f  },
            { MonsterType.Glare,    2f  }
        }},
        { MonsterType.Depths, new Dictionary<MonsterType, float> {
            { MonsterType.Pyros,    2f  },
            { MonsterType.Wildwood, 0.5f }
        }},
        { MonsterType.Pyros, new Dictionary<MonsterType, float> {
            { MonsterType.Depths,   0.5f },
            { MonsterType.Wildwood, 2f  },
            { MonsterType.Ancient,  0.5f },
            { MonsterType.Fae,      0f  }  // No afecta
        }},
        { MonsterType.Wildwood, new Dictionary<MonsterType, float> {
            { MonsterType.Techno,   0.5f },
            { MonsterType.Depths,   2f  },
            { MonsterType.Pyros,    0.5f },
            { MonsterType.Fae,      0f  }  // No afecta
        }},
        { MonsterType.Ancient, new Dictionary<MonsterType, float> {
            { MonsterType.Techno,   0.5f },
            { MonsterType.Flesh,    2f  },
            { MonsterType.Pyros,    0.5f },
            { MonsterType.Fae,      2f  }
        }},
        { MonsterType.Fae, new Dictionary<MonsterType, float> {
            { MonsterType.Techno,   0f  },  // No afecta
            { MonsterType.Depths,   0.5f }
        }},
    };

    //Funcion para devolver el multiplicador de tipo atacante vs defensor
    //Si no está em ña tabla devuelve 1 (neutral)
    public static float GetMultiplier(MonsterType attackType, MonsterType defenderType)
    {
        //Recorre el diccionario buscando en el tipo del atacante si aparece la entrada del tipo del defensor
        if(chart.TryGetValue(attackType, out var defenderDict))
        {
            //Busca el tipo defensor en la entrada del diccionario anidado y devuelve el multiplier
            if(defenderDict.TryGetValue(defenderType, out float multiplier))
            {
                return multiplier;
            }
        }

        //Si no encuentra el tipo del defensor dentro del tipo del atacante devuelve 1 (neutro)
        return 1f;
    }
}

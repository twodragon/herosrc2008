using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    public static class MobSpawnerController
    {
        public static int MOB_SPAWN_RANGE = 10;

        public static void SpawnMob(Vector3 _position, ushort _skinID, int _spawnSeconds, int _spawnCount, int _mapId)
        {
            _spawnSeconds *= 1000; // sec to ms

            MobSpawner mobSpawner = new MobSpawner();

            mobSpawner.position = _position; // 167, 182, 0

            mobSpawner.skinId = _skinID; // 40101

            mobSpawner.timeToSpawn = _spawnSeconds; // 5000

            mobSpawner.maxSpawn = _spawnCount; // 3

            mobSpawner.mapId = _mapId;
        }
    }
}

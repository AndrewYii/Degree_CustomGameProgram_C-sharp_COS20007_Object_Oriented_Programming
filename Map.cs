using System;
using System.Collections.Generic;

namespace DistinctionTask{
    /// <summary>
    /// This is the Map class that represents a game map with tiles, monsters, and various features.
    /// </summary>
    public class Map
    {
        private Tile[,] _tiles;
        private int _stageLevel;
        private int _mapWidth;
        private int _mapLength;
        private Random _random;
        private List<Monster> _monsters;
        /// <summary>
        /// Parameterized constructor to initialize a map with specified width, length, and stage level.
        /// </summary>
        public Map(int mapWidth, int mapLength, int stageLevel)
        {
            _mapWidth = mapWidth;
            _mapLength = mapLength;
            _stageLevel = stageLevel;
            _random = new Random();
            _tiles = new Tile[mapWidth, mapLength];
            _monsters = new List<Monster>();
            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < _mapLength; y++)
                {
                    _tiles[x, y] = new Tile(x, y, 100, 100, GenerateRandomTileType(x, y), true, false, false);
                }
            }
        }
        /// <summary>
        /// Generates a random tile type based on the position and stage level.
        /// </summary>
        private TileType GenerateRandomTileType(int col, int row)
        {
            if (col == 0 || row == 0 || col == _mapWidth - 1 || row == _mapLength - 1)
            {
                return TileType.Wall;
            }
            if (col == 1 && row == 1)
            {
                return TileType.Spawn; 
            }
            if (col == _mapWidth - 2 && row == _mapLength - 2)
            {
                return TileType.Exit; 
            }
            int randomValue = _random.Next(100);
            int monsterChance = 5 + (_stageLevel * 2); 
            int trapChance = 3 + (_stageLevel); 
            int treasureChance = 5; 
            int merchantChance = 2; 
            int blacksmithChance = 2; 
            int randomBuffChance = 4; 
            int wallChance = 20 - (_stageLevel);
            if (randomValue < monsterChance)
            {
                return TileType.Monster;
            }
            else if (randomValue < monsterChance + trapChance)
            {
                return TileType.Trap;
            }
            else if (randomValue < monsterChance + trapChance + treasureChance)
            {
                return TileType.Treasure;
            }
            else if (randomValue < monsterChance + trapChance + treasureChance + merchantChance)
            {
                return TileType.Merchant;
            }
            else if (randomValue < monsterChance + trapChance + treasureChance + merchantChance + blacksmithChance)
            {
                return TileType.Blacksmith;
            }
            else if (randomValue < monsterChance + trapChance + treasureChance + merchantChance + blacksmithChance + randomBuffChance)
            {
                return TileType.RandomBuff;
            }
            else if (randomValue < monsterChance + trapChance + treasureChance + merchantChance + blacksmithChance + randomBuffChance + wallChance)
            {
                return TileType.Wall;
            }
            else
            {
                return TileType.Empty;
            }
        }
        /// <summary>
        /// Checks if there is a valid path from the spawn point to the exit using BFS algorithm.
        /// </summary>
        public bool CheckPathAvailable()
        {
            bool[,] visited = new bool[_mapWidth, _mapLength];
            Queue<(int, int)> queue = new Queue<(int, int)>();
            int startX = 1;
            int startY = 1;
            int exitX = _mapWidth - 2;
            int exitY = _mapLength - 2;
            queue.Enqueue((startX, startY));
            visited[startX, startY] = true;
            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { -1, 0, 1, 0 };
            while (queue.Count > 0)
            {
                (int x, int y) = queue.Dequeue();
                if (x == exitX && y == exitY)
                {
                    return true;
                }
                for (int i = 0; i < 4; i++)
                    {
                        int newX = x + dx[i];
                        int newY = y + dy[i];
                        if (CheckInBounds(newX, newY) && !visited[newX, newY] && _tiles[newX, newY].IsWalkable)
                        {
                            visited[newX, newY] = true;
                            queue.Enqueue((newX, newY));
                        }
                    }
            }
            return false;
        }
        /// <summary>
        /// Checks if the given coordinates are within the bounds of the map.
        /// </summary>
        public bool CheckInBounds(int x, int y)
        {
            return x >= 0 && x < _mapWidth && y >= 0 && y < _mapLength;
        }
        /// <summary>
        /// Generates the map with random tile types and ensures a valid path exists.
        /// </summary>
        public void GenerateMap()
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < _mapLength; y++)
                {
                    TileType tileType = GenerateRandomTileType(x, y);
                    _tiles[x, y].TileType = tileType;
                    switch (tileType)
                    {
                        case TileType.Wall:
                            _tiles[x, y].IsWalkable = false;
                            break;
                        default:
                            _tiles[x, y].IsWalkable = true;
                            break;
                    }
                    _tiles[x, y].IsVisible = (x == 1 && y == 1) ||(x == 1 && y == 2) ||(x == 2 && y == 1) ||(x == 2 && y == 2);
                }
            }
            int attempts = 0;
            const int maxAttempts = 10;
            while (!CheckPathAvailable() && attempts < maxAttempts)
            {
                for (int x = 1; x < _mapWidth - 1; x++)
                {
                    for (int y = 1; y < _mapLength - 1; y++)
                    {
                        if ((x == 1 && y == 1) || (x == _mapWidth - 2 && y == _mapLength - 2)){
                            continue;
                        }
                        TileType tileType = GenerateRandomTileType(x, y);
                        _tiles[x, y].TileType = tileType;
                        _tiles[x, y].IsWalkable = (tileType != TileType.Wall);
                    }
                }
                attempts++;
            }
            if (attempts >= maxAttempts)
            {
                for (int x = 1; x < _mapWidth - 1; x++)
                {
                    for (int y = 1; y < _mapLength - 1; y++)
                    {
                        if (_tiles[x, y].TileType == TileType.Wall)
                        {
                            _tiles[x, y].TileType = TileType.Empty;
                            _tiles[x, y].IsWalkable = true;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Adds monsters to the map based on the stage level and difficulty.
        /// </summary>
        public void AddMonsters(int count, Difficulty difficulty, List<Unit> gameUnits)
        {
            Random random = new Random();
            int monsterCount = count;
            bool includeBoss = false;
            List<Monster> stageMonsters = Monster.GenerateStageMonsters(_stageLevel, difficulty, monsterCount, includeBoss);
            foreach (Monster monster in stageMonsters)
            {
                int x, y;
                do
                {
                    x = random.Next(2, _mapWidth - 2);
                    y = random.Next(2, _mapLength - 2);
                } while (_tiles[x, y].TileType != TileType.Empty);
                monster.Row = y;
                monster.Column = x;
                _monsters.Add(monster);
                gameUnits.Add(monster);
                _tiles[x, y].TileType = TileType.Monster;
            }
            if (_stageLevel == 3)
            {                int exitX = _mapWidth - 2;
                int exitY = _mapLength - 2;
                int bossLevel = _stageLevel + 2;
                Monster? monster = MonsterFactory.CreateMonsterFromSelection("BOSS_PHASE1", bossLevel, "", difficulty, exitY, exitX);
                if (monster != null && monster is Boss boss)
                {
                    Boss defaultBoss = new Boss("Phase 1 Boss", monster.HP, monster.MaxHP, monster.Damage, monster.CriticalRate, monster.Defense, monster.Speed, monster.Mana, monster.MoneyReward, bossLevel, exitY, exitX, true, 100);
                    _monsters.Add(defaultBoss);
                    gameUnits.Add(defaultBoss);
                }
                _tiles[exitX, exitY].TileType = TileType.Monster;

            }
        }
        /// <summary>
        /// Counts the number of monster tiles in the map.
        /// </summary>
        public int CountMonsterTiles()
        {
            int count = 0;
            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < _mapLength; y++)
                {
                    if (_tiles[x, y].TileType == TileType.Monster)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        /// <summary>
        /// Gets the tiles in the map.
        /// </summary>
        public Tile[,] Tiles
        {
            get { return _tiles; }
        }
        /// <summary>
        /// Gets the map width.
        /// </summary>
        public int MapWidth
        {
            get { return _mapWidth; }
        }
        /// <summary>
        /// Gets the map length.
        /// </summary>
        public int MapLength
        {
            get { return _mapLength; }
        }
    }
}
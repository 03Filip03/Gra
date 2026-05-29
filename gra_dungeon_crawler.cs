using System;
using System.Collections.Generic;

class Item
{
    public string name;
    public string itemType;
    public int value;

    public Item(string name, string itemType, int value)
    {
        this.name = name;
        this.itemType = itemType;
        this.value = value;
    }

    public override string ToString()
    {
        return $"{name} ({itemType}, wartosc: {value})";
    }
}

class Hero
{
    public string name;
    public int hp;
    public int maxHp;
    public int attack;
    public List<Item> inventory = new List<Item>();

    public Hero(string name)
    {
        this.name = name;
        hp = 100;
        maxHp = 100;
        attack = 10;
    }

    public void PickUp(Item item)
    {
        inventory.Add(item);
        if (item.itemType == "weapon")
        {
            attack += item.value;
            Console.WriteLine($"{name} podniosl {item.name}, atak: {attack}");
        }
        else if (item.itemType == "potion")
        {
            hp = Math.Min(hp + item.value, maxHp);
            Console.WriteLine($"{name} uzyl {item.name}, HP: {hp}/{maxHp}");
        }
    }

    public bool IsAlive()
    {
        return hp > 0;
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp < 0) hp = 0;
    }

    public override string ToString()
    {
        return $"{name} | HP: {hp}/{maxHp} | Atak: {attack}";
    }
}

class Monster
{
    public string name;
    public int hp;
    public int attack;

    public Monster(string name, int hp, int attack)
    {
        this.name = name;
        this.hp = hp;
        this.attack = attack;
    }

    public bool IsAlive()
    {
        return hp > 0;
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp < 0) hp = 0;
    }

    public override string ToString()
    {
        return $"{name} | HP: {hp} | Atak: {attack}";
    }
}

class Room
{
    public int roomId;
    public Monster monster;
    public Item item;

    public Room(int roomId)
    {
        this.roomId = roomId;
    }

    public override string ToString()
    {
        string opis = $"Pokoj {roomId}: ";
        if (monster != null && monster.IsAlive())
            opis += $"jest potworr - {monster}";
        else if (item != null)
            opis += $"jest przedmiot - {item}";
        else
            opis += "pusty";
        return opis;
    }
}

class Dungeon
{
    public List<Room> rooms = new List<Room>();
    public int current = 0;
    Random rng = new Random();

    public Dungeon()
    {
        Generate();
    }

    void Generate()
    {
        Monster[] allMonsters = {
            new Monster("Goblin", 20, 5),
            new Monster("Szkielet", 30, 8),
            new Monster("Troll", 50, 12),
            new Monster("Smok", 80, 20)
        };
        Item[] allItems = {
            new Item("Miecz", "weapon", 5),
            new Item("Topor", "weapon", 8),
            new Item("Mikstura", "potion", 20),
            new Item("Duza mikstura", "potion", 40)
        };

        for (int i = 1; i <= 6; i++)
        {
            Room room = new Room(i);
            double r = rng.NextDouble();
            if (r < 0.4)
            {
                Monster m = allMonsters[rng.Next(allMonsters.Length)];
                room.monster = new Monster(m.name, m.hp, m.attack);
            }
            else if (r < 0.65)
            {
                Item it = allItems[rng.Next(allItems.Length)];
                room.item = new Item(it.name, it.itemType, it.value);
            }
            rooms.Add(room);
        }
    }

    public Room GetCurrentRoom()
    {
        return rooms[current];
    }

    public bool NextRoom()
    {
        if (current + 1 < rooms.Count)
        {
            current++;
            return true;
        }
        return false;
    }

    public bool IsLastRoom()
    {
        return current == rooms.Count - 1;
    }
}

class Program
{
    static void Fight(Hero hero, Monster monster)
    {
        Console.WriteLine($"\nWalka: {hero.name} vs {monster.name}");
        while (hero.IsAlive() && monster.IsAlive())
        {
            monster.TakeDamage(hero.attack);
            Console.WriteLine($"{hero.name} atakuje, {monster.name} ma teraz {monster.hp} HP");
            if (!monster.IsAlive())
            {
                Console.WriteLine($"{monster.name} pokonany!");
                break;
            }
            hero.TakeDamage(monster.attack);
            Console.WriteLine($"{monster.name} atakuje, {hero.name} ma teraz {hero.hp} HP");
            if (!hero.IsAlive())
            {
                Console.WriteLine($"{hero.name} zginal...");
            }
        }
    }

    static void Main(string[] args)
    {
        Console.WriteLine("=== Dungeon Crawler ===\n");
        Console.Write("Podaj imie bohatera: ");
        string name = Console.ReadLine();
        Hero hero = new Hero(name);
        Dungeon dungeon = new Dungeon();

        while (true)
        {
            Room room = dungeon.GetCurrentRoom();
            Console.WriteLine($"\n{room}");

            if (room.monster != null && room.monster.IsAlive())
            {
                Fight(hero, room.monster);
                if (!hero.IsAlive())
                {
                    Console.WriteLine("\nGame over!");
                    break;
                }
            }

            if (room.item != null)
            {
                hero.PickUp(room.item);
                room.item = null;
            }

            Console.WriteLine($"\nStan: {hero}");

            if (dungeon.IsLastRoom())
            {
                Console.WriteLine("\nGratulacje! Przeszles caly loch!");
                break;
            }

            Console.Write("\nIsc dalej? (t/n): ");
            string choice = Console.ReadLine();
            if (choice != "t") break;
            dungeon.NextRoom();
        }
    }
}

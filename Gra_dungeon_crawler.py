import random

class Item:
    def __init__(self, name, item_type, value):
        self.name = name
        self.item_type = item_type
        self.value = value

    def __str__(self):
        return f"{self.name} ({self.item_type}, wartosc: {self.value})"


class Hero:
    def __init__(self, name):
        self.name = name
        self.hp = 100
        self.max_hp = 100
        self.attack = 10
        self.inventory = []

    def pick_up(self, item):
        self.inventory.append(item)
        if item.item_type == "weapon":
            self.attack += item.value
            print(f"{self.name} podniosl {item.name}, atak: {self.attack}")
        elif item.item_type == "potion":
            self.hp = min(self.hp + item.value, self.max_hp)
            print(f"{self.name} uzyл {item.name}, HP: {self.hp}/{self.max_hp}")

    def is_alive(self):
        return self.hp > 0

    def take_damage(self, dmg):
        self.hp -= dmg
        if self.hp < 0:
            self.hp = 0

    def __str__(self):
        return f"{self.name} | HP: {self.hp}/{self.max_hp} | Atak: {self.attack}"


class Monster:
    def __init__(self, name, hp, attack):
        self.name = name
        self.hp = hp
        self.attack = attack

    def is_alive(self):
        return self.hp > 0

    def take_damage(self, dmg):
        self.hp -= dmg
        if self.hp < 0:
            self.hp = 0

    def __str__(self):
        return f"{self.name} | HP: {self.hp} | Atak: {self.attack}"


class Room:
    def __init__(self, room_id):
        self.room_id = room_id
        self.monster = None
        self.item = None

    def __str__(self):
        opis = f"Pokoj {self.room_id}: "
        if self.monster and self.monster.is_alive():
            opis += f"jest potworr - {self.monster}"
        elif self.item:
            opis += f"jest przedmiot - {self.item}"
        else:
            opis += "pusty"
        return opis


class Dungeon:
    def __init__(self):
        self.rooms = []
        self.current = 0
        self._generate()

    def _generate(self):
        all_monsters = [
            Monster("Goblin", 20, 5),
            Monster("Szkielet", 30, 8),
            Monster("Troll", 50, 12),
            Monster("Smok", 80, 20)
        ]
        all_items = [
            Item("Miecz", "weapon", 5),
            Item("Topor", "weapon", 8),
            Item("Mikstura", "potion", 20),
            Item("Duza mikstura", "potion", 40)
        ]

        for i in range(1, 7):
            room = Room(i)
            r = random.random()
            if r < 0.4:
                m = random.choice(all_monsters)
                room.monster = Monster(m.name, m.hp, m.attack)
            elif r < 0.65:
                it = random.choice(all_items)
                room.item = Item(it.name, it.item_type, it.value)
            self.rooms.append(room)

    def get_current_room(self):
        return self.rooms[self.current]

    def next_room(self):
        if self.current + 1 < len(self.rooms):
            self.current += 1
            return True
        return False

    def is_last_room(self):
        return self.current == len(self.rooms) - 1


def fight(hero, monster):
    print(f"\nWalka: {hero.name} vs {monster.name}")
    while hero.is_alive() and monster.is_alive():
        monster.take_damage(hero.attack)
        print(f"{hero.name} atakuje, {monster.name} ma teraz {monster.hp} HP")
        if not monster.is_alive():
            print(f"{monster.name} pokonany!")
            break
        hero.take_damage(monster.attack)
        print(f"{monster.name} atakuje, {hero.name} ma teraz {hero.hp} HP")
        if not hero.is_alive():
            print(f"{hero.name} zginal...")


def main():
    print("=== Dungeon Crawler ===\n")
    name = input("Podaj imie bohatera: ")
    hero = Hero(name)
    dungeon = Dungeon()

    while True:
        room = dungeon.get_current_room()
        print(f"\n{room}")

        if room.monster and room.monster.is_alive():
            fight(hero, room.monster)
            if not hero.is_alive():
                print("\nGame over!")
                break

        if room.item:
            hero.pick_up(room.item)
            room.item = None

        print(f"\nStan: {hero}")

        if dungeon.is_last_room():
            print("\nGratulacje! Przeszles caly loch!")
            break

        dalej = input("\nIsc dalej? (t/n): ")
        if dalej != "t":
            break
        dungeon.next_room()


main()

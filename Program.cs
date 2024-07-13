using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace GladiatorFights
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Arena arena = new Arena();
            arena.Work();
        }
    }

    class Arena
    {
        public void Work()
        {
            const string CommandStart = "1";
            const string CommandExit = "exit";

            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine($"Введите {CommandStart}, чтобы начать игру.\n" +
                    $"Введите {CommandExit}, чтобы выйти.");
                string userInput = Console.ReadLine();

                Console.Clear();

                switch (userInput)
                {
                    case CommandStart:
                        Play();
                        break;

                    case CommandExit:
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("Такой команды нет.");
                        break;
                }
            }

            Console.Clear();
            Console.WriteLine("Программа завершена.");
        }

        private void Play()
        {
            int fightersAmount = 2;

            Fighter[] fighters = new Fighter[fightersAmount];

            for (int i = 0; i < fighters.Length; i++)
            {
                List<Fighter> availableFighters = CreateFighters();

                for (int j = 0; j < availableFighters.Count; j++)
                {
                    Console.Write($"Введите {j}, чтобы выбрать:");
                    availableFighters[j].ShowStats();
                }

                fighters[i] = availableFighters[ReadIndex(availableFighters.Count)];
            }

            Console.Clear();
            Console.WriteLine("Вы выбрали следующих бойцов:");

            foreach (Fighter fighter in fighters)
                fighter.ShowStats();

            Console.WriteLine("\nНажмите любую клавишу, чтобы начать бой.");
            Console.ReadLine();

            Fight(fighters);
        }

        private int ReadIndex(int arrayLength)
        {
            int index;
            int firstIndex = 0;
            int lastIndex = arrayLength - 1;

            do
            {
                Console.WriteLine($"Введите индекс от {firstIndex} до {lastIndex}.");
                index = ReadInt();
            }
            while (index < firstIndex || index > lastIndex);

            return index;
        }

        private int ReadInt()
        {
            int number;

            while (int.TryParse(Console.ReadLine(), out number) == false)
                Console.WriteLine("Это не число.");

            return number;
        }

        private void Fight(Fighter[] fighters)
        {
            Fighter firstFighter = fighters[0];
            Fighter secondFighter = fighters[1];

            int numberRound = 0;

            while (firstFighter.Health > 0 && secondFighter.Health > 0)
            {
                numberRound++;

                Console.WriteLine($"\nРаунд {numberRound}.\n");

                Console.ForegroundColor = ConsoleColor.Green;
                firstFighter.Attack(secondFighter);

                Console.ForegroundColor = ConsoleColor.Red;
                secondFighter.Attack(firstFighter);

                Console.ResetColor();
            }

            Console.WriteLine($"\nБой закончился в {numberRound} раунде.\n");

            if (firstFighter.Health > 0 && secondFighter.Health < 0)
            {
                Console.WriteLine("\nБой окончен, победил первый боец:");
                firstFighter.ShowStats();
            }
            else if (firstFighter.Health < 0 && secondFighter.Health > 0)
            {
                Console.WriteLine("\nБой окончен, победил второй боец:");
                secondFighter.ShowStats();
            }
            else
            {
                Console.WriteLine("\nБоевая ничья, оба бойца погибли.");
            }

            Console.ReadLine();
        }

        private List<Fighter> CreateFighters()
        {
            CreateFighterStats(out int health, out int damage);

            List<Fighter> fighters = new List<Fighter>();

            fighters.Add(new CriticalMaster(GetName(), health, damage));
            fighters.Add(new DoubleDamageMaster(GetName(), health, damage));
            fighters.Add(new Barbarian(GetName(), health, damage));
            fighters.Add(new Wizard(GetName(), health, damage));
            fighters.Add(new Rogue(GetName(), health, damage));

            return fighters;
        }

        private void CreateFighterStats(out int health, out int damage)
        {
            int minLimitHealth = 75;
            int maxLimitHealth = 101;

            int minLimitDamage = 10;
            int maxLimitDamage = 15;

            health = UserUtils.GenerateRandomNumber(minLimitHealth, maxLimitHealth);
            damage = UserUtils.GenerateRandomNumber(minLimitDamage, maxLimitDamage);
        }

        private string GetName()
        {
            List<string> names = new List<string>() { "Джон Сина", "Маркус", "Безымянный", "Цареубийца", "Подгорелый", "Впиши любое имя всё равно никто читать не будет", "Бывалый", "Маслёнок", "Вася" };

            return names[UserUtils.GenerateRandomNumber(0, names.Count - 1)];
        }
    }

    class Fighter
    {
        protected string Name;
        protected int Damage;

        public Fighter(string name, int health, int damage)
        {
            Name = name;
            Health = health;
            Damage = damage;
        }

        public int Health { get; protected set; }

        public virtual void Attack(Fighter fighter)
        {
            ShowAttackMessage();
            fighter.TakeDamage(Damage);
        }

        public void ShowAttackMessage()
        {
            Console.WriteLine($"{Name} наносит {Damage} урона.");
        }

        public virtual void TakeDamage(int damage)
        {
            Health -= damage;
            ShowTakeDamageMessage(damage);
        }

        public virtual void ShowStats()
        {
            Console.WriteLine($"Боец {Name} имеет {Health} здоровья и наносит {Damage} урона.");
        }

        private void ShowTakeDamageMessage(int damage)
        {
            Console.WriteLine($"{Name} получил {damage} урона и у него осталось {Health} здоровья.");
        }
    }

    class CriticalMaster : Fighter
    {
        private string _nameClass = "CriticalMaster";

        private int _criticalChancePercent = 25;
        private int _maxDamageBooster = 2;
        private int _standartDamageBooster = 1;

        public CriticalMaster(string name, int health, int damage) : base(name, health, damage) { }

        public override void Attack(Fighter fighter)
        {
            ShowAttackMessage();
            fighter.TakeDamage(Damage * IncreaseDamage());
        }

        public override void ShowStats()
        {
            Console.WriteLine(_nameClass);
            base.ShowStats();
        }

        private int IncreaseDamage()
        {
            int minLimitPercent = 0;
            int maxLimitPercent = 100;

            if (UserUtils.GenerateRandomNumber(minLimitPercent, maxLimitPercent) <= _criticalChancePercent)
            {
                Console.WriteLine("Крит!");
                return _maxDamageBooster;
            }
            else
            {
                return _standartDamageBooster;
            }
        }
    }

    class DoubleDamageMaster : Fighter
    {
        private string _nameClass = "DoubleDamageMaster";

        private int _maxDamageBooster = 2;
        private int _attackCounter = 1;
        private int _attackCounterDoubleDamage = 3;

        public DoubleDamageMaster(string name, int health, int damage) : base(name, health, damage) { }

        public override void Attack(Fighter fighter)
        {
            if (TryDoubleDamage())
            {
                ShowAttackMessage();
                Console.WriteLine("Двойной урон!");
                fighter.TakeDamage(Damage * _maxDamageBooster);
            }
            else
            {
                base.Attack(fighter);
            }
        }

        public override void ShowStats()
        {
            Console.WriteLine(_nameClass);
            base.ShowStats();
        }

        private bool TryDoubleDamage()
        {
            if (_attackCounterDoubleDamage == _attackCounter)
            {
                _attackCounter = 1;
                Console.WriteLine("Третья атака!");
                return true;
            }
            else
            {
                _attackCounter++;
                return false;
            }
        }
    }

    class Barbarian : Fighter
    {
        private string _nameClass = "Barbarian";

        private int _rage;
        private int _maxRage = 30;
        private int _heal = 15;

        public Barbarian(string name, int health, int damage) : base(name, health, damage) { }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);

            if (TryHeal(damage))
            {
                Health += _heal;
                Console.WriteLine($"Хилимся на {_heal}, теперь у нас {Health} здоровья.");
            }
        }

        public override void ShowStats()
        {
            Console.WriteLine(_nameClass);
            base.ShowStats();
        }

        private bool TryHeal(int damage)
        {
            if (_rage >= _maxRage)
            {
                _rage = 0;
                Console.WriteLine("Я зол!");
                return true;
            }
            else
            {
                _rage += damage;
                return false;
            }
        }
    }

    class Wizard : Fighter
    {
        private string _nameClass = "Wizard";

        private int _mana = 100;
        private int _fireBallCost = 25;
        private int _fireBallDamageBonus = 2;

        public Wizard(string name, int health, int damage) : base(name, health, damage) { }

        public override void Attack(Fighter fighter)
        {
            if (_mana >= _fireBallCost)
            {
                ShowAttackMessage();
                Console.WriteLine("Мана ещё есть лови FireBall!");
                fighter.TakeDamage(CastFireBall());
            }
            else
            {
                Console.WriteLine("Мана кончилась, в рукопашную!");
                base.Attack(fighter);
            }
        }

        public override void ShowStats()
        {
            Console.WriteLine(_nameClass);
            base.ShowStats();
        }

        private int CastFireBall()
        {
            _mana -= _fireBallCost;
            return Damage * _fireBallDamageBonus;
        }
    }

    class Rogue : Fighter
    {
        private string _nameClass = "Rogue";
        private int _dodgeChancePercent = 25;

        public Rogue(string name, int health, int damage) : base(name, health, damage) { }

        public override void TakeDamage(int damage)
        {
            if (TryDodge())
                Console.WriteLine($"Промахнулся! У меня всё ещё {Health}.");
            else
                base.TakeDamage(damage);
        }

        public override void ShowStats()
        {
            Console.WriteLine(_nameClass);
            base.ShowStats();
        }

        private bool TryDodge()
        {
            int minLimitPercent = 0;
            int maxLimitPercent = 100;

            if (UserUtils.GenerateRandomNumber(minLimitPercent, maxLimitPercent) <= _dodgeChancePercent)
            {
                Console.WriteLine("Сработал уворот!");
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int min, int max)
        {
            return s_random.Next(min, max);
        }
    }
}

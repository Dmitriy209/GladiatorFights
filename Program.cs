using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Xml.Linq;
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
                        Start();
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

        private void Start()
        {
            const string CriticalMaster = "1";
            const string DoubleDamageMaster = "2";
            const string Barbarian = "3";
            const string Wizard = "4";
            const string Rogue = "5";

            int fightersAmount = 2;

            Fighter[] fighters = new Fighter[fightersAmount];

            for (int i = 0; i < fighters.Length; i++)
            {
                Console.WriteLine($"Выберите бойца:\n" +
                    $"{CriticalMaster} - класс CriticalMaster.\n" +
                    $"{DoubleDamageMaster} - класс DoubleDamageMaster.\n" +
                    $"{Barbarian} - класс Barbarian.\n" +
                    $"{Wizard} - класс Wizard.\n" +
                    $"{Rogue} - класс Rogue.");
                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case CriticalMaster:
                        fighters[i] = CreateCriticalMaster();
                        break;

                    case DoubleDamageMaster:
                        fighters[i] = CreateDoubleDamageMaster();
                        break;

                    case Barbarian:
                        fighters[i] = CreateBarbarian();
                        break;

                    case Wizard:
                        fighters[i] = CreateWizard();
                        break;

                    case Rogue:
                        fighters[i] = CreateRogue();
                        break;

                    default:
                        Console.WriteLine("Такого бойца нет.");
                        i--;
                        break;
                }
            }

            Console.Clear();
            Console.WriteLine("Вы выбрали следующих бойцов:");

            foreach (Fighter fighter in fighters)
                fighter.ShowStats();

            Console.WriteLine("\nНажмите любую клавишу, чтобы начать бой.");
            Console.ReadLine();

            Fight(fighters);
        }

        private void Fight(Fighter[] fighters)
        {
            var firstFighter = fighters[0];
            var secondFighter = fighters[1];

            int numberRound = 0;

            while (firstFighter.Health > 0 && secondFighter.Health > 0)
            {
                numberRound++;

                Console.WriteLine($"\nРаунд {numberRound}.\n");

                Console.ForegroundColor = ConsoleColor.Green;
                firstFighter.TakeDamage(secondFighter.Attack());

                Console.ForegroundColor = ConsoleColor.Red;
                secondFighter.TakeDamage(firstFighter.Attack());

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

        private CriticalMaster CreateCriticalMaster()
        {
            CreateFighterStats(out int health, out int damage);

            CriticalMaster criticalMaster = new CriticalMaster(GetName(), health, damage);

            criticalMaster.ShowStats();

            return criticalMaster;
        }

        private DoubleDamageMaster CreateDoubleDamageMaster()
        {
            CreateFighterStats(out int health, out int damage);

            DoubleDamageMaster doubleDamageMaster = new DoubleDamageMaster(GetName(), health, damage);

            doubleDamageMaster.ShowStats();

            return doubleDamageMaster;
        }

        private Barbarian CreateBarbarian()
        {
            CreateFighterStats(out int health, out int damage);

            Barbarian barbarian = new Barbarian(GetName(), health, damage);

            barbarian.ShowStats();

            return barbarian;
        }

        private Wizard CreateWizard()
        {
            CreateFighterStats(out int health, out int damage);

            Wizard wizard = new Wizard(GetName(), health, damage);

            wizard.ShowStats();

            return wizard;
        }

        private Rogue CreateRogue()
        {
            CreateFighterStats(out int health, out int damage);

            Rogue Rogue = new Rogue(GetName(), health, damage);

            Rogue.ShowStats();

            return Rogue;
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

        public virtual int Attack()
        {
            ShowAttackMessage();
            return Damage;
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

        public void ShowTakeDamageMessage(int damage)
        {
            Console.WriteLine($"{Name} получил {damage} урона и у него осталось {Health} здоровья.");
        }

        public virtual void ShowStats()
        {
            Console.WriteLine($"Боец {Name} имеет {Health} здоровья и наносит {Damage} урона.");
        }
    }

    class CriticalMaster : Fighter
    {
        private string _nameClass = "CriticalMaster";

        private int _criticalChancePercent = 25;
        private int _maxDamageBooster = 2;
        private int _standartDamageBooster = 1;

        public CriticalMaster(string name, int health, int damage) : base(name, health, damage)
        {

        }

        public override int Attack()
        {
            ShowAttackMessage();
            return Damage * DamageBooster();
        }

        private int DamageBooster()
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

        public override void ShowStats()
        {
            Console.WriteLine(_nameClass);
            base.ShowStats();
        }
    }

    class DoubleDamageMaster : Fighter
    {
        private string _doubleDamageMaster = "DoubleDamageMaster";

        private int _maxDamageBooster = 2;
        private int _attackCounter = 1;
        private int _attackCounterDoubleDamage = 3;

        public DoubleDamageMaster(string name, int health, int damage) : base(name, health, damage)
        {

        }

        public override int Attack()
        {
            ShowAttackMessage();

            if (TryDoubleDamage())
            {
                Console.WriteLine("Двойной урон!");
                return Damage * _maxDamageBooster;
            }
            else
            {
                return base.Attack();
            }
        }

        public bool TryDoubleDamage()
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

        public override void ShowStats()
        {
            Console.WriteLine(_doubleDamageMaster);
            base.ShowStats();
        }
    }

    class Barbarian : Fighter
    {
        private string _barbarian = "Barbarian";

        private int _rage;
        private int _maxRage = 30;
        private int _heal = 15;

        public Barbarian(string name, int health, int damage) : base(name, health, damage)
        {

        }

        public override void TakeDamage(int damage)
        {
            Health -= damage;
            Console.WriteLine($"{Name} получил {damage} урона и у него осталось {Health} здоровья.");

            if (TryHeal(damage))
            {
                Health += _heal;
                Console.WriteLine($"Хилимся на {_heal}, теперь у нас {Health} здоровья.");
            }
        }

        public bool TryHeal(int damage)
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

        public override void ShowStats()
        {
            Console.WriteLine(_barbarian);
            base.ShowStats();
        }
    }

    class Wizard : Fighter
    {
        private string _wizard = "Wizard";

        private int _mana = 100;
        private int _fireBallCost = 25;
        private int _fireBallDamageBonus = 2;


        public Wizard(string name, int health, int damage) : base(name, health, damage)
        {

        }

        public override int Attack()
        {
            ShowAttackMessage();

            if (_mana >= _fireBallCost)
            {
                Console.WriteLine("Мана ещё есть лови FireBall!");
                return CastFireBall();
            }
            else
            {
                Console.WriteLine("Мана кончилась в рукопашную!");
                return base.Attack();
            }
        }

        private int CastFireBall()
        {
            _mana -= _fireBallCost;
            return Damage * _fireBallDamageBonus;
        }

        public override void ShowStats()
        {
            Console.WriteLine(_wizard);
            base.ShowStats();
        }
    }

    class Rogue : Fighter
    {
        private string _rogue = "Rogue";
        private int _dodgeChancePercent = 25;

        public Rogue(string name, int health, int damage) : base(name, health, damage)
        {

        }

        public override void TakeDamage(int damage)
        {
            if (TryDodge())
                Console.WriteLine($"Промахнулся! У меня всё ещё {Health}.");
            else
                Health -= damage;
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

        public override void ShowStats()
        {
            Console.WriteLine(_rogue);
            base.ShowStats();
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

using System;

namespace VideogameStudio
{
    public abstract class Employee : IWorker
    {
        private string name;
        public string Name
        {
            get => name;
            private set
            {
                if (!char.IsUpper(value[0]))
                {
                    throw new ArgumentException("Имя сотрудника должно начинаться с большой буквы.");
                }
                name = value;
            }
        }

        private int age;
        public int Age
        {
            get => age;
            private set
            {
                if (value < 20 || value > 80)
                {
                    throw new ArgumentException("Принимаем в студию только сотрудников в возрасте от 20 до 80 лет.");
                }
                age = value;
            }
        }

        private int workExpierence;
        public int WorkExperience
        {
            get => workExpierence;
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Опыт работы нанимаемого сотрудника не может быть отрицательным.");
                }
                workExpierence = value;
            }
        }

        private int workedMonths = 0;
        public int WorkedMonths
        {
            get => workedMonths;
            private set
            {
                workedMonths = value;
                if (workedMonths % 12 == 0)
                {
                    ImproveSkills();
                }
            }
        }

        public int Productivity { get; protected set; }

        public decimal Salary { get; protected set; }

        public abstract Specilization Speciality { get; }

        public HappinessLevel Happiness { get; private set; } = HappinessLevel.Happy;

        public Employee(string name, int age, int workExperience)
        {
            Name = name;
            Age = age;
            WorkExperience = workExperience;
            if (age - workExpierence < 18)
            {
                throw new ArgumentException("Опыт работы нанимаемого сотрудника не соответствует возрасту (считается только опыт работы с 18 лет).");
            }
        }

        public virtual void ImproveSkills()
        {
            WorkExperience++;
            Productivity += 20;
            Salary += 10000;
        }

        public void SimulateMonthWorking(decimal salary)
        {
            WorkedMonths++;
            if (Salary > salary)
            {
                Happiness = HappinessLevel.Unhappy;
            }
            if (Happiness == HappinessLevel.Unhappy && Salary <= salary)
            {
                Happiness = HappinessLevel.Happy;
            }
        }
    }
}

namespace VideogameStudio
{
    public sealed class Developer : Employee
    {
        public Developer(string name, int age, int workExperience)
            : base(name, age, workExperience)
        {
            Productivity = 10 + (workExperience * 10);
            Salary = 50000 + (workExperience * 20000);
        }

        public override Specilization Speciality => Specilization.Developer;

        public override void ImproveSkills()
        {
            Productivity += 10;
            Salary += 20000;
        }
    }
}

namespace VideogameStudio
{
    public sealed class SoftwareTester: Employee
    {
        public SoftwareTester(string name, int age, int workExperience)
            : base(name, age, workExperience)
        {
            Productivity = 20 + (workExperience * 20);
            Salary = 30000 + (workExperience * 20000);
        }

        public override Specilization Speciality => Specilization.SoftwareTester;
    }
}

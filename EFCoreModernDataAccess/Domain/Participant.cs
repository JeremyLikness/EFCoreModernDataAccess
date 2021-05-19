namespace EFModernDA.Domain
{
    public abstract class Participant
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name => $"{LastName}.{FirstName}";

        public override string ToString() => Name;
    }
}

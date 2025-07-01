namespace SmartContractVehicle.DTO
{
    public class UserTO
    {
        public string Id { get; set; } = string.Empty;

        public string? UserName { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }

        public bool IsRenter { get; set; }

        public bool IsLessor { get; set; }
    }
}

namespace SmartContractVehicle.DTO
{
    public class UserProfileUpdateTO
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? CurrentPassword { get; set; }

        public string? NewPassword { get; set; }

        public string? ConfirmNewPassword { get; set; }
    }
}

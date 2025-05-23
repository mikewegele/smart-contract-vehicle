namespace User.Domain
{
    public class NewUser
    {
        public string Email { get; }
        public string Password { get; }
        public string Name { get; }

        public NewUser(string email, string password, string name)
        {
            Email = email;
            Password = password;
            Name = name;
        }
    }
}

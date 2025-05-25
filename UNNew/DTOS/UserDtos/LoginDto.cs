namespace UNNew.DTOS.UserDTO
{
    public class LoginDto
    {
        public string UserName { get; set; }

        public string Password { get; set; }
        public int RoleId { get; set; }

        public string? Email { get; set; }

    }
}

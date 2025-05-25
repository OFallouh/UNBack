namespace UNNew.DTOS.UserDtos
{
    public class AddUserDto
    {
        public string UserName { get; set; }

        public string Password { get; set; }
        public int RoleId { get; set; }
        public string Permissions { get; set; }
        public string? Email { get; set; }
    }
}

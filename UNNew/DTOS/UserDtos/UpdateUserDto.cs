namespace UNNew.DTOS.UserDTO
{
    public class UpdateUserDto
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public int RoleId { get; set; }
        public string? Email { get; set; }
        public string Permissions { get; set; }
    }
}

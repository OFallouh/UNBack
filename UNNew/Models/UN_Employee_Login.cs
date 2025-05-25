namespace UNNew.Models
{
    public class UN_Employee_Login
    {
        public int Id { get; set; }
    
        public string Password { get; set; }

        public int EmployeeId { get; set; }

        public virtual UnEmp Employee { get; set; } // العلاقة مع جدول UN_Emp

    }
}

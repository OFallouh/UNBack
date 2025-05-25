using UNNew.Models;

namespace UNNew.DTOS.LaptopDto
{
    public class GetAllGetLaptopTypeDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public Activite? Activite { get; set; }
    }
}

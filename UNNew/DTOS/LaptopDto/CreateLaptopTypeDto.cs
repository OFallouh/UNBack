using UNNew.Models;

namespace UNNew.DTOS.LaptopDto
{
    public class CreateLaptopTypeDto
    {
        public string Name { get; set; }
        public Activite? Activite { get; set; }
    }
}

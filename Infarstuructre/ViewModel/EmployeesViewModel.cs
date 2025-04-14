using Domin.Entity;
using Domin.Resource;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infarstuructre.ViewModel
{
    
    
       
        public class EmployeesViewModel
        {
            public NewEmployees NewEmployees { get; set; } = new NewEmployees();
            public List<Employees> Employees { get; set; } = new List<Employees>();
            public List<Service> services { get; set; } = new List<Service>(); // Initialize here
        }

    

    public class NewEmployees
    {
            public int Id { get; set; }

            [Required(ErrorMessage = "Name is required.")]
            [MaxLength(20, ErrorMessage = "Name cannot exceed 20 characters.")]
            [MinLength(3, ErrorMessage = "Name must be at least 3 characters.")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string Email { get; set; }
        public int servisId { get; set; }

        public string TitleResume { get; set; }
            public bool IsActive { get; set; }

            [Required(ErrorMessage = "Phone number is required.")]
            public string Phone { get; set; }

            [Required(ErrorMessage = "Date is required.")]
            public DateTime Date { get; set; }


    }
    }

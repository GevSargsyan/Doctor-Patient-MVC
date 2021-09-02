using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entityentitiy.ViewModels
{
    public class RegisterView
    {

        [Required(ErrorMessage = "Նշեք Մուտքանունը")]
        [RegularExpression(@"^.{5,}$", ErrorMessage = "Մուտքանունը պետք է լինի 5 սիմվոլից ավել")]

        public string Login { get; set; }

        [Required(ErrorMessage = "Նշեք Գաղտնաբառը")]
        [RegularExpression(@"^.{5,}$", ErrorMessage = "Գաղտնաբառը պետք է լինի 5 սիմվոլից ավել")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Նշեք Տարիքը")]
        public int  Age{ get; set; }

       
   
        public bool  IsDoctor{ get; set; }
        [Required(ErrorMessage = "Նշեք Մասնագիտությունը")]
        public string Speciality { get; set; }



        [Required(ErrorMessage = "Նշեք Անունը")]
        public string FirstName { get; set; }



        [Required(ErrorMessage = "Նշեք Ազգանունը")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Նշեք Հայրանունը")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Նշեք Մեյլը")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",
         ErrorMessage = "Նշեք ճիշտ մեյլ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Նշեք Հեռախոսահամարը")]
        [RegularExpression(@"\d{9}",
         ErrorMessage = "Նշեք ճիշտ հեռախոսահամար")]
        public string PhoneNumber { get; set; }




        public IFormFile file { get; set; }
        public byte[] Avatar { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;
using Sport.Service.Strings;

namespace Sport.Service.ViewModels
{
    public class PhoneConfirmBindingModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Register_UserNotFound")]
        public string UserId { get; set; }
        public string NewPhoneNumber { get; set; }
        public string Code { get; set; }
    }
}
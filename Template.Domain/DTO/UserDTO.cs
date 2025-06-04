using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Template.Infrastructure.Models;

namespace Template.Domain.DTO
{
    public class UserRequest : UserDTO
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }

        [Obsolete("Inaccessible hidden inherited variable", true)]
        public new string? Id { get; set; }

        [Obsolete("Inaccessible hidden inherited variable", true)]
        public new int? OrderNumber { get; set; }

        [Obsolete("Inaccessible hidden inherited variable", true)]
        public new bool? IsDeleted { get; set; }

        [Obsolete("Inaccessible hidden inherited variable", true)]
        public new DateTime? CreatedDate { get; set; }

        [Obsolete("Inaccessible hidden inherited variable", true)]
        public new string? CreatedBy { get; set; }

        [Obsolete("Inaccessible hidden inherited variable", true)]
        public new DateTime? UpdatedDate { get; set; }

        [Obsolete("Inaccessible hidden inherited variable", true)]
        public new string? UpdatedBy { get; set; }

        [Obsolete("Inaccessible hidden inherited variable", true)]
        public new DateTime? DeletedDate { get; set; }

        [Obsolete("Inaccessible hidden inherited variable", true)]
        public new string? DeletedBy { get; set; }
    }

    public class UserDTO
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Firstname not empty.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Lastname is required.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [Phone(ErrorMessage = "Invalid Phone.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email.")]
        public string? Email { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public int OrderNumber { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTime? UpdatedDate { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string? UpdatedBy { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTime? DeletedDate { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string? DeletedBy { get; set; }

        public static UserDTO AddFromModel(Users model)
        {
            var result = new UserDTO();

            if (model != null)
            {
                result.Id = model.ID;
                result.FirstName = model.FirstName;
                result.LastName = model.LastName;
                result.Phone = model.Phone;
                result.Email = model.Email;
                result.OrderNumber = model.OrderNumber;
                result.IsDeleted = model.IsDeleted;
                result.CreatedDate = model.CreatedDate;
                result.CreatedBy = model.CreatedBy;
                result.UpdatedDate = model.UpdatedDate;
                result.UpdatedBy = model.UpdatedBy;
                result.DeletedDate = model.DeletedDate;
                result.DeletedBy = model.DeletedBy;
            }

            return result;
        }

        public void AddToModel(Users model)
        {
            model.ID = Guid.NewGuid().ToString();
            model.FirstName = FirstName;
            model.LastName = LastName;
            model.Phone = Phone;
            model.Email = Email;
        }

        public void UpdateToModel(Users model)
        {
            model.FirstName = FirstName;
            model.LastName = LastName;
            model.Phone = Phone;
            model.Email = Email;
            model.UpdatedDate = DateTime.Now;
            model.UpdatedBy = "System";
        }

        public void DeleteToModel(Users model)
        {
            model.IsDeleted = true;
            model.DeletedDate = DateTime.Now;
            model.DeletedBy = "System";
        }
    }
}
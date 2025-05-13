using Template.Infrastructure.Models;

namespace Template.Domain.DTO
{
    public class UserDTO
    {
        public string? ID { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }

        public static UserDTO? CreateFromModel(Users model)
        {
            if (model != null)
            {
                var result = new UserDTO()
                {
                    ID = model.ID,
                    Username = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Phone = model.Phone,
                    Email = model.Email,
                    IsDeleted = model.IsDeleted,
                    CreatedDate = model.CreatedDate,
                    CreatedBy = model.CreatedBy,
                    UpdatedDate = model.UpdatedDate,
                    UpdatedBy = model.UpdatedBy
                };

                return result;
            }
            else
            {
                return null;
            }
        }


        public async Task CreateSaveToModelAsync(Users model)
        {
            model.ID = Guid.NewGuid().ToString();
            model.Username = Username;
            model.FirstName = FirstName;
            model.LastName = LastName;
            model.Phone = Phone;
            model.Email = Email;
            model.IsDeleted = false;
            model.CreatedDate = DateTime.Now;
            model.CreatedBy = "System";
        }

        public async Task UpdateSaveToModelAsync(Users model)
        {
            model.FirstName = FirstName;
            model.LastName = LastName;
            model.Phone = Phone;
            model.Email = Email;
            model.UpdatedDate = DateTime.Now;
            model.UpdatedBy = "System";
        }

        public async Task DeleteSaveToModelAsync(Users model)
        {
            model.IsDeleted = true;
            model.UpdatedDate = DateTime.Now;
            model.UpdatedBy = "System";
        }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Template.Infrastructure.Models;

namespace Template.Domain.DTO
{
    public class LogoutRequest
    {
        public string? RefreshToken { get; set; }
    }

    public class LoginCacheDTO : LoginDTO
    {
        public string? Id { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public bool isHave { get; set; }    
    }

    public class LoginRequest : LoginDTO
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }

        [Obsolete("Inaccessible hidden inherited variable", true)]
        public new string? Token { get; set; }

        [Obsolete("Inaccessible hidden inherited variable", true)]
        public new int? Expires { get; set; }

        [Obsolete("Inaccessible hidden inherited variable", true)]
        public new DateTime? CreatedDate { get; set; }
    }

    public class LoginDTO
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public GrantTypeStatus GrantType { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Required(ErrorMessage = "Email not empty.")]
        public string? Email { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string? RefreshToken { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string? Token { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public double Expires { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTime? CreatedDate { get; set; }

        public static LoginDTO CreateFromModel(Tokens model)
        {
            var result = new LoginDTO();

            if (model != null)
            {
                result.Token = model.Token;
                result.RefreshToken = model.RefreshToken;
                result.Email = model.Email;
                result.Expires = model.Expires;
                result.Email = model.Email;
                result.CreatedDate = model.CreatedDate;
            }

            return result;
        }

        public void AddToModel(Tokens model, int expiryMinutes)
        {
            model.Token = Token;
            model.Expires = expiryMinutes;
            model.RefreshToken = RefreshToken;
            model.Email = Email;
            model.ExpiredDate = DateTime.Now.AddMinutes(expiryMinutes);
        }
    }

    public enum GrantTypeStatus
    {
        PASSWORD = 1,
        REFRESH_TOKEN = 2
    }
}
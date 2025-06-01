using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Template.Infrastructure.Models;

namespace Template.Domain.DTO
{
    public class MessageDTO
    {
        [Required(ErrorMessage = "Email not empty.")]
        public string? ID { get; set; }

        [Required(ErrorMessage = "User id not empty.")]
        public string? UserID { get; set; }

        [Required(ErrorMessage = "Topic not empty.")]
        public string? Topic { get; set; }

        [Required(ErrorMessage = "Detail not empty.")]
        public string? Detail { get; set; }

        public static MessageDTO CreateFromModel(Messages model)
        {
            var result = new MessageDTO();

            if (model != null)
            {
                result.ID = model.ID;
                result.UserID = model.UserID;
                result.Topic = model.Topic;
                result.Detail = model.Detail;
            }

            return result;
        }

        public void AddToModel(Messages model)
        {
            model.ID = Guid.NewGuid().ToString();
            model.UserID = UserID;
            model.Topic = Topic;
            model.Detail = Detail;
        }
    }
}
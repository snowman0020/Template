using Template.Domain.DTO;

namespace Template.Helper.Line
{
    public interface ILine
    {
        Task<LineDTO> SendMessageAsync(LineRequest input);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Errors.Model;
using Template.Domain.DTO;
using Template.Domain.Filter;
using Template.Domain.Paging;
using Template.Domain.SortBy;
using Template.Infrastructure;
using Template.Infrastructure.Models;
using Template.Service.IServices;

namespace Template.Service.Services
{
    public class UserService : IUserService
    {
        private readonly TemplateDbContext _db;
        private readonly ILogger<UserService> _logger;

        public UserService(TemplateDbContext db, ILogger<UserService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<PageList<UserDTO>> GetUserListAsync(UserFilter? filter, PageParam pageParam, UserSortBy? sortBy)
        {
            try
            {
                var query = _db.Users.AsNoTracking();

                #region Filter
                if (filter != null)
                {
                    if (!string.IsNullOrEmpty(filter.FullName))
                    {
                        string fullName = filter.FullName;
                        query = query.Where(f => f.FirstName == fullName || f.LastName == fullName);
                    }
                    if (!string.IsNullOrEmpty(filter.Phone))
                    {
                        string phone = filter.Phone;
                        query = query.Where(f => f.Phone == phone);
                    }
                    if (!string.IsNullOrEmpty(filter.Email))
                    {
                        string email = filter.Email;
                        query = query.Where(f => f.Email == email);
                    }
                }
                #endregion

                #region SortBy
                if (sortBy != null && sortBy.UserEnumSortBy != null)
                {
                    switch (sortBy.UserEnumSortBy.Value)
                    {
                        case UserEnumSortBy.FullName:
                            if (sortBy.Ascending)
                            {
                                query = query.OrderBy(o => o.FirstName).OrderBy(o => o.LastName);
                            }
                            else
                            {
                                query = query.OrderByDescending(o => o.FirstName).OrderByDescending(o => o.LastName);
                            }
                            break;
                        case UserEnumSortBy.Phone:
                            if (sortBy.Ascending)
                            {
                                query = query.OrderBy(o => o.Phone);
                            }
                            else
                            {
                                query = query.OrderByDescending(o => o.Phone);
                            }
                            break;
                        case UserEnumSortBy.Email:
                            if (sortBy.Ascending)
                            {
                                query = query.OrderBy(o => o.Email);
                            }
                            else
                            {
                                query = query.OrderByDescending(o => o.Email);
                            }
                            break;
                    }
                }
                else
                {
                    query = query.OrderBy(o => o.OrderNumber);
                }
                #endregion

                #region Paging
                var queryResult = await PageList<Users>.ToModelList(query, pageParam);

                var userListDTO = queryResult.Select(qr => UserDTO.CreateFromModel(qr)).ToList();

                var result = PageList<UserDTO>.ToPagedList(userListDTO, pageParam);
                #endregion

                return result;
            }
            catch (Exception ex)
            {
                string message = $"Can not get user list => message: {ex.Message.ToString()}";

                _logger.LogError(message);
                throw new BadRequestException(message);
            }
        }
        public async Task<UserDTO> GetUserByIdAsync(string Id)
        {
            var result = new UserDTO();

            try
            {
                var model = await _db.Users.Where(m => m.ID == Id).AsNoTracking().FirstOrDefaultAsync();

                if (model == null)
                {
                    throw new BadRequestException("Data not found.");
                }
                else
                {
                    if (model.IsDeleted)
                    {
                        throw new BadRequestException("Data has deleted.");
                    }
                }

                result = UserDTO.CreateFromModel(model);
            }
            catch (Exception ex)
            {
                string message = $"Can not search by id => message: {ex.Message.ToString()}";

                _logger.LogError(message);
                throw new BadRequestException(message);
            }

            return result;
        }
        public async Task<UserDTO> AddUserAsync(UserDTO input)
        {
            var result = new UserDTO();

            var model = await _db.Users.Where(m => m.FirstName == input.FirstName && m.LastName == input.LastName).AsNoTracking().FirstOrDefaultAsync();

            if (model != null)
            {
                throw new BadRequestException("Firstname and Lastname duplicate.");
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var user = new Users();

                    input.AddToModel(user);

                    await _db.Users.AddAsync(user);
                    await _db.SaveChangesAsync();

                    string userId = user.ID ?? "";

                    result = await GetUserByIdAsync(userId);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    string message = $"Can not add data => message: {ex.Message.ToString()}";

                    _logger.LogError(message);
                    throw new BadRequestException(message);
                }
            }

            return result;
        }
        public async Task<UserDTO> UpdateUserAsync(string Id, UserDTO input)
        {
            var result = new UserDTO();

            if (string.IsNullOrEmpty(Id))
            {
                throw new BadRequestException("Id is required.");
            }

            var model = await _db.Users.Where(m => m.FirstName == input.FirstName && m.LastName == input.LastName).AsNoTracking().FirstOrDefaultAsync();

            if (model != null && model.ID != Id)
            {
                throw new BadRequestException("Firstname and Lastname duplicate.");
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var user = await _db.Users.Where(m => m.ID == Id).FirstOrDefaultAsync();

                    if (user != null)
                    {
                        input.UpdateToModel(user);

                        _db.Users.Update(user);
                        await _db.SaveChangesAsync();
                    }

                    result = await GetUserByIdAsync(Id);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    string message = $"Can not update data => message: {ex.Message.ToString()}";

                    _logger.LogError(message);
                    throw new BadRequestException(message);
                }
            }

            return result;
        }
        public async Task DeleteUserAsync(string Id)
        {
            var model = await _db.Users.Where(m => m.ID == Id && m.IsDeleted == false).FirstOrDefaultAsync();

            if (model == null)
            {
                throw new BadRequestException("Data not found.");
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var input = new UserDTO();

                    input.DeleteToModel(model);

                    _db.Users.Update(model);
                    await _db.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    string message = $"Can not delete data => message: {ex.Message.ToString()}";

                    _logger.LogError(message);
                    throw new BadRequestException(message);
                }
            }
        }
    }
}
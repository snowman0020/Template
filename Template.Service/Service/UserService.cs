using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Template.Domain.DTO;
using Template.Domain.Filter;
using Template.Domain.Paging;
using Template.Domain.SortBy;
using Template.Helper;
using Template.Infrastructure;
using Template.Infrastructure.Models;
using Template.Service.IServices;

namespace Template.Service.Services
{
    public class UserService : IUserService
    {
        private readonly TemplateDbContext _db;
        private readonly ILogger<UserService> _logger;

        public UserService(TemplateDbContext db, ILogger<UserService> logger, IErrorExceptionHandler customExceptionHandler)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<PageList<UserDTO>> GetUserListAsync(UserFilter? filter, PageParam pageParam, UserSortBy? sortBy)
        {
            try
            {
                _logger.LogInformation($"call: GetUserListAsync: filter: {JsonSerializer.Serialize(filter)}, pageParam: {JsonSerializer.Serialize(pageParam)}, sortBy: {JsonSerializer.Serialize(sortBy)}");

                var query = _db.Users.AsNoTracking();

                #region Filter
                if (filter != null)
                {
                    if (!string.IsNullOrEmpty(filter.FullName))
                    {
                        _logger.LogInformation("filter: FullName");
                        string fullName = filter.FullName;
                        query = query.Where(f => f.FirstName == fullName || f.LastName == fullName);
                    }
                    if (!string.IsNullOrEmpty(filter.Phone))
                    {
                        _logger.LogInformation("filter: Phone");
                        string phone = filter.Phone;
                        query = query.Where(f => f.Phone == phone);
                    }
                    if (!string.IsNullOrEmpty(filter.Email))
                    {
                        _logger.LogInformation("filter: email");
                        string email = filter.Email;
                        query = query.Where(f => f.Email == email);
                    }

                    _logger.LogDebug($"filter: data: {JsonSerializer.Serialize(query.ToList())}");
                }
                #endregion

                #region SortBy
                if (sortBy != null)
                {
                    switch (sortBy.UserEnumSortBy)
                    {
                        case UserEnumSortBy.FullName:
                            if (sortBy.Ascending)
                            {
                                _logger.LogInformation("sortBy: FullName => Ascending");
                                query = query.OrderBy(o => o.FirstName).OrderBy(o => o.LastName);
                            }
                            else
                            {
                                _logger.LogInformation("sortBy: FullName => Descending");
                                query = query.OrderByDescending(o => o.FirstName).OrderByDescending(o => o.LastName);
                            }
                            break;
                        case UserEnumSortBy.Phone:
                            if (sortBy.Ascending)
                            {
                                _logger.LogInformation("sortBy: Phone => Ascending");
                                query = query.OrderBy(o => o.Phone);
                            }
                            else
                            {
                                _logger.LogInformation("sortBy: Phone => Descending");
                                query = query.OrderByDescending(o => o.Phone);
                            }
                            break;
                        case UserEnumSortBy.Email:
                            if (sortBy.Ascending)
                            {
                                _logger.LogInformation("sortBy: Email => Ascending");
                                query = query.OrderBy(o => o.Email);
                            }
                            else
                            {
                                _logger.LogInformation("sortBy: Email => Descending");
                                query = query.OrderByDescending(o => o.Email);
                            }
                            break;
                    }

                    _logger.LogDebug($"sortBy: data: {JsonSerializer.Serialize(query.ToList())}");
                }
                else
                {
                    _logger.LogInformation("sortBy default: OrderNumber => Ascending");
                    query = query.OrderBy(o => o.OrderNumber);
                    _logger.LogDebug($"sortBy default: data: {JsonSerializer.Serialize(query.ToList())}");
                }
                #endregion

                #region Paging
                var queryResult = await PageList<Users>.ToModelList(query, pageParam);

                var userListDTO = queryResult.Select(qr => UserDTO.CreateFromModel(qr)).ToList();

                var result = PageList<UserDTO>.ToPagedList(userListDTO, pageParam);
                #endregion

                _logger.LogDebug($"data: {JsonSerializer.Serialize(result)}");

                return result;
            }
            catch (Exception ex)
            {
                Error.Status = ErrorStatus.BAD_REQUEST;
                Error.Title = "Can not get user list.";
                Error.Message = ex.Message.ToString();

                _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                throw;
            }
        }
        public async Task<UserDTO> GetUserByIdAsync(string Id)
        {
            var result = new UserDTO();

            try
            {
                _logger.LogInformation($"call: GetUserByIdAsync: {Id}");

                var model = await _db.Users.Where(m => m.ID == Id).AsNoTracking().FirstOrDefaultAsync();

                if (model == null)
                {
                    Error.Status = ErrorStatus.BAD_REQUEST;
                    Error.Title = "Data not found.";
                    Error.Message = "";

                    throw new ErrorException();
                }
                else
                {
                    if (model.IsDeleted)
                    {
                        Error.Status = ErrorStatus.BAD_REQUEST;
                        Error.Title = "Data has deleted.";
                        Error.Message = "";

                        throw new ErrorException();
                    }
                }

                _logger.LogDebug($"data before createFromModel: {JsonSerializer.Serialize(model)}");

                result = UserDTO.CreateFromModel(model);

                _logger.LogDebug($"data after createFromModel: {JsonSerializer.Serialize(result)}");
            }
            catch (ErrorException)
            {
                _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                throw;
            }
            catch (Exception ex)
            {
                Error.Status = ErrorStatus.BAD_REQUEST;
                Error.Title = "Can not search by id.";
                Error.Message = ex.Message.ToString();

                _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                throw;
            }

            return result;
        }
        public async Task<UserDTO> AddUserAsync(UserDTO input)
        {
            var result = new UserDTO();

            _logger.LogInformation($"call: AddUserAsync: {JsonSerializer.Serialize(input)}");

            var model = await _db.Users.Where(m => m.FirstName == input.FirstName && m.LastName == input.LastName).AsNoTracking().FirstOrDefaultAsync();

            if (model != null)
            {
                Error.Status = ErrorStatus.BAD_REQUEST;
                Error.Title = "Firstname and Lastname duplicate.";
                Error.Message = "";

                throw new ErrorException();
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

                    _logger.LogDebug($"data: {JsonSerializer.Serialize(result)}");

                    transaction.Commit();
                }
                catch (ErrorException)
                {
                    transaction.Rollback();

                    _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                    throw;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    Error.Status = ErrorStatus.BAD_REQUEST;
                    Error.Title = "Can not add data.";
                    Error.Message = ex.Message.ToString();

                    _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                    throw new ErrorException();
                }
            }

            return result;
        }
        public async Task<UserDTO> UpdateUserAsync(string Id, UserDTO input)
        {
            var result = new UserDTO();

            _logger.LogInformation($"call: UpdateUserAsync: {Id}, {JsonSerializer.Serialize(input)}");

            if (string.IsNullOrEmpty(Id))
            {
                Error.Status = ErrorStatus.BAD_REQUEST;
                Error.Title = "Id is required.";
                Error.Message = "";

                throw new ErrorException();
            }

            var model = await _db.Users.Where(m => m.FirstName == input.FirstName && m.LastName == input.LastName).AsNoTracking().FirstOrDefaultAsync();

            if (model != null && model.ID != Id)
            {
                Error.Status = ErrorStatus.BAD_REQUEST;
                Error.Title = "Firstname and Lastname duplicate.";
                Error.Message = "";

                throw new ErrorException();
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

                    _logger.LogDebug($"data: {JsonSerializer.Serialize(result)}");

                    transaction.Commit();
                }
                catch (ErrorException)
                {
                    transaction.Rollback();

                    _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                    throw;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    Error.Status = ErrorStatus.BAD_REQUEST;
                    Error.Title = "Can not update data.";
                    Error.Message = ex.Message.ToString();

                    _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                    throw new ErrorException();
                }
            }

            return result;
        }
        public async Task DeleteUserAsync(string Id)
        {
            _logger.LogInformation($"call: DeleteUserAsync: {Id}");

            var model = await _db.Users.Where(m => m.ID == Id && m.IsDeleted == false).FirstOrDefaultAsync();

            if (model == null)
            {
                Error.Status = ErrorStatus.BAD_REQUEST;
                Error.Title = "Data not found.";
                Error.Message = "";

                throw new ErrorException();
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
                catch (ErrorException)
                {
                    transaction.Rollback();

                    _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                    throw;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    Error.Status = ErrorStatus.BAD_REQUEST;
                    Error.Title = "Can not delete data.";
                    Error.Message = ex.Message.ToString();

                    _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                    throw new ErrorException();
                }
            }
        }
    }
}
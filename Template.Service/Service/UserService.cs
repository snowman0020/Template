using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Template.Domain.AppSetting;
using Template.Domain.DTO;
using Template.Domain.Filter;
using Template.Domain.Paging;
using Template.Domain.SortBy;
using Template.Helper.ErrorException;
using Template.Helper.MessagePublish;
using Template.Helper.PasswordHash;
using Template.Infrastructure;
using Template.Infrastructure.Models;
using Template.Service.IServices;
using static MassTransit.ValidationResultExtensions;

namespace Template.Service.Services
{
    public class UserService : IUserService
    {
        private readonly TemplateDbContext _db;
        private readonly ILogger<UserService> _logger;
        private readonly IPasswordHash _passwordHash;
        private readonly IMessagePublish _messagePublish;
        private readonly RabbitMQData _rabbitMQData;

        public UserService(TemplateDbContext db, ILogger<UserService> logger, IPasswordHash passwordHash, IMessagePublish messagePublish, IOptions<RabbitMQData> rabbitMQData)
        {
            _db = db;
            _logger = logger;
            _passwordHash = passwordHash;
            _messagePublish = messagePublish;
            _rabbitMQData = rabbitMQData.Value;
        }

        public async Task<PageList<UserDTO>> GetUserListAsync(UserFilter? filter, PageParam pageParam, UserSortBy? sortBy)
        {
            _logger.LogInformation($"call: GetUserListAsync=> Start");

            var userListDTO = new List<UserDTO>();

            try
            {
                _logger.LogDebug($"filter: {JsonSerializer.Serialize(filter)}, pageParam: {JsonSerializer.Serialize(pageParam)}, sortBy: {JsonSerializer.Serialize(sortBy)}");

                var queryUser = _db.Users.AsNoTracking();

                #region Filter
                if (filter != null)
                {
                    if (!string.IsNullOrEmpty(filter.FullName))
                    {
                        _logger.LogInformation("filter: FullName");
                        string fullName = filter.FullName;
                        queryUser = queryUser.Where(qu => qu.FirstName == fullName || qu.LastName == fullName);
                    }
                    if (!string.IsNullOrEmpty(filter.Phone))
                    {
                        _logger.LogInformation("filter: Phone");
                        string phone = filter.Phone;
                        queryUser = queryUser.Where(qu => qu.Phone == phone);
                    }
                    if (!string.IsNullOrEmpty(filter.Email))
                    {
                        _logger.LogInformation("filter: email");
                        string email = filter.Email;
                        queryUser = queryUser.Where(qu => qu.Email == email);
                    }

                    _logger.LogDebug($"filter: data: {JsonSerializer.Serialize(queryUser.ToList())}");
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
                                queryUser = queryUser.OrderBy(qu => qu.FirstName).OrderBy(o => o.LastName);
                            }
                            else
                            {
                                _logger.LogInformation("sortBy: FullName => Descending");
                                queryUser = queryUser.OrderByDescending(qu => qu.FirstName).OrderByDescending(o => o.LastName);
                            }
                            break;
                        case UserEnumSortBy.Phone:
                            if (sortBy.Ascending)
                            {
                                _logger.LogInformation("sortBy: Phone => Ascending");
                                queryUser = queryUser.OrderBy(qu => qu.Phone);
                            }
                            else
                            {
                                _logger.LogInformation("sortBy: Phone => Descending");
                                queryUser = queryUser.OrderByDescending(qu => qu.Phone);
                            }
                            break;
                        case UserEnumSortBy.Email:
                            if (sortBy.Ascending)
                            {
                                _logger.LogInformation("sortBy: Email => Ascending");
                                queryUser = queryUser.OrderBy(qu => qu.Email);
                            }
                            else
                            {
                                _logger.LogInformation("sortBy: Email => Descending");
                                queryUser = queryUser.OrderByDescending(qu => qu.Email);
                            }
                            break;
                    }

                    _logger.LogDebug($"sortBy: data: {JsonSerializer.Serialize(queryUser.ToList())}");
                }
                else
                {
                    _logger.LogInformation("sortBy default: OrderNumber => Ascending");

                    queryUser = queryUser.OrderBy(qu => qu.OrderNumber);

                    _logger.LogDebug($"sortBy default: data: {JsonSerializer.Serialize(queryUser.ToList())}");
                }
                #endregion

                #region Paging
                var queryUserResult = await PageList<Users>.ToModelList(queryUser, pageParam);

                userListDTO = queryUserResult.Select(qur => UserDTO.CreateFromModel(qur)).ToList();
                #endregion
            }
            catch (Exception ex)
            {
                Error.Status = ErrorStatus.BAD_REQUEST;
                Error.Title = "Can not get user list.";
                Error.Message = ex.Message.ToString();

                _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                throw;
            }

            var result = PageList<UserDTO>.ToPagedList(userListDTO, pageParam);

            _logger.LogDebug($"data: {JsonSerializer.Serialize(result)}");

            _logger.LogInformation($"call: GetUserListAsync=> Finish");

            return result;
        }

        public async Task<UserDTO> GetUserByIdAsync(string Id)
        {
            _logger.LogInformation($"call: GetUserByIdAsync=> Start");

            var result = new UserDTO();

            try
            {
                _logger.LogDebug($"User id: {Id}");

                var modelUser = await _db.Users.Where(u => u.ID == Id && u.IsDeleted == false).AsNoTracking().FirstOrDefaultAsync();

                if (modelUser == null)
                {
                    Error.Status = ErrorStatus.BAD_REQUEST;
                    Error.Title = "Data not found.";
                    Error.Message = "";

                    throw new ErrorException();
                }
                else
                {
                    if (modelUser.IsDeleted)
                    {
                        Error.Status = ErrorStatus.BAD_REQUEST;
                        Error.Title = "Data has deleted.";
                        Error.Message = "";

                        throw new ErrorException();
                    }
                }

                _logger.LogDebug($"data before createFromModel: {JsonSerializer.Serialize(modelUser)}");

                result = UserDTO.CreateFromModel(modelUser);

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

            _logger.LogInformation($"call: GetUserByIdAsync=> Finish");

            return result;
        }

        public async Task<UserDTO> AddUserAsync(UserRequest input, string userAddId)
        {
            _logger.LogInformation($"call: AddUserAsync=> Start");

            var result = new UserDTO();

            _logger.LogDebug($"User add: {JsonSerializer.Serialize(input)}, Created by: {userAddId}");

            var modelUser = await _db.Users.Where(u => u.FirstName == input.FirstName && u.LastName == input.LastName).AsNoTracking().FirstOrDefaultAsync();

            if (modelUser != null)
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

                    if (!string.IsNullOrEmpty(input.Password))
                    {
                        user.Password = _passwordHash.Encrypt(input.Password);
                    }

                    user.CreatedBy = userAddId;

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

            _logger.LogInformation($"call: AddUserAsync=> Finish");

            return result;
        }

        public async Task<UserDTO> UpdateUserAsync(string Id, UserRequest input, string userUpdateId)
        {
            _logger.LogInformation($"call: UpdateUserAsync=> Start");

            var result = new UserDTO();

            _logger.LogDebug($"User update: {JsonSerializer.Serialize(input)}, Updated by: {userUpdateId}");

            if (string.IsNullOrEmpty(Id))
            {
                Error.Status = ErrorStatus.BAD_REQUEST;
                Error.Title = "Id is required.";
                Error.Message = "";

                throw new ErrorException();
            }

            var modelCheckUser = await _db.Users.Where(u => u.FirstName == input.FirstName && u.LastName == input.LastName && u.ID != Id && u.IsDeleted == true).FirstOrDefaultAsync();

            if (modelCheckUser != null)
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
                    var modelUser = await _db.Users.Where(u => u.ID == Id && u.IsDeleted == false).FirstOrDefaultAsync();

                    if (modelUser == null)
                    {
                        Error.Status = ErrorStatus.BAD_REQUEST;
                        Error.Title = "Data not found.";
                        Error.Message = "";

                        throw new ErrorException();
                    }

                    if (modelUser == null)
                    {
                        Error.Status = ErrorStatus.BAD_REQUEST;
                        Error.Title = "Data not found.";
                        Error.Message = "";

                        throw new ErrorException();
                    }
                    else
                    {
                        if (modelUser.IsDeleted)
                        {
                            Error.Status = ErrorStatus.BAD_REQUEST;
                            Error.Title = "Data has deleted.";
                            Error.Message = "";

                            throw new ErrorException();
                        }
                    }

                    input.UpdateToModel(modelUser);

                    if (!string.IsNullOrEmpty(input.Password))
                    {
                        modelUser.Password = _passwordHash.Encrypt(input.Password);
                    }

                    modelUser.UpdatedBy = userUpdateId;

                    _db.Users.Update(modelUser);
                    await _db.SaveChangesAsync();

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

            _logger.LogInformation($"call: UpdateUserAsync=> Finish");

            return result;
        }

        public async Task DeleteUserAsync(string Id, string userDeleteId)
        {
            _logger.LogInformation($"call: DeleteUserAsync");

            _logger.LogDebug($"User delete: {Id}, Deleted by: {userDeleteId}");

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

                    model.DeletedBy = userDeleteId;

                    _db.Users.Update(model);
                    await _db.SaveChangesAsync();

                    transaction.Commit();

                    //SEND TO MESSAGE QUEUE IF DELETE FUNCTION
                    var message = new MessageDTO();
                    message.UserID = Id;
                    message.Topic = "Has Deleted.";
                    message.Detail = $"Id {userDeleteId} has deleted user id {Id}.";

                    var modelMessage = new Messages();

                    message.AddToModel(modelMessage);

                    _logger.LogDebug($"data before createFromModel: {JsonSerializer.Serialize(modelMessage)}");

                    var messageNew = MessageDTO.CreateFromModel(modelMessage);

                    _logger.LogDebug($"data after createFromModel: {JsonSerializer.Serialize(messageNew)}");

                    string queueName = _rabbitMQData.QueueName ?? "";

                    await _messagePublish.MessagePublishAsync(messageNew, queueName);
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
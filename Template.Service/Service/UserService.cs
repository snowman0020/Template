using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly DatabaseContext DB;
        private readonly IConfiguration Configuration;

        public UserService(DatabaseContext db, IConfiguration configuration)
        {
            DB = db;
            Configuration = configuration;
        }

        public async Task<UserPaging> GetUserListAsync(UserFilter userFilter, PageParam pageParam, UserSortBy? userSortBy)
        {
            var result = new UserPaging();

            try
            {
                IQueryable<Users> query = DB.Users.Where(o => o.IsDeleted == false);

                if (!string.IsNullOrEmpty(userFilter.Username))
                {
                    query = query.Where(x => x.Username.ToUpper().Contains(userFilter.Username.ToUpper()));
                }
                if (!string.IsNullOrEmpty(userFilter.FullName))
                {
                    query = query.Where(x => x.FirstName.ToUpper().Contains(userFilter.FullName.ToUpper()) || 
                                             x.LastName.ToUpper().Contains(userFilter.FullName.ToUpper()));
                }
                if (!string.IsNullOrEmpty(userFilter.Phone))
                {
                    query = query.Where(x => x.Phone == userFilter.Phone);
                }
                if (!string.IsNullOrEmpty(userFilter.Email))
                {
                    query = query.Where(x => x.Email.ToUpper().Contains(userFilter.Email.ToUpper()));
                }

                if (userSortBy.UserEnumSortBy != null)
                {
                    switch (userSortBy.UserEnumSortBy.Value)
                    {
                        case UserEnumSortBy.Username:
                            if (userSortBy.Ascending) query = query.OrderBy(o => o.Username);
                            else query = query.OrderByDescending(o => o.Username);
                            break;
                        case UserEnumSortBy.FullName:
                            if (userSortBy.Ascending) query = query.OrderBy(o => o.FirstName).OrderBy(o => o.LastName);
                            else query = query.OrderByDescending(o => o.FirstName).OrderByDescending(o => o.LastName);
                            break;
                        case UserEnumSortBy.Phone:
                            if (userSortBy.Ascending) query = query.OrderBy(o => o.Phone);
                            else query = query.OrderByDescending(o => o.Phone);
                            break;
                        case UserEnumSortBy.Email:
                            if (userSortBy.Ascending) query = query.OrderBy(o => o.Email);
                            else query = query.OrderByDescending(o => o.Email);
                            break;
                        default:
                            query = query.OrderBy(o => o.FirstName);
                            break;
                    }
                }
                else
                {
                    query = query.OrderBy(o => o.FirstName);
                }

                var pageOutput = PagingData.Paging(pageParam, ref query);

                var queryResults = await query.ToListAsync();

                var data = queryResults.Select(o => new UserDTO
                {
                    ID = o.ID,
                    Username = o.Username,
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    Phone = o.Phone,
                    Email = o.Email,
                    IsDeleted = o.IsDeleted,
                    CreatedDate = o.CreatedDate,
                    CreatedBy = o.CreatedBy,
                    UpdatedDate = o.UpdatedDate,
                    UpdatedBy = o.UpdatedBy
                }).ToList();

                result = new UserPaging()
                {
                    UserList = data,
                    PageOutput = pageOutput
                };
            }
            catch (Exception ex)
            {
                throw new BadRequestException(string.Format("ไม่สามารถเรียกข้อมูลได้เกิดปัญหา ข้อความ: {0}", ex.Message.ToString()));
            }

            return result;
        }

        public async Task<UserDTO> GetUserByIdAsync(string ID)
        {
            var result = new UserDTO();

            try
            {
                var model = await DB.Users.Where(o => o.ID == ID && o.IsDeleted == false).FirstOrDefaultAsync();

                if (model == null)
                {
                    throw new BadRequestException("ไม่พบข้อมูล.");
                }

                result = UserDTO.CreateFromModel(model);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(string.Format("ไม่สามารถค้นหาข้อมูลได้เกิดปัญหา ข้อความ: {0}", ex.Message.ToString()));
            }

            return result;
        }
        public async Task<UserDTO> CreateUserAsync(UserDTO input)
        {
            var result = new UserDTO();

            if (string.IsNullOrEmpty(input.Username))
            {
                throw new BadRequestException("Username ห้ามว่าง.");
            }
            if (string.IsNullOrEmpty(input.FirstName))
            {
                throw new BadRequestException("ชื่อ ห้ามว่าง.");
            }
            if (string.IsNullOrEmpty(input.LastName))
            {
                throw new BadRequestException("นามสกุล ห้ามว่าง.");
            }
            if (string.IsNullOrEmpty(input.Phone))
            {
                throw new BadRequestException("เบอร์ติดต่อ ห้ามว่าง.");
            }
            if (string.IsNullOrEmpty(input.Email))
            {
                throw new BadRequestException("Email ห้ามว่าง.");
            }

            var model = await DB.Users.Where(o => o.FirstName == input.FirstName && o.LastName == input.LastName).FirstOrDefaultAsync();

            if (model != null)
            {
                throw new BadRequestException("ชื่อ-นามสกุล ซ้ำ.");
            }

            using (var transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    var user = new Users();

                    await input.CreateSaveToModelAsync(user);

                    await DB.Users.AddAsync(user);
                    await DB.SaveChangesAsync();

                    result = await GetUserByIdAsync(user.ID);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new BadRequestException(string.Format("ไม่สามารถสร้างได้เกิดปัญหา ข้อความ: {0}", ex.Message.ToString()));
                }
            }

            return result;
        }
        public async Task<UserDTO> UpdateUserAsync(string ID, UserDTO input)
        {
            var result = new UserDTO();

            if (string.IsNullOrEmpty(input.FirstName))
            {
                throw new BadRequestException("ชื่อ ห้ามว่าง.");
            }
            if (string.IsNullOrEmpty(input.LastName))
            {
                throw new BadRequestException("นามสกุล ห้ามว่าง.");
            }
            if (string.IsNullOrEmpty(input.Phone))
            {
                throw new BadRequestException("เบอร์ติดต่อ ห้ามว่าง.");
            }
            if (string.IsNullOrEmpty(input.Email))
            {
                throw new BadRequestException("Email ห้ามว่าง.");
            }

            var model = await DB.Users.Where(o => o.FirstName == input.FirstName && o.LastName == input.LastName).FirstOrDefaultAsync();

            if (model != null)
            {
                throw new BadRequestException("ชื่อ-นามสกุล ซ้ำ.");
            }

            using (var transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    var user = await DB.Users.Where(o => o.ID == ID).FirstOrDefaultAsync();

                    if (user != null)
                    {
                        await input.UpdateSaveToModelAsync(user);

                        DB.Users.Update(user);
                        await DB.SaveChangesAsync();
                    }

                    result = await GetUserByIdAsync(ID);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new BadRequestException(string.Format("ไม่สามารถแก้ไขได้เกิดปัญหา ข้อความ: {0}", ex.Message.ToString()));
                }
            }

            return result;
        }
        public async Task<bool> DeleteUserAsync(string ID)
        {
            var result = false;

            var model = await DB.Users.Where(o => o.ID == ID).FirstOrDefaultAsync();

            if (model == null)
            {
                throw new BadRequestException("ไม่พบข้อมูล.");
            }

            using (var transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    var input = new UserDTO();

                    await input.DeleteSaveToModelAsync(model);

                    DB.Users.Update(model);
                    await DB.SaveChangesAsync();

                    result = true;

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new BadRequestException(string.Format("ไม่สามารถลบได้เกิดปัญหา ข้อความ: {0}", ex.Message.ToString()));
                }
            }

            return result;
        }
    }
}
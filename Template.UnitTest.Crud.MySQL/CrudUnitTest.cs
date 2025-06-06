using Microsoft.EntityFrameworkCore;
using Template.Helper.PasswordHash;
using Template.Infrastructure.MySQL;
using Template.Infrastructure.MySQL.Models;

namespace Template.UnitTest.Crud.MySQL
{
    public class CrudUnitTest : IClassFixture<Startup>, IDisposable
    {
        private readonly TemplateDbContext _db;
        private readonly IPasswordHash _passwordHash;

        public CrudUnitTest(TemplateDbContext db, IPasswordHash passwordHash)
        {
            _db = db;
            _passwordHash = passwordHash;
        }

        public void Dispose()
        {
            _db.Database.EnsureDeleted();
            _db.Database.Migrate();
        }

        [Fact]
        public async Task CrudFlowTest()
        {
            Dispose();

            string userId = new Guid().ToString();

            string passwordOld = "12345";
            string passwordNew = "123456";

            var passwordEncryptOld = _passwordHash.Encrypt(passwordOld);
            var firstNameOld = "Unit Test Firstname";
            var lastNameOld = "Unit Test Lastname";
            var phoneOld = "0111111111";
            var emailOld = "unit_test@email.com";
            var createdByOld = userId;
            var createdDateOld = DateTime.Now;

            var inputAdd = new Users()
            {
                ID = userId,
                FirstName = firstNameOld,
                LastName = lastNameOld,
                Phone = phoneOld,
                Email = emailOld,
                CreatedBy = createdByOld,
                CreatedDate = createdDateOld
            };

            Assert.NotNull(inputAdd);

            inputAdd.Password = passwordEncryptOld;

            Assert.True(inputAdd.Password.Length > 6);

            Assert.Equal(userId, inputAdd.CreatedBy);

            bool isAddSuccess = false;
            try
            {
                var orderNumber = await _db.Users.MaxAsync(u => (int?)u.OrderNumber) ?? 0;

                inputAdd.OrderNumber = orderNumber + 1;

                await _db.Users.AddAsync(inputAdd);
                await _db.SaveChangesAsync();

                isAddSuccess = true;
            }
            catch
            {
            }
            finally
            {
                Assert.True(isAddSuccess);
            }

            var resultAdd = await _db.Users.Where(u => u.ID == userId && u.IsDeleted == false).FirstOrDefaultAsync();

            Assert.NotNull(resultAdd);

            resultAdd.FirstName = "Unit Test Firstname12";
            resultAdd.LastName = "Unit Test Lastname12";
            resultAdd.Phone = "0111111112";
            resultAdd.Email = "unit_test12@email.com";
            resultAdd.UpdatedBy = userId;
            resultAdd.UpdatedDate = DateTime.Now;

            Assert.NotNull(resultAdd);

            resultAdd.Password = _passwordHash.Encrypt(passwordNew);

            Assert.NotEqual(passwordEncryptOld, resultAdd.Password);

            Assert.Equal(resultAdd.ID, resultAdd.UpdatedBy);

            bool isUpdateSuccess = false;
            try
            {
                _db.Users.Update(resultAdd);
                await _db.SaveChangesAsync();

                isUpdateSuccess = true;
            }
            catch
            {
            }
            finally
            {
                Assert.True(isUpdateSuccess);
            }

            var resultUpdate = await _db.Users.Where(u => u.ID == userId && u.IsDeleted == false).FirstOrDefaultAsync();

            Assert.NotNull(resultUpdate);
            Assert.NotEqual(resultUpdate.FirstName, firstNameOld);
            Assert.NotEqual(resultUpdate.LastName, lastNameOld);
            Assert.NotEqual(resultUpdate.Phone, phoneOld);
            Assert.NotEqual(resultUpdate.Email, emailOld);

            resultUpdate.IsDeleted = true;
            resultUpdate.DeletedBy = userId;
            resultUpdate.DeletedDate = DateTime.Now;

            bool isDeleteSuccess = false;
            try
            {
                _db.Users.Update(resultUpdate);
                await _db.SaveChangesAsync();

                isDeleteSuccess = true;
            }
            catch
            {
            }
            finally
            {
                Assert.True(isDeleteSuccess);
            }

            var resultDelete = await _db.Users.Where(u => u.ID == userId).FirstOrDefaultAsync();

            Assert.NotNull(resultDelete);
            Assert.True(resultDelete.IsDeleted);
        }
    }
}
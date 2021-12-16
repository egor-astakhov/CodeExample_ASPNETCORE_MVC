using Integral.Application.Common.Persistence;
using Integral.Application.Common.Services;
using Integral.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Integral.Infrastructure.UnitTests.Persistence
{
    public class ApplicationDbContextTests : IDisposable
    {
        private readonly string _userId;
        private readonly DateTime _dateTime;
        private readonly Mock<IDateTimeService> _dateTimeServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly ApplicationDbContext _context;

        public ApplicationDbContextTests()
        {
            _dateTime = new DateTime(3000, 1, 1);
            _dateTimeServiceMock = new Mock<IDateTimeService>();
            _dateTimeServiceMock.Setup(m => m.Now).Returns(_dateTime);

            _userId = "00000000-0000-0000-0000-000000000000";
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _currentUserServiceMock.Setup(m => m.UserId).Returns(_userId);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options, _currentUserServiceMock.Object, _dateTimeServiceMock.Object);

            //_sut.TodoItems.Add(new TodoItem
            //{
            //    Id = 1,
            //    Title = "Do this thing."
            //});

            //_sut.SaveChanges();
        }

        //[Fact]
        //public async Task SaveChangesAsync_GivenNewTodoItem_ShouldSetCreatedProperties()
        //{
        //    var item = new TodoItem
        //    {
        //        Id = 2,
        //        Title = "This thing is done.",
        //        Done = true
        //    };

        //    _sut.TodoItems.Add(item);

        //    await _sut.SaveChangesAsync();

        //    item.Created.ShouldBe(_dateTime);
        //    item.CreatedBy.ShouldBe(_userId);
        //}

        //[Fact]
        //public async Task SaveChangesAsync_GivenExistingTodoItem_ShouldSetLastModifiedProperties()
        //{
        //    long id = 1;

        //    var item = await _sut.TodoItems.FindAsync(id);

        //    item.Done = true;

        //    await _sut.SaveChangesAsync();

        //    item.LastModified.ShouldNotBeNull();
        //    item.LastModified.ShouldBe(_dateTime);
        //    item.LastModifiedBy.ShouldBe(_userId);
        //}



        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

using Colegio;
using Colegio.Models;
using Colegio.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Student.Producer;

public class RegistrationTests
{
    [Fact]
    public void GetRegistrations_ReturnsRegistrationList_WhenDataExists()
    {
        // Arrange
        var data = new List<RegistrationDto>
        {
            new RegistrationDto {Id = Guid.NewGuid(),StudentIdentification= 123,Institution="Colegio Pereira",City="Pereira",GradeId=Guid.NewGuid(),StudentId=Guid.NewGuid() },
            new RegistrationDto {Id = Guid.NewGuid(),StudentIdentification= 456,Institution="Colegio Salle",City="Bogota",GradeId=Guid.NewGuid(),StudentId=Guid.NewGuid() },
        }.AsQueryable();

        Mock<DbSet<RegistrationDto>> mockSet = MoqDbsetRegistration(data);

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Registrations).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Registration>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Registration(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.GetRegistrations();

        // Assert
        Assert.Equal(data.ToList(), result);
    }

    private static Mock<DbSet<RegistrationDto>> MoqDbsetRegistration(IQueryable<RegistrationDto> data)
    {
        var mockSet = new Mock<DbSet<RegistrationDto>>();
        mockSet.As<IQueryable<RegistrationDto>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<RegistrationDto>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<RegistrationDto>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        mockSet.Setup(m => m.Add(It.IsAny<RegistrationDto>())).Callback<RegistrationDto>((s) => data.ToList().Add(s));
        mockSet.Setup(m => m.Remove(It.IsAny<RegistrationDto>())).Callback<RegistrationDto>((s) => data.ToList().Remove(s));
        mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(d => d.Id == (Guid)ids[0]));
        return mockSet;
    }

    [Fact]
    public void GetRegistrations_ReturnsNull()
    {
        // Arrange
        var data = new List<RegistrationDto>();

        Mock<DbSet<RegistrationDto>> mockSet = MoqDbsetRegistration(data.AsQueryable());

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Registrations).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Registration>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Registration(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.GetRegistrations();

        // Assert
        Assert.Equal(data.ToList(), result);
    }

    [Fact]
    public void CreateRegistration_ReturnsTrue_WhenRegistrationIsCreated()
    {
        // Arrange
        var registration = new RegistrationDto { StudentIdentification = 123, Institution = "Colegio Pereira", City = "Pereira", GradeId = Guid.NewGuid(), StudentId = Guid.NewGuid() };

        var mockSet = new Mock<DbSet<RegistrationDto>>();
        mockSet.Setup(m => m.Add(It.IsAny<RegistrationDto>()));

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Registrations).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Registration>>();
        var mockProducer = new Mock<IProducer>();
        mockProducer.Setup(p => p.ProduceMessage(It.IsAny<string>()));
        var service = new Registration(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.CreateRegistration(registration);

        // Assert
        Assert.True(result);
        mockSet.Verify(m => m.Add(It.IsAny<RegistrationDto>()), Times.Once());
        mockContext.Verify(m => m.SaveChanges(), Times.Once());
    }

    [Fact]
    public void DeleteRegistration_ReturnsTrue_WhenRegistrationExists()
    {
        // Arrange
        var registrationId = Guid.NewGuid();
        var registration = new RegistrationDto { Id = registrationId, StudentIdentification = 123, Institution = "Colegio Pereira", City = "Pereira", GradeId = Guid.NewGuid(), StudentId = Guid.NewGuid() };

        var data = new List<RegistrationDto> { registration }.AsQueryable();
        var mockSet = MoqDbsetRegistration(data);

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Registrations).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Registration>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Registration(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.DeleteRegistration(registrationId);

        // Assert
        Assert.True(result);
        mockSet.Verify(m => m.Remove(It.IsAny<RegistrationDto>()), Times.Once());
        mockContext.Verify(m => m.SaveChanges(), Times.Once());
    }

    [Fact]
    public void DeleteRegistration_ReturnsFalse_WhenRegistrationDoesNotExist()
    {
        // Arrange
        var registrationId = Guid.NewGuid();

        var data = new List<RegistrationDto>().AsQueryable();
        var mockSet = MoqDbsetRegistration(data);

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Registrations).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Registration>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Registration(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.DeleteRegistration(registrationId);

        // Assert
        Assert.False(result);
        mockSet.Verify(m => m.Remove(It.IsAny<RegistrationDto>()), Times.Never());
        mockContext.Verify(m => m.SaveChanges(), Times.Never());
    }

    [Fact]
    public void DeleteRegistration_LogsError_WhenExceptionIsThrown()
    {
        // Arrange
        var registrationId = Guid.NewGuid();

        var mockSet = new Mock<DbSet<RegistrationDto>>();
        mockSet.Setup(m => m.Find(registrationId)).Throws(new Exception("Test exception"));

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Registrations).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Registration>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Registration(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.DeleteRegistration(registrationId);

        // Assert
        Assert.False(result);
        mockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
    }

    [Fact]
    public void GetRegistrationByEstudentIdentification_ReturnsRegistration_WhenRegistrationExists()
    {
        // Arrange
        var registrationId = 123;
        var registration = new RegistrationDto { Id = Guid.NewGuid(), StudentIdentification = 123, Institution = "Colegio Pereira", City = "Pereira", GradeId = Guid.NewGuid(), StudentId = Guid.NewGuid() };

        var data = new List<RegistrationDto> { registration }.AsQueryable();
        var mockSet = MoqDbsetRegistration(data);

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Registrations).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Registration>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Registration(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.GetRegistrationByEstudentIdentification(registrationId);

        // Assert
        Assert.Equal(registration, result);
    }


}
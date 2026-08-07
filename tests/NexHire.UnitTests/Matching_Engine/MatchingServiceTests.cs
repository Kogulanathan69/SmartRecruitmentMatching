using NexHire.Application.Modules.Matching_Engine.DTOs;
using NexHire.Application.Modules.Matching_Engine.Services;

namespace NexHire.UnitTests.Matching_Engine;

public class MatchingServiceTests
{
    [Fact]
    public void Calculate_WithValidRequest_ReturnsExpectedScore()
    {
        // Arrange
        var service = new MatchingService();

        var request = new CalculateMatchRequest
        {
            CandidateProfileId = Guid.NewGuid(),
            VacancyId = Guid.NewGuid(),
            SkillsScore = 80m,
            ExperienceScore = 75m,
            EducationScore = 100m,
            LocationScore = 100m
        };

        // Act
        var result = service.Calculate(request);

        // Assert
        Assert.Equal(request.CandidateProfileId, result.CandidateProfileId);
        Assert.Equal(request.VacancyId, result.VacancyId);
        Assert.Equal("RM-1.0", result.RuleVersion);
        Assert.Equal(84m, result.TotalScore);

        Assert.False(string.IsNullOrWhiteSpace(result.ReplayReceipt));
    }

    [Fact]
    public void Calculate_WithSameInput_ReturnsSameReplayReceipt()
    {
        // Arrange
        var service = new MatchingService();

        var request = new CalculateMatchRequest
        {
            CandidateProfileId = Guid.NewGuid(),
            VacancyId = Guid.NewGuid(),
            SkillsScore = 80m,
            ExperienceScore = 75m,
            EducationScore = 100m,
            LocationScore = 100m
        };

        // Act
        var firstResult = service.Calculate(request);
        var secondResult = service.Calculate(request);

        // Assert
        Assert.Equal(
            firstResult.ReplayReceipt,
            secondResult.ReplayReceipt);
    }

    [Fact]
    public void Calculate_WithEmptyCandidateProfileId_ThrowsException()
    {
        // Arrange
        var service = new MatchingService();

        var request = new CalculateMatchRequest
        {
            CandidateProfileId = Guid.Empty,
            VacancyId = Guid.NewGuid(),
            SkillsScore = 80m,
            ExperienceScore = 75m,
            EducationScore = 100m,
            LocationScore = 100m
        };

        // Act
        var action = () => service.Calculate(request);

        // Assert
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Calculate_WithEmptyVacancyId_ThrowsException()
    {
        // Arrange
        var service = new MatchingService();

        var request = new CalculateMatchRequest
        {
            CandidateProfileId = Guid.NewGuid(),
            VacancyId = Guid.Empty,
            SkillsScore = 80m,
            ExperienceScore = 75m,
            EducationScore = 100m,
            LocationScore = 100m
        };

        // Act
        var action = () => service.Calculate(request);

        // Assert
        Assert.Throws<ArgumentException>(action);
    }
}
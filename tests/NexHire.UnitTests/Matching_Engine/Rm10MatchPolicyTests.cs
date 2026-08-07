using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NexHire.Domain.Modules.Matching_Engine.Policies;

namespace NexHire.UnitTests.Matching_Engine;

public class Rm10MatchPolicyTests
{
    [Fact]
    public void Calculate_WithValidComponentScores_ReturnsExpectedWeightedScore()
    {
        // Arrange
        var scores = new MatchComponentScores(
            Skills: 80m,
            Experience: 75m,
            Education: 100m,
            Location: 100m);

        // Act
        var result = Rm10MatchPolicy.Calculate(scores);

        // Assert
        Assert.Equal("RM-1.0", result.RuleVersion);

        Assert.Equal(44m, result.SkillPoints);
        Assert.Equal(15m, result.ExperiencePoints);
        Assert.Equal(15m, result.EducationPoints);
        Assert.Equal(10m, result.LocationPoints);

        Assert.Equal(84m, result.TotalScore);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Calculate_WithInvalidComponentScore_ThrowsException(
        decimal invalidScore)
    {
        // Arrange
        var scores = new MatchComponentScores(
            Skills: invalidScore,
            Experience: 50m,
            Education: 50m,
            Location: 50m);

        // Act
        var action = () => Rm10MatchPolicy.Calculate(scores);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(action);
    }
}

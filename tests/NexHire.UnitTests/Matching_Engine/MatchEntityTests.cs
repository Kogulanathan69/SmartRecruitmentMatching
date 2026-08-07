using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NexHire.Domain.Modules.Matching_Engine.Entities;

namespace NexHire.UnitTests.Matching_Engine;

public class MatchEntityTests
{
    [Fact]
    public void MatchRuleVersion_DefaultWeights_ShouldEqualOneHundredPercent()
    {
        // Arrange
        var rule = new MatchRuleVersion();

        // Act
        var isValid = rule.HasValidWeights();

        // Assert
        Assert.True(isValid);

        Assert.Equal("RM-1.0", rule.Version);
        Assert.Equal(0.55m, rule.SkillsWeight);
        Assert.Equal(0.20m, rule.ExperienceWeight);
        Assert.Equal(0.15m, rule.EducationWeight);
        Assert.Equal(0.10m, rule.LocationWeight);
    }

    [Fact]
    public void MatchRuleVersion_InvalidWeights_ShouldReturnFalse()
    {
        // Arrange
        var rule = new MatchRuleVersion
        {
            SkillsWeight = 0.50m
        };

        // Act
        var isValid = rule.HasValidWeights();

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void MatchResult_WhenCreated_ShouldGenerateIdentifier()
    {
        // Arrange & Act
        var result = new MatchResult();

        // Assert
        Assert.NotEqual(Guid.Empty, result.MatchResultId);
        Assert.NotEqual(default, result.CalculatedAt);
    }
}
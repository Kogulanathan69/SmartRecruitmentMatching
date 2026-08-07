using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NexHire.Application.Modules.Matching_Engine.DTOs;

namespace NexHire.Application.Modules.Matching_Engine.Interfaces;

/// <summary>
/// Defines the application-level matching operation.
/// The implementation will use the deterministic RM-1.0 domain policy.
/// </summary>
public interface IMatchingService
{
    CalculateMatchResponse Calculate(CalculateMatchRequest request);
}
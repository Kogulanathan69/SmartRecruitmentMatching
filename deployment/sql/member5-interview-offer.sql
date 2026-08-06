/*
  NexHire Member 5 schema — REVIEW BEFORE APPLYING.
  ApplicationId, CompanyId, CandidateProfileId, and UserId foreign keys are not added here
  because the final shared table names and key types must be confirmed by the owning members.
*/

CREATE TABLE dbo.Interviews
(
    InterviewId UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Interviews PRIMARY KEY,
    ApplicationId UNIQUEIDENTIFIER NOT NULL,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    CandidateProfileId UNIQUEIDENTIFIER NOT NULL,
    ScheduledAtUtc DATETIMEOFFSET NOT NULL,
    DurationMinutes INT NOT NULL,
    Mode INT NOT NULL,
    MeetingLink NVARCHAR(1000) NULL,
    Location NVARCHAR(500) NULL,
    ContactPhone NVARCHAR(50) NULL,
    Notes NVARCHAR(4000) NULL,
    Status INT NOT NULL,
    CancellationReason NVARCHAR(1000) NULL,
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    CreatedAtUtc DATETIMEOFFSET NOT NULL,
    UpdatedAtUtc DATETIMEOFFSET NOT NULL,
    CompletedAtUtc DATETIMEOFFSET NULL,
    RowVersion ROWVERSION NOT NULL,
    CONSTRAINT CK_Interviews_Duration CHECK (DurationMinutes BETWEEN 15 AND 480),
    CONSTRAINT CK_Interviews_Mode CHECK (Mode IN (1,2,3)),
    CONSTRAINT CK_Interviews_Status CHECK (Status IN (1,2,3,4))
);

CREATE INDEX IX_Interviews_ApplicationId ON dbo.Interviews(ApplicationId);
CREATE INDEX IX_Interviews_Company_Schedule ON dbo.Interviews(CompanyId, ScheduledAtUtc DESC);
CREATE INDEX IX_Interviews_Candidate_Schedule ON dbo.Interviews(CandidateProfileId, ScheduledAtUtc DESC);

CREATE TABLE dbo.InterviewScores
(
    InterviewScoreId UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_InterviewScores PRIMARY KEY,
    InterviewId UNIQUEIDENTIFIER NOT NULL,
    EvaluatorUserId UNIQUEIDENTIFIER NOT NULL,
    Score INT NOT NULL,
    Feedback NVARCHAR(4000) NOT NULL,
    CreatedAtUtc DATETIMEOFFSET NOT NULL,
    CONSTRAINT FK_InterviewScores_Interviews FOREIGN KEY (InterviewId)
        REFERENCES dbo.Interviews(InterviewId) ON DELETE CASCADE,
    CONSTRAINT UQ_InterviewScores_Interview_Evaluator UNIQUE (InterviewId, EvaluatorUserId),
    CONSTRAINT CK_InterviewScores_Score CHECK (Score BETWEEN 0 AND 100)
);

CREATE TABLE dbo.Offers
(
    OfferId UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Offers PRIMARY KEY,
    ApplicationId UNIQUEIDENTIFIER NOT NULL,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    CandidateProfileId UNIQUEIDENTIFIER NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Currency CHAR(3) NOT NULL,
    StartDate DATE NOT NULL,
    ExpiresAtUtc DATETIMEOFFSET NOT NULL,
    Terms NVARCHAR(MAX) NOT NULL,
    Status INT NOT NULL,
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    CreatedAtUtc DATETIMEOFFSET NOT NULL,
    UpdatedAtUtc DATETIMEOFFSET NOT NULL,
    SentAtUtc DATETIMEOFFSET NULL,
    RespondedAtUtc DATETIMEOFFSET NULL,
    RejectionReason NVARCHAR(2000) NULL,
    WithdrawalReason NVARCHAR(2000) NULL,
    RowVersion ROWVERSION NOT NULL,
    CONSTRAINT CK_Offers_Amount CHECK (Amount > 0),
    CONSTRAINT CK_Offers_Status CHECK (Status IN (1,2,3,4,5,6))
);

CREATE INDEX IX_Offers_ApplicationId ON dbo.Offers(ApplicationId);
CREATE INDEX IX_Offers_Company_Created ON dbo.Offers(CompanyId, CreatedAtUtc DESC);
CREATE INDEX IX_Offers_Candidate_Created ON dbo.Offers(CandidateProfileId, CreatedAtUtc DESC);
CREATE INDEX IX_Offers_Status_Expiry ON dbo.Offers(Status, ExpiresAtUtc);

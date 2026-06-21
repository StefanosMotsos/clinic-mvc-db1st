USE [ClinicMVCDBFirst];
GO

-- ============================================
-- 1. ROLES
-- ============================================
CREATE TABLE [dbo].[Roles] (
    [Id]        INT             IDENTITY(1, 1) NOT NULL,
    [Name]      NVARCHAR(50)    NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Roles_Name] UNIQUE ([Name])
);
GO

CREATE NONCLUSTERED INDEX [IX_Roles_Name]
    ON [dbo].[Roles]([Name] ASC);
GO

-- ============================================
-- 2. CAPABILITIES
-- ============================================
CREATE TABLE [dbo].[Capabilities] (
    [Id]            INT             IDENTITY(1, 1) NOT NULL,
    [Name]          NVARCHAR(100)   NOT NULL,
    [Description]   NVARCHAR(255)   NULL,
    CONSTRAINT [PK_Capabilities] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Capabilities_Name] UNIQUE ([Name])
);
GO

CREATE NONCLUSTERED INDEX [IX_Capabilities_Name]
    ON [dbo].[Capabilities]([Name] ASC);
GO

-- ============================================
-- 3. ROLES_CAPABILITIES (Many-to-Many)
-- ============================================
CREATE TABLE [dbo].[RolesCapabilities] (
    [CapabilitiesId]    INT NOT NULL,
    [RolesId]           INT NOT NULL,
    CONSTRAINT [PK_RolesCapabilities] PRIMARY KEY CLUSTERED ([CapabilitiesId], [RolesId]),

    CONSTRAINT [FK_RolesCapabilities_Capabilities]
        FOREIGN KEY ([CapabilitiesId]) REFERENCES [dbo].[Capabilities]([Id])
        ON DELETE CASCADE,

    CONSTRAINT [FK_RolesCapabilities_Roles]
        FOREIGN KEY ([RolesId]) REFERENCES [dbo].[Roles]([Id])
        ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_RolesCapabilities_CapabilityId]
    ON [dbo].[RolesCapabilities]([CapabilitiesId] ASC);
GO

-- ============================================
-- 4. USERS  (BaseEntity: audit + soft delete)
-- ============================================
CREATE TABLE [dbo].[Users] (
    [Id]            INT                 IDENTITY(1, 1) NOT NULL,
    [Username]      NVARCHAR(50)        NOT NULL,
    [Password]      NVARCHAR(100)       NOT NULL,
    [Email]         NVARCHAR(50)        NOT NULL,
    [Firstname]     NVARCHAR(50)        NOT NULL,
    [Lastname]      NVARCHAR(50)        NOT NULL,
    [RoleId]        INT                 NOT NULL,

    -- BaseEntity columns
    [InsertedAt]    DATETIME2           NOT NULL CONSTRAINT [DF_Users_InsertedAt] DEFAULT (SYSUTCDATETIME()),
    [ModifiedAt]    DATETIME2           NOT NULL CONSTRAINT [DF_Users_ModifiedAt] DEFAULT (SYSUTCDATETIME()),
    [IsDeleted]     BIT                 NOT NULL CONSTRAINT [DF_Users_IsDeleted] DEFAULT (0),
    [DeletedAt]     DATETIME2           NULL,
    [Uuid]          UNIQUEIDENTIFIER    NOT NULL CONSTRAINT [DF_Users_Uuid] DEFAULT (NEWID()),

    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),

    CONSTRAINT [FK_Users_RoleId]
        FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([Id])
        ON DELETE NO ACTION
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Username]
    ON [dbo].[Users]([Username] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email]
    ON [dbo].[Users]([Email] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Users_RoleId]
    ON [dbo].[Users]([RoleId] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Uuid]
    ON [dbo].[Users]([Uuid] ASC);
GO

-- ============================================
-- 5. DOCTORS  (BaseEntity: audit + soft delete)
-- ============================================
CREATE TABLE [dbo].[Doctors] (
    [Id]            INT                 IDENTITY(1, 1) NOT NULL,
    [Specialty]     NVARCHAR(50)        NOT NULL,
    [PhoneNumber]   NVARCHAR(20)        NOT NULL,
    [UserId]        INT                 NOT NULL,

    -- BaseEntity columns
    [InsertedAt]    DATETIME2           NOT NULL CONSTRAINT [DF_Doctors_InsertedAt] DEFAULT (SYSUTCDATETIME()),
    [ModifiedAt]    DATETIME2           NOT NULL CONSTRAINT [DF_Doctors_ModifiedAt] DEFAULT (SYSUTCDATETIME()),
    [IsDeleted]     BIT                 NOT NULL CONSTRAINT [DF_Doctors_IsDeleted] DEFAULT (0),
    [DeletedAt]     DATETIME2           NULL,
    [Uuid]          UNIQUEIDENTIFIER    NOT NULL CONSTRAINT [DF_Doctors_Uuid] DEFAULT (NEWID()),

    CONSTRAINT [PK_Doctors] PRIMARY KEY CLUSTERED ([Id] ASC),

    CONSTRAINT [FK_Doctors_UserId]
        FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
        ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_Doctors_Specialty]
    ON [dbo].[Doctors]([Specialty] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Doctors_UserId]
    ON [dbo].[Doctors]([UserId] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Doctors_Uuid]
    ON [dbo].[Doctors]([Uuid] ASC);
GO

-- ============================================
-- 6. PATIENTS  (BaseEntity: audit + soft delete)
-- ============================================
CREATE TABLE [dbo].[Patients] (
    [Id]            INT                 IDENTITY(1, 1) NOT NULL,
    [AMKA]          NVARCHAR(11)        NOT NULL,
    [DateOfBirth]   DATE                NOT NULL,
    [BloodType]     NVARCHAR(20)        NULL,
    [UserId]        INT                 NOT NULL,

    -- BaseEntity columns
    [InsertedAt]    DATETIME2           NOT NULL CONSTRAINT [DF_Patients_InsertedAt] DEFAULT (SYSUTCDATETIME()),
    [ModifiedAt]    DATETIME2           NOT NULL CONSTRAINT [DF_Patients_ModifiedAt] DEFAULT (SYSUTCDATETIME()),
    [IsDeleted]     BIT                 NOT NULL CONSTRAINT [DF_Patients_IsDeleted] DEFAULT (0),
    [DeletedAt]     DATETIME2           NULL,
    [Uuid]          UNIQUEIDENTIFIER    NOT NULL CONSTRAINT [DF_Patients_Uuid] DEFAULT (NEWID()),

    CONSTRAINT [PK_Patients] PRIMARY KEY CLUSTERED ([Id] ASC),

    CONSTRAINT [FK_Patients_UserId]
        FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
        ON DELETE CASCADE
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Patients_AMKA]
    ON [dbo].[Patients]([AMKA] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Patients_UserId]
    ON [dbo].[Patients]([UserId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Patients_BloodType]
    ON [dbo].[Patients]([BloodType] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Patients_Uuid]
    ON [dbo].[Patients]([Uuid] ASC);
GO

-- ============================================
-- 7. MEDICALPROGRAMS  (no audit columns)
-- ============================================
CREATE TABLE [dbo].[MedicalPrograms] (
    [Id]            INT             IDENTITY(1, 1) NOT NULL,
    [Title]         NVARCHAR(100)   NOT NULL,
    [Description]   NVARCHAR(255)   NOT NULL,
    [DoctorId]      INT             NOT NULL,
    CONSTRAINT [PK_MedicalPrograms] PRIMARY KEY CLUSTERED ([Id] ASC),

    CONSTRAINT [FK_Programs_DoctorId]
        FOREIGN KEY ([DoctorId]) REFERENCES [dbo].[Doctors]([Id])
        ON DELETE NO ACTION
);
GO

CREATE NONCLUSTERED INDEX [IX_MedicalPrograms_Description]
    ON [dbo].[MedicalPrograms]([Description] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_MedicalPrograms_DoctorId]
    ON [dbo].[MedicalPrograms]([DoctorId] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_MedicalPrograms_Title]
    ON [dbo].[MedicalPrograms]([Title] ASC);
GO

-- ============================================
-- 8. PATIENTSPROGRAMS (Many-to-Many, no audit columns)
-- ============================================
CREATE TABLE [dbo].[PatientsPrograms] (
    [ProgramsId]    INT NOT NULL,
    [PatientsId]    INT NOT NULL,
    CONSTRAINT [PK_PatientsPrograms] PRIMARY KEY CLUSTERED ([ProgramsId], [PatientsId]),

    CONSTRAINT [FK_PatientsPrograms_MedicalPrograms]
        FOREIGN KEY ([ProgramsId]) REFERENCES [dbo].[MedicalPrograms]([Id])
        ON DELETE CASCADE,

    CONSTRAINT [FK_PatientsPrograms_Patients]
        FOREIGN KEY ([PatientsId]) REFERENCES [dbo].[Patients]([Id])
        ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_PatientsPrograms_PatientsId]
    ON [dbo].[PatientsPrograms]([PatientsId] ASC);
GO

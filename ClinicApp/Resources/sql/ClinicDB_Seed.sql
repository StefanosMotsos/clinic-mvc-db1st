USE [ClinicMVCDBFirst];
GO

-- ============================================
-- Insert Roles
-- ============================================
INSERT INTO [dbo].[Roles] ([Name])
VALUES
    ('ADMIN'),
    ('EMPLOYEE'),
    ('DOCTOR'),
    ('PATIENT');
GO

-- ============================================
-- Insert Capabilities
-- ============================================
INSERT INTO [dbo].[Capabilities] ([Name], [Description])
VALUES
    ('INSERT_DOCTOR',           'Create a new doctor'),
    ('VIEW_DOCTORS',            'View doctor list and details'),
    ('VIEW_DOCTOR',             'View doctor'),
    ('EDIT_DOCTOR',             'Modify existing doctor'),
    ('DELETE_DOCTOR',           'Remove a doctor'),
    ('VIEW_ONLY_DOCTOR',        'View only own doctor details'),
    ('INSERT_PATIENT',          'Create a new patient'),
    ('VIEW_PATIENTS',           'View patient list and details'),
    ('VIEW_PATIENT',            'View patient'),
    ('EDIT_PATIENT',            'Modify existing patient'),
    ('DELETE_PATIENT',          'Remove a patient'),
    ('VIEW_ONLY_PATIENT',       'View only own patient details'),
    ('INSERT_MEDICALPROGRAM',   'Create a new medical program'),
    ('VIEW_MEDICALPROGRAMS',    'View medical program list and details'),
    ('VIEW_MEDICALPROGRAM',     'View medical program'),
    ('EDIT_MEDICALPROGRAM',     'Modify existing medical program'),
    ('DELETE_MEDICALPROGRAM',   'Remove a medical program');
GO

-- ============================================
-- ADMIN: all capabilities
-- ============================================
INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
SELECT r.[Id], c.[Id]
FROM [dbo].[Roles] r
CROSS JOIN [dbo].[Capabilities] c
WHERE r.[Name] = 'ADMIN';
GO

-- ============================================
-- EMPLOYEE: VIEW_DOCTORS, VIEW_DOCTOR,
--           VIEW_PATIENTS, VIEW_PATIENT,
--           VIEW_MEDICALPROGRAMS, VIEW_MEDICALPROGRAM
-- ============================================
INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
SELECT r.[Id], c.[Id]
FROM [dbo].[Roles] r
CROSS JOIN [dbo].[Capabilities] c
WHERE r.[Name] = 'EMPLOYEE'
  AND c.[Name] IN (
    'VIEW_DOCTORS', 'VIEW_DOCTOR',
    'VIEW_PATIENTS', 'VIEW_PATIENT',
    'VIEW_MEDICALPROGRAMS', 'VIEW_MEDICALPROGRAM'
  );
GO

-- ============================================
-- DOCTOR: VIEW_ONLY_DOCTOR,
--         VIEW_PATIENTS, VIEW_PATIENT,
--         VIEW_MEDICALPROGRAMS, VIEW_MEDICALPROGRAM,
--         INSERT_MEDICALPROGRAM, EDIT_MEDICALPROGRAM
-- ============================================
INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
SELECT r.[Id], c.[Id]
FROM [dbo].[Roles] r
CROSS JOIN [dbo].[Capabilities] c
WHERE r.[Name] = 'DOCTOR'
  AND c.[Name] IN (
    'VIEW_ONLY_DOCTOR',
    'VIEW_PATIENTS', 'VIEW_PATIENT',
    'VIEW_MEDICALPROGRAMS', 'VIEW_MEDICALPROGRAM',
    'INSERT_MEDICALPROGRAM', 'EDIT_MEDICALPROGRAM'
  );
GO

-- ============================================
-- PATIENT: VIEW_ONLY_PATIENT,
--          VIEW_MEDICALPROGRAMS, VIEW_MEDICALPROGRAM
-- ============================================
INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
SELECT r.[Id], c.[Id]
FROM [dbo].[Roles] r
CROSS JOIN [dbo].[Capabilities] c
WHERE r.[Name] = 'PATIENT'
  AND c.[Name] IN (
    'VIEW_ONLY_PATIENT',
    'VIEW_MEDICALPROGRAMS', 'VIEW_MEDICALPROGRAM'
  );
GO

-- ============================================
-- Seed ADMIN user
-- Plaintext password (dev only): Cod1ngF@
-- Stored value is a real BCrypt hash (work factor 11),
-- verifiable via BCrypt.Net.BCrypt.Verify(plain, hash) in EncryptedUtil.
-- ============================================
INSERT INTO [dbo].[Users] ([Username], [Password], [Email], [Firstname], [Lastname], [RoleId])
SELECT 'admin', '$2b$11$g/5vUK6AbuGzibKd3gBYG.dTZXriQ.Hg9OwJ2I4LWVczS2hSeY93.', 'admin@clinic.local', 'Admin', 'User', r.[Id]
FROM [dbo].[Roles] r
WHERE r.[Name] = 'ADMIN';
GO

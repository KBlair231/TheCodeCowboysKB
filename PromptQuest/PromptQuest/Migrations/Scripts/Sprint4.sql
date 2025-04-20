IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Enemies] (
    [EnemyId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [ImageUrl] nvarchar(max) NOT NULL,
    [MaxHealth] int NOT NULL,
    [CurrentHealth] int NOT NULL,
    [Attack] int NOT NULL,
    [Defense] int NOT NULL,
    CONSTRAINT [PK_Enemies] PRIMARY KEY ([EnemyId])
);

CREATE TABLE [Players] (
    [PlayerId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [HealthPotions] int NOT NULL,
    [MaxHealth] int NOT NULL,
    [CurrentHealth] int NOT NULL,
    [Defense] int NOT NULL,
    [Attack] int NOT NULL,
    [Class] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Players] PRIMARY KEY ([PlayerId])
);

CREATE TABLE [GameStates] (
    [UserGoogleId] nvarchar(450) NOT NULL,
    [PlayerId] int NULL,
    [EnemyId] int NULL,
    [StoredMessages] nvarchar(max) NOT NULL,
    [InCombat] bit NOT NULL,
    [InCampsite] bit NOT NULL,
    [InEvent] bit NOT NULL,
    [IsPlayersTurn] bit NOT NULL,
    [PlayerLocation] int NOT NULL,
    [IsLocationComplete] bit NOT NULL,
    [Floor] int NOT NULL,
    CONSTRAINT [PK_GameStates] PRIMARY KEY ([UserGoogleId]),
    CONSTRAINT [FK_GameStates_Enemies_EnemyId] FOREIGN KEY ([EnemyId]) REFERENCES [Enemies] ([EnemyId]) ON DELETE CASCADE,
    CONSTRAINT [FK_GameStates_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([PlayerId]) ON DELETE CASCADE
);

CREATE TABLE [Items] (
    [ItemId] int NOT NULL IDENTITY,
    [PlayerId] int NOT NULL,
    [Equipped] bit NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Attack] int NOT NULL,
    [Defense] int NOT NULL,
    [ImageSrc] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Items] PRIMARY KEY ([ItemId]),
    CONSTRAINT [FK_Items_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([PlayerId]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_GameStates_EnemyId] ON [GameStates] ([EnemyId]) WHERE [EnemyId] IS NOT NULL;

CREATE UNIQUE INDEX [IX_GameStates_PlayerId] ON [GameStates] ([PlayerId]) WHERE [PlayerId] IS NOT NULL;

CREATE INDEX [IX_Items_PlayerId] ON [Items] ([PlayerId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250419235551_InitialCreate', N'9.0.2');

COMMIT;
GO


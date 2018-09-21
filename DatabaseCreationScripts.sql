SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Categories](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Achievements](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Details] [nvarchar](max) NOT NULL,
	[IsSecret] [bit] NOT NULL,
 CONSTRAINT [PK_Achievements] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Achievements] ADD  CONSTRAINT [DF_Achievements_IsSecret]  DEFAULT ((0)) FOR [IsSecret]
GO

ALTER TABLE [dbo].[Achievements]  WITH CHECK ADD  CONSTRAINT [FK_Achievements_Categories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
GO

ALTER TABLE [dbo].[Achievements] CHECK CONSTRAINT [FK_Achievements_Categories]
GO

CREATE TABLE [dbo].[UserAchievements](
	[UserId] [nvarchar](200) NOT NULL,
	[AchievementId] [int] NOT NULL,
	[DateUnlocked] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_UserAchievements] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[AchievementId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserAchievements] ADD  CONSTRAINT [DF_UserAchievements_DateUnlocked]  DEFAULT (getutcdate()) FOR [DateUnlocked]
GO
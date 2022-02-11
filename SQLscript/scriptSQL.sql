USE [master]
GO
/****** Object:  Database [Tournaments]    Script Date: 11.02.2022 21:03:29 ******/
CREATE DATABASE [Tournaments]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Tournaments', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\Tournaments.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Tournaments_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\Tournaments_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [Tournaments] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Tournaments].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Tournaments] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Tournaments] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Tournaments] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Tournaments] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Tournaments] SET ARITHABORT OFF 
GO
ALTER DATABASE [Tournaments] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Tournaments] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Tournaments] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Tournaments] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Tournaments] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Tournaments] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Tournaments] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Tournaments] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Tournaments] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Tournaments] SET  ENABLE_BROKER 
GO
ALTER DATABASE [Tournaments] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Tournaments] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Tournaments] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Tournaments] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Tournaments] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Tournaments] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Tournaments] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Tournaments] SET RECOVERY FULL 
GO
ALTER DATABASE [Tournaments] SET  MULTI_USER 
GO
ALTER DATABASE [Tournaments] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Tournaments] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Tournaments] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Tournaments] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Tournaments] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Tournaments] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'Tournaments', N'ON'
GO
ALTER DATABASE [Tournaments] SET QUERY_STORE = OFF
GO
USE [Tournaments]
GO
/****** Object:  Table [dbo].[MatchupEntries]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MatchupEntries](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[MatchupId] [int] NOT NULL,
	[ParentMatchupId] [int] NULL,
	[TeamCompetingId] [int] NULL,
	[Score] [float] NULL,
 CONSTRAINT [PK_MatchupEntries] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Matchups]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Matchups](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[TournamentId] [int] NOT NULL,
	[WinnerId] [int] NULL,
	[MatchupRound] [int] NOT NULL,
 CONSTRAINT [PK_Matchups] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[People]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[People](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[EmailAddress] [nvarchar](100) NOT NULL,
	[CellphoneNumber] [varchar](20) NULL,
 CONSTRAINT [PK_People] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Prizes]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Prizes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[PlaceNumber] [int] NOT NULL,
	[PlaceName] [nvarchar](50) NOT NULL,
	[PrizeAmount] [money] NOT NULL,
	[PrizePercentage] [float] NOT NULL,
 CONSTRAINT [PK_Prizes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeamMembers]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeamMembers](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[TeamId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
 CONSTRAINT [PK_TeamMembers] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Teams]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Teams](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[TeamName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Teams] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TournamentPrizes]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TournamentPrizes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[TournamentId] [int] NOT NULL,
	[PrizeId] [int] NOT NULL,
 CONSTRAINT [PK_TournamentPrizes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tournaments]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tournaments](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[TournamentName] [nvarchar](150) NOT NULL,
	[EntryFee] [money] NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_Tournaments] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TournamentsEntries]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TournamentsEntries](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[TournamentId] [int] NOT NULL,
	[TeamId] [int] NOT NULL,
 CONSTRAINT [PK_TournamentsEntries] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[MatchupEntries]  WITH CHECK ADD  CONSTRAINT [FK_MatchupEntries_Matchups] FOREIGN KEY([MatchupId])
REFERENCES [dbo].[Matchups] ([id])
GO
ALTER TABLE [dbo].[MatchupEntries] CHECK CONSTRAINT [FK_MatchupEntries_Matchups]
GO
ALTER TABLE [dbo].[MatchupEntries]  WITH CHECK ADD  CONSTRAINT [FK_MatchupEntries_Teams] FOREIGN KEY([TeamCompetingId])
REFERENCES [dbo].[Teams] ([id])
GO
ALTER TABLE [dbo].[MatchupEntries] CHECK CONSTRAINT [FK_MatchupEntries_Teams]
GO
ALTER TABLE [dbo].[Matchups]  WITH CHECK ADD  CONSTRAINT [FK_Matchups_Teams] FOREIGN KEY([WinnerId])
REFERENCES [dbo].[Teams] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Matchups] CHECK CONSTRAINT [FK_Matchups_Teams]
GO
ALTER TABLE [dbo].[TeamMembers]  WITH CHECK ADD  CONSTRAINT [FK_TeamMembers_People] FOREIGN KEY([PersonId])
REFERENCES [dbo].[People] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TeamMembers] CHECK CONSTRAINT [FK_TeamMembers_People]
GO
ALTER TABLE [dbo].[TeamMembers]  WITH CHECK ADD  CONSTRAINT [FK_TeamMembers_Teams] FOREIGN KEY([TeamId])
REFERENCES [dbo].[Teams] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TeamMembers] CHECK CONSTRAINT [FK_TeamMembers_Teams]
GO
ALTER TABLE [dbo].[TournamentPrizes]  WITH CHECK ADD  CONSTRAINT [FK_TournamentPrizes_Prizes] FOREIGN KEY([PrizeId])
REFERENCES [dbo].[Prizes] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TournamentPrizes] CHECK CONSTRAINT [FK_TournamentPrizes_Prizes]
GO
ALTER TABLE [dbo].[TournamentPrizes]  WITH CHECK ADD  CONSTRAINT [FK_TournamentPrizes_Tournaments] FOREIGN KEY([TournamentId])
REFERENCES [dbo].[Tournaments] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TournamentPrizes] CHECK CONSTRAINT [FK_TournamentPrizes_Tournaments]
GO
ALTER TABLE [dbo].[TournamentsEntries]  WITH CHECK ADD  CONSTRAINT [FK_TournamentsEntries_Teams] FOREIGN KEY([TeamId])
REFERENCES [dbo].[Teams] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TournamentsEntries] CHECK CONSTRAINT [FK_TournamentsEntries_Teams]
GO
ALTER TABLE [dbo].[TournamentsEntries]  WITH CHECK ADD  CONSTRAINT [FK_TournamentsEntries_Tournaments] FOREIGN KEY([TournamentId])
REFERENCES [dbo].[Tournaments] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TournamentsEntries] CHECK CONSTRAINT [FK_TournamentsEntries_Tournaments]
GO
/****** Object:  StoredProcedure [dbo].[spMatchupEntries_GetByMatchup]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spMatchupEntries_GetByMatchup]
	@MatchupId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select *
	from MatchupEntries
	where MatchupId=@MatchupId;
END
GO
/****** Object:  StoredProcedure [dbo].[spMatchupEntries_Insert]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spMatchupEntries_Insert] 
	@MatchupId int,
	@ParentMatchupId int,
	@TeamCompetingId int,
	@id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    insert into dbo.MatchupEntries (MatchupId,ParentMatchupId,TeamCompetingId)
	values (@MatchupId,@ParentMatchupId,@TeamCompetingId);

	select @id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[spMatchupEntries_Update]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spMatchupEntries_Update]
	@id int,
	@TeamCompetingId int = null,
	@Score float = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	update dbo.MatchupEntries
	set TeamCompetingId=@TeamCompetingId,Score=@Score
	where id=@id;
END
GO
/****** Object:  StoredProcedure [dbo].[spMatchups_GetByTournament]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spMatchups_GetByTournament] 
	@TournamentId int
AS
BEGIN
	SET NOCOUNT ON

	select m.*
		from dbo.Matchups m
		where m.TournamentId = @TournamentId
		Order by MatchupRound;

END
GO
/****** Object:  StoredProcedure [dbo].[spMatchups_Insert]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spMatchups_Insert] 
	@TournamentId int,
	@MatchupRound int,
	@id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    insert into dbo.Matchups (MatchupRound,TournamentId)
	values (@MatchupRound, @TournamentId);
	select @id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[spMatchups_Update]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spMatchups_Update]
	@id int,
	@WinnerId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	update dbo.Matchups
	set WinnerId=@WinnerId
	where id = @id;
    
END
GO
/****** Object:  StoredProcedure [dbo].[spPeople_GetAll]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spPeople_GetAll] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select *
	from dbo.People

END
GO
/****** Object:  StoredProcedure [dbo].[spPeople_Insert]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spPeople_Insert]
	@FirstName nvarchar(100),
	@LastName nvarchar(100),
	@EmailAddress nvarchar(100),
	@CellphoneNumber varchar(20),
	@id int = 0 output
AS
BEGIN
	SET NOCOUNT ON;

    insert into dbo.People (FirstName, LastName, EmailAddress, CellphoneNumber)
	values (@FirstName, @LastName, @EmailAddress, @CellphoneNumber);

	select @id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[spPrizes_GetByTournament]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spPrizes_GetByTournament] 
	@TournamentId int
AS
BEGIN
	SET NOCOUNT ON

	SELECT p. *
	FROM dbo.Prizes p
	inner join dbo.TournamentPrizes t on p.id = t.PrizeId
	WHERE t.TournamentId = @TournamentId

END
GO
/****** Object:  StoredProcedure [dbo].[spPrizes_Insert]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spPrizes_Insert]
	@PlaceNumber int,
	@PlaceName nvarchar(50),
	@PrizeAmount money,
	@PrizePercentage float,
	@id int = 0 output
AS
BEGIN
	SET NOCOUNT ON;

    insert into dbo.Prizes (PlaceNumber, PlaceName, PrizeAmount, PrizePercentage)
	values (@PlaceNumber, @PlaceName, @PrizeAmount, @PrizePercentage);

	select @id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[spTeam_GetAll]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spTeam_GetAll] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select *
	from dbo.Teams;

END
GO
/****** Object:  StoredProcedure [dbo].[spTeam_GetByTournament]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spTeam_GetByTournament] 
	@TournamentId int
AS
BEGIN
	SET NOCOUNT ON

	SELECT p. *
	FROM dbo.Teams p
	inner join dbo.TournamentsEntries t on p.id = t.TeamId
	WHERE t.TournamentId = @TournamentId

END
GO
/****** Object:  StoredProcedure [dbo].[spTeam_Insert]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spTeam_Insert]
	@TeamName nvarchar(200),
	@id int =0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	insert into dbo.Teams (TeamName)
	values (@TeamName);
	select @id= SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[spTeamMembers_GetByTeam]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spTeamMembers_GetByTeam] 
	@TeamId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	select p.*
	from dbo.TeamMembers m
	inner join dbo.People p on m.PersonId = p.id
	where m.TeamId = @TeamId;

END
GO
/****** Object:  StoredProcedure [dbo].[spTeamMembers_Insert]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spTeamMembers_Insert] 
	@TeamId int,
	@PersonId int,
	@id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	insert into dbo.TeamMembers(TeamId,PersonId)
	values (@TeamId,@PersonId);

	select @id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[spTournamentEntries_Insert]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spTournamentEntries_Insert] 
	@TournamentId int,
	@TeamId int,
	@id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    insert into dbo.TournamentsEntries (TournamentId, TeamId)
	values (@TournamentId, @TeamId);

	select @id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[spTournamentPrizes_Insert]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spTournamentPrizes_Insert] 
	@TournamentId int,
	@PrizeId int,
	@id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	insert into dbo.TournamentPrizes(TournamentId,PrizeId)
	values (@TournamentId,@PrizeId);

	select @id = SCOPE_IDENTITY();
    
END
GO
/****** Object:  StoredProcedure [dbo].[spTournaments_Complete]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spTournaments_Complete] 
	@id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    update dbo.Tournaments
	set Active = 0
	where id = @id;
END
GO
/****** Object:  StoredProcedure [dbo].[spTournaments_GetAll]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
CREATE PROCEDURE [dbo].[spTournaments_GetAll] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	from dbo.Tournaments
	where Active = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[spTournaments_Insert]    Script Date: 11.02.2022 21:03:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spTournaments_Insert]
		@TournamentName nvarchar(200),
		@EntryFee money,
		@id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    insert into dbo.Tournaments (TournamentName, EntryFee, Active)
	values (@TournamentName, @EntryFee, 1);

		select @id = SCOPE_IDENTITY();

END
GO
USE [master]
GO
ALTER DATABASE [Tournaments] SET  READ_WRITE 
GO

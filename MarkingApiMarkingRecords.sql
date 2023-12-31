/****** Script for SelectTopNRows command from SSMS  ******/
SELECT *
  FROM [SampleData].[dbo].[MarkingRecords]

  --insert into MarkingRecords values (
  --'531PAP  03FC', 'S1234567', 'EMGTEST001', '22.7453', 'SG-531|PAP|C 22.7453M|', 'ABCDE', '531PAPC','S1609', '2017-01-25', '13:03:00', '', '1', 'ABCDE', GETDATE(), 22.7453, GETDATE())


/****** Object:  Table [dbo].[MarkingRecords]    Script Date: 25/01/2018 13:06:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MarkingRecords](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[SpecNo] [nvarchar](20) NOT NULL,
	[InaCode] [nvarchar](20) NOT NULL,
	[LotNo] [nvarchar](20) NOT NULL,
	[FreqData] [nvarchar](20) NOT NULL,
	[MarkingData] [nvarchar](50) NOT NULL,
	[WeekCode] [nvarchar](12) NOT NULL,
	[PackageType] [nvarchar](10) NULL,
	[MrkOperator] [nvarchar](20) NULL,
	[MarkingDate] [nvarchar](10) NULL,
	[MarkingTime] [nvarchar](10) NULL,
	[Remark] [nvarchar](50) NULL,
	[MachineNo] [nvarchar](20) NULL,
	[PcsData] [nvarchar](50) NULL,
	[MrkDateTime] [datetime] NOT NULL,
	[FreqValue] [decimal](18, 6) NOT NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_MarkingRecords] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


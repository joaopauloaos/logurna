CREATE TABLE URNAS 
(
	Zona										INT,
	Sessao										INT,
	[Local]										INT,
	Municipio									INT,
	Modelo										VARCHAR(100),
	InicioVotacao								DATETIME,
	FimVotacao									DATETIME,
	Versao										VARCHAR(100),
	Votos										INT,
	QtdLinhas									INT,
	TotalTeclasIndevidas						INT,
	TempoTotalTeclasIndevidas					INT,
	TempoTotalVotacao							INT,
	TempoTotalTotal								INT,
	TempoTotalOcioso							INT,
	TempoTotalHabilitacao						INT,
	TempoTotalVoto1								INT,
	TempoTotalVoto2								INT,
	TempoTotalVoto3								INT,
	TempoTotalVoto4								INT,
	TempoTotalVoto5								INT,
	EleitoresTeclasIndevidas					INT,
	MediaTempoTotal								FLOAT,
	MediaTempoVotacao							FLOAT,
	MediaTempoOcioso							FLOAT,
	MediaTempoHabilitacao						FLOAT,
	MediaTempoVoto1								FLOAT,
	MediaTempoVoto2								FLOAT,
	MediaTempoVoto3								FLOAT,
	MediaTempoVoto4								FLOAT,
	MediaTempoVoto5								FLOAT,
	DesvPadraoTempoTotal						FLOAT,
	DesvPadraoTempoVotacao						FLOAT,
	DesvPadraoTempoOcioso						FLOAT,
	DesvPadraoTempoHabilitacao					FLOAT,
	DesvPadraoTempoVoto1						FLOAT,
	DesvPadraoTempoVoto2						FLOAT,
	DesvPadraoTempoVoto3						FLOAT,
	DesvPadraoTempoVoto4						FLOAT,
	DesvPadraoTempoVoto5						FLOAT,
	MaxRepeticaoClassificacaoTempoTotal			INT,
	MaxRepeticaoClassificacaoTempoVotacao		INT,
	MaxRepeticaoClassificacaoTempoOcioso		INT,
	MaxRepeticaoClassificacaoTempoHabilitacao	INT,
	MaxRepeticaoClassificacaoTempoVoto1			INT,
	MaxRepeticaoClassificacaoTempoVoto2			INT,
	MaxRepeticaoClassificacaoTempoVoto3			INT,
	MaxRepeticaoClassificacaoTempoVoto4			INT,
	MaxRepeticaoClassificacaoTempoVoto5			INT
)
GO

CREATE TABLE VOTOS
(
	Zona							INT,
	Sessao							INT,
	Ordem							INT,
	DataInicioAguardando			DATETIME,
	DataTituloDigitadoMesario		DATETIME,
	DataInicioDigital				DATETIME,
	DataFimDigital					DATETIME,
	DataHabilitacao					DATETIME,
	DataVoto1						DATETIME,
	DataVoto2						DATETIME,
	DataVoto3						DATETIME,
	DataVoto4						DATETIME,
	DataVoto5						DATETIME,
	DataFimVoto						DATETIME,
	SucessoDigital					BIT,
	TentativasDigital				INT,
	TeclasIndevidas					INT,
	TempoTotal						INT,
	TempoVotacao					INT,
	TempoOcioso						INT,
	TempoHabilitacao				INT,
	TempoVoto1						INT,
	TempoVoto2						INT,
	TempoVoto3						INT,
	TempoVoto4						INT,
	TempoVoto5						INT,
	Municipio						INT,
	Local							INT,
	TempoTotalClassificacao			VARCHAR(5),
	TempoVotacaoClassificacao		VARCHAR(5),
	TempoOciosoClassificacao		VARCHAR(5),
	TempoHabilitacaoClassificacao	VARCHAR(5),
	TempoVoto1Classificacao			VARCHAR(5),
	TempoVoto2Classificacao			VARCHAR(5),
	TempoVoto3Classificacao			VARCHAR(5),
	TempoVoto4Classificacao			VARCHAR(5),
	TempoVoto5Classificacao			VARCHAR(5)
)
go

-- TRUNCATE TABLE URNAS
BULK INSERT URNAS FROM 'F:\tse\urna_ba.csv'
WITH
(	
	FORMAT = 'CSV',
	ROWTERMINATOR = '0x0A',
	FIELDTERMINATOR = ';',
	FIELDQUOTE = '"'
)

select zona, Sessao, local, Municipio, MaxRepeticaoClassificacaoTempoTotal, MaxRepeticaoClassificacaoTempoVotacao from URNAS order by MaxRepeticaoClassificacaoTempoTotal desc

select MaxRepeticaoClassificacaoTempoTotal, count(*) as qtd from URNAS group by MaxRepeticaoClassificacaoTempoTotal  order by MaxRepeticaoClassificacaoTempoTotal 

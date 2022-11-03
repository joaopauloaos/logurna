using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogUrna
{
    class CLogUrna
    {
        public String Modelo;
        public String Turno;
        public String Fase;
        public String Versao;
        public String Linha56;
        public DateTime InicioVotacao;
        public DateTime FimVotacao;
        public int Municipio;
        public int Zona;
        public int Sessao;
        public int Local;
        public int QtdLinhas;
        public int Votos;
        public int LinhaSessao;
        public int LinhaSolicitacaoSessao;

        public int TotalTeclasIndevidas;
        public int TempoTotalTeclasIndevidas;
        public int TempoTotalVotacao;
        public int TempoTotalTotal;
        public int TempoTotalOcioso;
        public int TempoTotalHabilitacao;
        public int TempoTotalVoto1;
        public int TempoTotalVoto2;
        public int TempoTotalVoto3;
        public int TempoTotalVoto4;
        public int TempoTotalVoto5;
        public int EleitoresTeclasIndevidas;

        public Double MediaTempoTotal;
        public Double MediaTempoVotacao;
        public Double MediaTempoOcioso;
        public Double MediaTempoHabilitacao;
        public Double MediaTempoVoto1;
        public Double MediaTempoVoto2;
        public Double MediaTempoVoto3;
        public Double MediaTempoVoto4;
        public Double MediaTempoVoto5;

        public Double DesvPadraoTempoTotal;
        public Double DesvPadraoTempoVotacao;
        public Double DesvPadraoTempoOcioso;
        public Double DesvPadraoTempoHabilitacao;
        public Double DesvPadraoTempoVoto1;
        public Double DesvPadraoTempoVoto2;
        public Double DesvPadraoTempoVoto3;
        public Double DesvPadraoTempoVoto4;
        public Double DesvPadraoTempoVoto5;

        public int MaxRepeticaoClassificacaoTempoTotal;
        public int MaxRepeticaoClassificacaoTempoVotacao;
        public int MaxRepeticaoClassificacaoTempoOcioso;
        public int MaxRepeticaoClassificacaoTempoHabilitacao;
        public int MaxRepeticaoClassificacaoTempoVoto1;
        public int MaxRepeticaoClassificacaoTempoVoto2;
        public int MaxRepeticaoClassificacaoTempoVoto3;
        public int MaxRepeticaoClassificacaoTempoVoto4;
        public int MaxRepeticaoClassificacaoTempoVoto5;
    }
}

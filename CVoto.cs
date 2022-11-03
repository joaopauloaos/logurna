using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogUrna
{
    class CVoto
    {
        public DateTime DataInicioAguardando;
        public DateTime DataTituloDigitadoMesario;
        public DateTime DataFimVoto;
        public DateTime DataInicioDigital;
        public DateTime DataFimDigital;
        public Boolean SucessoDigital;
        public int TentativasDigital;
        public int TeclasIndevidas;

        public DateTime DataHabilitacao;
        public DateTime DataVoto1;
        public DateTime DataVoto2;
        public DateTime DataVoto3;
        public DateTime DataVoto4;
        public DateTime DataVoto5;
        
        public int Ordem;
        public int TempoTotalTeclasIndevidas;

        public int TempoVotacao
        {
            get
            {
                return (int) DataFimVoto.Subtract(DataHabilitacao).TotalSeconds;
            }
        }

        public int TempoTotal
        {
            get
            {
                return (int)DataFimVoto.Subtract(DataInicioAguardando).TotalSeconds;
            }
        }

        public int TempoOcioso
        {
            get
            {
                return (int)DataHabilitacao.Subtract(DataInicioAguardando).TotalSeconds;
            }
        }

        public int TempoHabilitacao
        {
            get
            {
                return (int)DataHabilitacao.Subtract(DataTituloDigitadoMesario).TotalSeconds;
            }
        }

        public int TempoVoto1
        {
            get
            {
                return (int)DataVoto1.Subtract(DataHabilitacao).TotalSeconds;
            }
        }

        public int TempoVoto2
        {
            get
            {
                return (int)DataVoto2.Subtract(DataVoto1).TotalSeconds;
            }
        }

        public int TempoVoto3
        {
            get
            {
                return (int)DataVoto3.Subtract(DataVoto2).TotalSeconds;
            }
        }
        public int TempoVoto4
        {
            get
            {
                return (int)DataVoto4.Subtract(DataVoto3).TotalSeconds;
            }
        }

        public int TempoVoto5
        {
            get
            {
                return (int)DataVoto5.Subtract(DataVoto4).TotalSeconds;
            }
        }


        public String TempoTotalClassificacao;
        public String TempoVotacaoClassificacao;        
        public String TempoOciosoClassificacao;
        public String TempoHabilitacaoClassificacao;
        public String TempoVoto1Classificacao;
        public String TempoVoto2Classificacao;
        public String TempoVoto3Classificacao;
        public String TempoVoto4Classificacao;
        public String TempoVoto5Classificacao;        
    }
}

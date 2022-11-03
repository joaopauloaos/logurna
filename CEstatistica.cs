using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogUrna
{
    class CEstatistica
    {
        public CEstatistica()
        {
            Serie = new List<int>();
        }

        public int Total;
        public int Quantidade;
        protected List<int> Serie;

        internal void Adiciona(int novoValor)
        {
            Quantidade++;
            Total += novoValor;
            Serie.Add(novoValor);
        }

        public void Calcula()
        {
            if(Quantidade < 1)
            {
                return;
            }

            Media = ((float) Total) / ((float) Quantidade);

            Double somatoriaVarianca = 0;
            for(int i=0; i< Serie.Count; i++)
            {
                double valor = Serie[i] - Media;
                somatoriaVarianca += Math.Pow(valor, 2);
            }

            double fator = 1.0 / (double) Quantidade;
            Variancia = fator * somatoriaVarianca;
            DesvioPadrao = Math.Sqrt(Variancia);

            Intervalo1 = Media - (DesvioPadrao );
            Intervalo2 = Media - (DesvioPadrao / 2);
            Intervalo3 = Media + (DesvioPadrao / 2);
            Intervalo4 = Media + (DesvioPadrao );
        }

        public String Classifica(int valor)
        {
            String retorno = "n/d";

            if(valor < Intervalo1)
            {
                retorno = "B2";
            } 
            else if((valor > Intervalo1) && (valor < Intervalo2))
            {
                retorno = "B1";
            }
            else if ((valor >= Intervalo2) && (valor < Media))
            {
                retorno = "MB";
            }
            else if ((valor >= Media) && (valor < Intervalo3))
            {
                retorno = "MA";
            }
            else if ((valor >= Intervalo3) && (valor < Intervalo4))
            {
                retorno = "A1";
            }
            else if (valor >= Intervalo4)
            {
                retorno = "A2";
            }

            return retorno;
        }

        public Double Media;
        public Double Variancia;
        public Double DesvioPadrao;

        public Double Intervalo1;
        public Double Intervalo2;
        public Double Intervalo3;
        public Double Intervalo4;

        public String UltimaClassificacao;
        public int QtdMaxClassificacao;

        private int QtdClassAtual = 0;
        public void ContaClassificacoes(String novaClasse)
        {
            if(novaClasse != UltimaClassificacao)
            {
                UltimaClassificacao = novaClasse;
                if(QtdClassAtual > QtdMaxClassificacao)
                {
                    QtdMaxClassificacao = QtdClassAtual;
                }

                QtdClassAtual = 0;
            }

            QtdClassAtual++;
        }
    }
}

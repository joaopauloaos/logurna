using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogUrna
{
    public class CRegistroLog
    {        
        static System.Globalization.CultureInfo ptBR = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
        public DateTime DataEvento;
        public String TipoEvento = "";
        public String Identificacao = "";
        public String Programa = "";
        public String Mensagem = "";
        public String Atenticacao = "";

        public CRegistroLog()
        {

        }

        static public CRegistroLog Parse(String linha)
        {
            if(String.IsNullOrEmpty(linha))
            {
                return null;
            }

            CRegistroLog retorno = new CRegistroLog();
            try
            {
                // 02/10/2022 07:37:05	INFO	67305985	VOTA	Mesário 088679040477 não é eleitor da seção	F82B3C1958BC3FB7
                string[] colunas = linha.Split('\t');
                if(colunas.Length < 1)
                {
                    return retorno;
                }

                DateTime.TryParse(colunas[0], ptBR, System.Globalization.DateTimeStyles.None, out retorno.DataEvento);
                
                retorno.TipoEvento = colunas[1];
                retorno.Identificacao = colunas[2];
                retorno.Programa = colunas[3];
                retorno.Mensagem = colunas[4];
                retorno.Atenticacao = colunas[5];
            }
            catch(Exception ex)
            {

            }
            return retorno;
        }
    }
}

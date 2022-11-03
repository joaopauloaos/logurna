using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogUrna
{
    class Program
    {
        static String Versao = "LogUrna 1.1e";
        static String PathInput = ".\\";
        private static String fileExtension = "*.dat";
        private static String PathOutput = "";
        private static List<String> FilesToProcess;
        private static int MaxTasks = 30;

        static void Parameters(string[] args)
        {
            #region [+ Parametros]

            StringBuilder logParams = new StringBuilder();
            Boolean displayHelp = false;
            Int32 IgnoreCount = 0;
            Int32 pCount = 0;
            Boolean InvalidCommand = false;
            foreach (String p in args)
            {
#if DEBUG
                Console.WriteLine(p);
#endif

                if (IgnoreCount > 0)
                {
                    IgnoreCount--;
                    pCount++;
                    continue;
                }

                String pAux = p;
                String parameter = "";
                if (pAux.Contains("="))
                {
                    int p1 = p.IndexOf('=');
                    pAux = p.Substring(0, p1);
                    parameter = p.Substring(p1 + 1);
                }

                pAux = pAux.ToLower();

                switch (pAux)
                {
                    case "--input-path":
                        IgnoreCount = 0;
                        if (pCount + 1 < args.Length)
                        {
                            PathInput = args[pCount + 1];
                            IgnoreCount++;
                        }
                        break;
                    case "--output-path":
                        IgnoreCount = 0;
                        if (pCount + 1 < args.Length)
                        {
                            PathOutput = args[pCount + 1];
                            IgnoreCount++;
                        }
                        break;
                    case "--max-task":
                    case "--max-tasks":
                        IgnoreCount = 0;
                        if (pCount + 1 < args.Length)
                        {
                            int mTask = Convert.ToInt32(args[pCount + 1]);
                            if(mTask < 1)
                            {
                                InvalidCommand = true;
                            } else
                            {
                                MaxTasks = mTask;
                            }
                            IgnoreCount++;
                        }
                        break;
                    default:
                        if (IgnoreCount > 0)
                        {
                            IgnoreCount--;
                        }
                        else
                        {
                            InvalidCommand = true;
                        }

                        break;
                }

                logParams.AppendLine("");
                logParams.AppendLine("");
                pCount++;
            }

            if (InvalidCommand || displayHelp)
            {
                ShowHelp();
                System.Environment.Exit(0);
                return;
            }

            #endregion
        }

        static void ShowHelp()
        {
            Console.WriteLine(Versao);
            Console.WriteLine("LogUrna <opcoes>");
            Console.WriteLine("OPCOES");
            Console.WriteLine("--input-path\t\tCaminho dos arquivos de entrada");
            Console.WriteLine("");
            Console.WriteLine("--output-path\t\tCaminho onde serao gravados os arquivos com os resultados");
            Console.WriteLine("");
            Console.WriteLine("--max-task\t\tquantidade maxima de taks a serem utilizadas");
            Console.WriteLine("");
            Console.WriteLine("");
        }

        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator = "";
            System.Threading.Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberGroupSeparator = "";

            Parameters(args);

            FilesToProcess = System.IO.Directory.GetFiles(PathInput, fileExtension).ToList();

            if((FilesToProcess.Count == 0) || (String.IsNullOrEmpty(PathOutput)))
            {
                ShowHelp();
                return;
            }

#if DEBUG
            // /*
            // FilesToProcess.Clear();
            // MaxTasks = 1;
            if (FilesToProcess.Count < 1)
            {
                // FilesToProcess.Add(@"C:\Temp\eleicoes2022\logd.dat");
            }

            // FilesToProcess.Clear();
            // FilesToProcess.Add(@"C:\Temp\eleicoes2022\AL_RIO LARGO_0015_0057_logd.dat");
            // */
#endif

            Console.WriteLine(String.Format("{0} - {1} arquivos para processar", Versao, FilesToProcess.Count));

            if(!System.IO.Directory.Exists(PathOutput))
            {
                System.IO.Directory.CreateDirectory(PathOutput);
            }

            int sleepTime = 15;
            int taskCount = 0;
            int taskMax = MaxTasks;
            object lockObject = new object();
            DateTime lastShow = DateTime.Now;
            DateTime startProc = DateTime.Now;
            int totalArquivos = 0;
            int ultimoTotalArquivos = 0;
            int curY = Console.CursorTop;
            while(totalArquivos < FilesToProcess.Count)
            {
                String arquivo = FilesToProcess[totalArquivos];

                Boolean executeNewTask = false;
                lock(lockObject)
                {
                    if(taskCount < taskMax )
                    {
                        taskCount ++;
                        executeNewTask = true;
                    }
                }

                if(executeNewTask)
                {
                    totalArquivos++;

                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                ProcessaArquivo(arquivo);
                            }
                            catch(Exception ex)
                            {

                            }
                            finally
                            {
                                lock (lockObject)
                                {
                                    taskCount --;
                                }
                            }
                        });                   
                } 
                else
                {
                    System.Threading.Thread.Sleep(sleepTime);
                }

                TimeSpan elapsed = DateTime.Now.Subtract(lastShow);
                if (elapsed.TotalSeconds > 1)
                {
                    TimeSpan elapsedTotal = DateTime.Now.Subtract(startProc);

                    int screenSize = System.Console.WindowWidth;
                    int diferencaArquivos = totalArquivos - ultimoTotalArquivos;
                    double arquivosPorSegundo = ((double) diferencaArquivos / elapsed.TotalSeconds);
                    lastShow = DateTime.Now;
                    ultimoTotalArquivos = totalArquivos;
                    double percent = (double)totalArquivos / (double) FilesToProcess.Count;

                    double percentPerSecond = ((percent * 100) / elapsedTotal.Seconds);
                    double etaSeccond = ((1 - percent) * 100) * percentPerSecond ;

                    Console.SetCursorPosition(0, curY);
                    String auxLine = String.Format("{3:N0}s - total={0} - {1:N1} arquivos/s - {2:P2} - ETA {4:N0}s", 
                        totalArquivos, arquivosPorSegundo, percent, 
                        elapsedTotal.TotalSeconds, 
                        etaSeccond).PadRight(screenSize, ' ');
                    
                    Console.WriteLine(auxLine);
                }
            }

            // aguarda tasks terminarem
            while(taskCount > 0)
            {
                System.Threading.Thread.Sleep(sleepTime);
            }

            Console.WriteLine(String.Format("{0} arquivos processados", totalArquivos));
        }

        static String PegaValor(String mensagem, String separador)
        {
            String valor = "";
            int p = mensagem.IndexOf(separador);
            if(p > 0)
            {
                valor = mensagem.Substring(p + 1).Trim();
            }

            return valor;
        }

        static Boolean ProcessaArquivo(String arquivo)
        {
            Boolean resultado = false;
            try
            {
                DateTime dataInicio = DateTime.Now;
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

                System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator = "";
                System.Threading.Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberGroupSeparator = "";

                System.Text.Encoding windows1252 = System.Text.Encoding.GetEncoding(1252);

                String arquivoTextoW = DataLib.UtilStr.ReadFile(arquivo, windows1252);
                String arquivoTexto = "";
                arquivoTexto = arquivoTextoW;
                // arquivoTexto = DataLib.UtilStr.ConvertEnconding(windows1252, System.Text.Encoding.UTF8, arquivoTextoW);
                String[] linhas = arquivoTexto.Split(new char[] {'\r', '\n' });
                int i = 0;
                int contaVotos = 0;

                CLogUrna Urna = new CLogUrna();
                List<CVoto> ListaVotos = new List<CVoto>();
                Boolean votacaoAberta = false;
                CVoto votoAtual = null;
                CRegistroLog ultimoEvento = null;
                CRegistroLog registro = null;
                while (i < linhas.Length)
                {
                    if(String.IsNullOrEmpty(linhas[i]))
                    {
                        i++;
                        continue;
                    }                    
                    
                    ultimoEvento = registro;
                    registro = CRegistroLog.Parse(linhas[i]);
                    if (i == 55)
                    {
                        Urna.Linha56 = registro.Mensagem;
                    }

                    i++;

                    if (registro == null)
                    {
                        continue;
                    }

                    #region [+ Urna]
                    if (registro.Mensagem.StartsWith("Município:"))
                    {
                        Urna.Municipio = int.Parse(PegaValor(registro.Mensagem, ":"));
                        continue;
                    }
                    else if (registro.Mensagem.StartsWith("Zona Eleitoral:"))
                    {
                        Urna.Zona = int.Parse(PegaValor(registro.Mensagem, ":"));
                        continue;
                    }
                    else if (registro.Mensagem.StartsWith("Local de Votação:"))
                    {
                        Urna.Local = int.Parse(PegaValor(registro.Mensagem, ":"));
                        continue;
                    }
                    else if (registro.Mensagem.StartsWith("Seção Eleitoral:"))
                    {
                        Urna.Sessao = int.Parse(PegaValor(registro.Mensagem, ":"));
                        continue;
                    }
                    else if (registro.Mensagem.StartsWith("Turno da UE:"))
                    {
                        Urna.Turno = PegaValor(registro.Mensagem, ":");
                        continue;
                    }
                    else if (registro.Mensagem.StartsWith("Fase da UE:"))
                    {
                        Urna.Fase = PegaValor(registro.Mensagem, ":");
                        continue;
                    }
                    else if (registro.Mensagem.StartsWith("Identificação do Modelo de Urna:"))
                    {
                        Urna.Modelo = PegaValor(registro.Mensagem, ":");
                        continue;
                    }
                    else if (registro.Mensagem.StartsWith("Versão da aplicação:"))
                    {
                        Urna.Versao = PegaValor(registro.Mensagem, ":");
                        continue;
                    }
                    else if (registro.Mensagem.StartsWith("Seção informada pelo operador"))
                    {
                        Urna.LinhaSessao = i;
                        continue;
                    }
                    else if (registro.Mensagem.StartsWith("Solicitação do número da seção"))
                    {
                        Urna.LinhaSolicitacaoSessao = i;
                        continue;
                    }

                    #endregion

                    if (!votacaoAberta)
                    {
                        if (registro.Mensagem.StartsWith("Urna pronta para receber votos"))
                        {
                            votacaoAberta = true;
                            Urna.InicioVotacao = registro.DataEvento;
                            continue;
                        }
                    }
                    else if (registro.Mensagem.StartsWith("Código de encerramento digitado"))
                    {
                        votacaoAberta = false;
                        Urna.FimVotacao = registro.DataEvento;
                    }

                    #region [+ Vota]

                    if (votoAtual == null)
                    {
                        if (registro.Mensagem.StartsWith("Aguardando digitação do título"))
                        {
                            contaVotos++;

                            votoAtual = new CVoto();
                            votoAtual.Ordem = contaVotos;
                            votoAtual.DataInicioAguardando = registro.DataEvento;
                        }
                    }
                    else
                    {
                        if (registro.Mensagem.StartsWith("O voto do eleitor foi computado"))
                        {
                            votoAtual.DataFimVoto = registro.DataEvento;

                            if (votoAtual.TeclasIndevidas > 0)
                            {
                                Urna.TempoTotalTeclasIndevidas += votoAtual.TempoTotalTeclasIndevidas;
                                Urna.TotalTeclasIndevidas += votoAtual.TeclasIndevidas;
                                Urna.EleitoresTeclasIndevidas++;
                            }

                            Urna.TempoTotalVotacao += votoAtual.TempoVotacao;
                            Urna.TempoTotalTotal += votoAtual.TempoTotal;
                            Urna.TempoTotalOcioso += votoAtual.TempoOcioso;
                            Urna.TempoTotalHabilitacao += votoAtual.TempoHabilitacao;
                            Urna.TempoTotalVoto1 += votoAtual.TempoVoto1;
                            Urna.TempoTotalVoto2 += votoAtual.TempoVoto2;
                            Urna.TempoTotalVoto3 += votoAtual.TempoVoto3;
                            Urna.TempoTotalVoto4 += votoAtual.TempoVoto4;
                            Urna.TempoTotalVoto5 += votoAtual.TempoVoto5;

                            ListaVotos.Add(votoAtual);
                            votoAtual = null;
                        }
                        else if (registro.Mensagem.StartsWith("Título digitado pelo mesário"))
                        {
                            votoAtual.DataTituloDigitadoMesario = registro.DataEvento;
                        }
                        else if (registro.Mensagem.StartsWith("Solicita digital. Tentativa [1]"))
                        {
                            votoAtual.DataInicioDigital = registro.DataEvento;
                        }
                        else if (registro.Mensagem.StartsWith("Dedo reconhecido e o score para habilitá-lo"))
                        {
                            votoAtual.DataFimDigital = registro.DataEvento;
                            votoAtual.SucessoDigital = true;
                        }
                        else if (registro.Mensagem.StartsWith("Digital capturada não corresponde a digital do"))
                        {
                            votoAtual.TentativasDigital++;
                        }
                        else if (registro.Mensagem.StartsWith("Número de tentativas de reconhecimento do dedo. Tentativa [4] de [4]"))
                        {
                            votoAtual.DataFimDigital = registro.DataEvento;
                            votoAtual.SucessoDigital = false;
                        }
                        else if (registro.Mensagem.StartsWith("Eleitor foi habilitado"))
                        {
                            votoAtual.DataHabilitacao = registro.DataEvento;
                        }
                        else if (registro.Mensagem.StartsWith("Voto confirmado para [Deputado Federal]"))
                        {
                            votoAtual.DataVoto1 = registro.DataEvento;
                        }
                        else if (registro.Mensagem.StartsWith("Voto confirmado para [Deputado Estadual]"))
                        {
                            votoAtual.DataVoto2 = registro.DataEvento;
                        }
                        else if (registro.Mensagem.StartsWith("Voto confirmado para [Senador]"))
                        {
                            votoAtual.DataVoto3 = registro.DataEvento;
                        }
                        else if (registro.Mensagem.StartsWith("Voto confirmado para [Governador]"))
                        {
                            votoAtual.DataVoto4 = registro.DataEvento;
                        }
                        else if (registro.Mensagem.StartsWith("Voto confirmado para [Presidente]"))
                        {
                            votoAtual.DataVoto5 = registro.DataEvento;
                        }
                        else if (registro.Mensagem.StartsWith("Tecla indevida pressionada"))
                        {
                            votoAtual.TeclasIndevidas++;
                            votoAtual.TempoTotalTeclasIndevidas += (int) registro.DataEvento.Subtract(ultimoEvento.DataEvento).TotalSeconds;
                        }
                    }

                    #endregion                    
                }

                Urna.QtdLinhas = i - 1;
                Urna.Votos = ListaVotos.Count;

                #region [+ Estatistica Voto]

                CEstatistica estatTotalizaTempoTotal = new CEstatistica();
                CEstatistica estatTotalizaTempoVotacao = new CEstatistica();
                CEstatistica estatTotalizaTempoOcioso = new CEstatistica();
                CEstatistica estatTotalizaTempoHabilitacao = new CEstatistica();
                CEstatistica estatTotalizaTempoVoto1 = new CEstatistica();
                CEstatistica estatTotalizaTempoVoto2 = new CEstatistica();
                CEstatistica estatTotalizaTempoVoto3 = new CEstatistica();
                CEstatistica estatTotalizaTempoVoto4 = new CEstatistica();
                CEstatistica estatTotalizaTempoVoto5 = new CEstatistica();

                foreach (CVoto voto in ListaVotos)
                {
                    estatTotalizaTempoTotal.Adiciona(voto.TempoTotal);
                    estatTotalizaTempoVotacao.Adiciona(voto.TempoVotacao);
                    estatTotalizaTempoOcioso.Adiciona(voto.TempoOcioso);
                    estatTotalizaTempoHabilitacao.Adiciona(voto.TempoHabilitacao);
                    estatTotalizaTempoVoto1.Adiciona(voto.TempoVoto1);
                    estatTotalizaTempoVoto2.Adiciona(voto.TempoVoto2);
                    estatTotalizaTempoVoto3.Adiciona(voto.TempoVoto3);
                    estatTotalizaTempoVoto4.Adiciona(voto.TempoVoto4);
                    estatTotalizaTempoVoto5.Adiciona(voto.TempoVoto5);
                }

                estatTotalizaTempoTotal.Calcula();
                estatTotalizaTempoVotacao.Calcula();
                estatTotalizaTempoOcioso.Calcula();
                estatTotalizaTempoHabilitacao.Calcula();
                estatTotalizaTempoVoto1.Calcula();
                estatTotalizaTempoVoto2.Calcula();
                estatTotalizaTempoVoto3.Calcula();
                estatTotalizaTempoVoto4.Calcula();
                estatTotalizaTempoVoto5.Calcula();

                #endregion

                #region [+ Classificacao Voto]

                foreach (CVoto voto in ListaVotos)
                {
                    voto.TempoVotacaoClassificacao = estatTotalizaTempoVotacao.Classifica(voto.TempoVotacao);
                    voto.TempoTotalClassificacao = estatTotalizaTempoTotal.Classifica(voto.TempoTotal);
                    voto.TempoOciosoClassificacao = estatTotalizaTempoOcioso.Classifica(voto.TempoOcioso);
                    voto.TempoHabilitacaoClassificacao = estatTotalizaTempoHabilitacao.Classifica(voto.TempoHabilitacao);

                    voto.TempoVoto1Classificacao = estatTotalizaTempoVoto1.Classifica(voto.TempoVoto1);
                    voto.TempoVoto2Classificacao = estatTotalizaTempoVoto2.Classifica(voto.TempoVoto2);
                    voto.TempoVoto3Classificacao = estatTotalizaTempoVoto3.Classifica(voto.TempoVoto3);
                    voto.TempoVoto4Classificacao = estatTotalizaTempoVoto4.Classifica(voto.TempoVoto4);
                    voto.TempoVoto5Classificacao = estatTotalizaTempoVoto5.Classifica(voto.TempoVoto5);
                }

                #endregion

                #region [+ Conta o maximo de Repeticoes]

                foreach (CVoto voto in ListaVotos)
                {
                    estatTotalizaTempoVotacao.ContaClassificacoes(voto.TempoVotacaoClassificacao);
                    estatTotalizaTempoTotal.ContaClassificacoes(voto.TempoTotalClassificacao);
                    estatTotalizaTempoOcioso.ContaClassificacoes(voto.TempoOciosoClassificacao);
                    estatTotalizaTempoHabilitacao.ContaClassificacoes(voto.TempoHabilitacaoClassificacao);

                    estatTotalizaTempoVoto1.ContaClassificacoes(voto.TempoVoto1Classificacao);
                    estatTotalizaTempoVoto2.ContaClassificacoes(voto.TempoVoto2Classificacao);
                    estatTotalizaTempoVoto3.ContaClassificacoes(voto.TempoVoto3Classificacao);
                    estatTotalizaTempoVoto4.ContaClassificacoes(voto.TempoVoto4Classificacao);
                    estatTotalizaTempoVoto5.ContaClassificacoes(voto.TempoVoto5Classificacao);
                }

                // finaliza
                estatTotalizaTempoVotacao.ContaClassificacoes("");
                estatTotalizaTempoTotal.ContaClassificacoes("");
                estatTotalizaTempoOcioso.ContaClassificacoes("");
                estatTotalizaTempoHabilitacao.ContaClassificacoes("");

                estatTotalizaTempoVoto1.ContaClassificacoes("");
                estatTotalizaTempoVoto2.ContaClassificacoes("");
                estatTotalizaTempoVoto3.ContaClassificacoes("");
                estatTotalizaTempoVoto4.ContaClassificacoes("");
                estatTotalizaTempoVoto5.ContaClassificacoes("");

                #endregion


                // Urna - dados estatisticos
                Urna.MediaTempoTotal = estatTotalizaTempoTotal.Media;
                Urna.MediaTempoHabilitacao = estatTotalizaTempoHabilitacao.Media;
                Urna.MediaTempoVotacao = estatTotalizaTempoVotacao.Media;
                Urna.MediaTempoOcioso = estatTotalizaTempoOcioso.Media;
                Urna.MediaTempoVoto1 = estatTotalizaTempoVoto1.Media;
                Urna.MediaTempoVoto2 = estatTotalizaTempoVoto2.Media;
                Urna.MediaTempoVoto3 = estatTotalizaTempoVoto3.Media;
                Urna.MediaTempoVoto4 = estatTotalizaTempoVoto4.Media;
                Urna.MediaTempoVoto5 = estatTotalizaTempoVoto5.Media;

                Urna.DesvPadraoTempoTotal = estatTotalizaTempoTotal.DesvioPadrao;
                Urna.DesvPadraoTempoHabilitacao = estatTotalizaTempoHabilitacao.DesvioPadrao;
                Urna.DesvPadraoTempoVotacao = estatTotalizaTempoVotacao.DesvioPadrao;
                Urna.DesvPadraoTempoOcioso = estatTotalizaTempoOcioso.DesvioPadrao;
                Urna.DesvPadraoTempoVoto1 = estatTotalizaTempoVoto1.DesvioPadrao;
                Urna.DesvPadraoTempoVoto2 = estatTotalizaTempoVoto2.DesvioPadrao;
                Urna.DesvPadraoTempoVoto3 = estatTotalizaTempoVoto3.DesvioPadrao;
                Urna.DesvPadraoTempoVoto4 = estatTotalizaTempoVoto4.DesvioPadrao;
                Urna.DesvPadraoTempoVoto5 = estatTotalizaTempoVoto5.DesvioPadrao;

                Urna.MaxRepeticaoClassificacaoTempoTotal = estatTotalizaTempoTotal.QtdMaxClassificacao;
                Urna.MaxRepeticaoClassificacaoTempoVotacao = estatTotalizaTempoVotacao.QtdMaxClassificacao;
                Urna.MaxRepeticaoClassificacaoTempoOcioso = estatTotalizaTempoOcioso.QtdMaxClassificacao;
                Urna.MaxRepeticaoClassificacaoTempoHabilitacao = estatTotalizaTempoHabilitacao.QtdMaxClassificacao;
                Urna.MaxRepeticaoClassificacaoTempoVoto1 = estatTotalizaTempoVoto1.QtdMaxClassificacao;
                Urna.MaxRepeticaoClassificacaoTempoVoto2 = estatTotalizaTempoVoto2.QtdMaxClassificacao;
                Urna.MaxRepeticaoClassificacaoTempoVoto3 = estatTotalizaTempoVoto3.QtdMaxClassificacao;
                Urna.MaxRepeticaoClassificacaoTempoVoto4 = estatTotalizaTempoVoto4.QtdMaxClassificacao;
                Urna.MaxRepeticaoClassificacaoTempoVoto5 = estatTotalizaTempoVoto5.QtdMaxClassificacao;


                #region [+ Resultado  Votos]

                StringBuilder sbSaida = new StringBuilder();
                String auxLinha = "";

                sbSaida.AppendLine("Municipio;Zona;Sessao;Local;Voto;InicioAguardando;TituloDigitado;InicioDigital;FimDigital;" +
                                   "Habilitacao;Voto1;Voto2;Voto3;Voto4;Voto5;FimVotacao;SucessoDigital;TentativasDigital;TeclasIndevidas;" +
                                   "TempoTotal;TempoVotacao;TempoOcioso;TempoHabilitacao;TempoVoto1;TempoVoto2;TempoVoto3;TempoVoto4;TempoVoto5;" +
                                   "ClassTempoTotal;ClassTempoVotacao;ClassTempoOcioso;ClassTempoHabilitacao;ClassTempoVoto1;ClassTempoVoto2;ClassTempoVoto3;ClassTempoVoto4;ClassTempoVoto5;");

                foreach (CVoto voto in ListaVotos)
                {
                    auxLinha = String.Format(
                        "{26};{0};{1};{27};{2};{3:yyyy-MM-dd HH:mm:ss};{4:yyyy-MM-dd HH:mm:ss};{5:yyyy-MM-dd HH:mm:ss};{6:yyyy-MM-dd HH:mm:ss};" +
                        "{7:yyyy-MM-dd HH:mm:ss};{8:yyyy-MM-dd HH:mm:ss};{9:yyyy-MM-dd HH:mm:ss};" +
                        "{10:yyyy-MM-dd HH:mm:ss};{11:yyyy-MM-dd HH:mm:ss};{12:yyyy-MM-dd HH:mm:ss};" +
                        "{13:yyyy-MM-dd HH:mm:ss};{14};{15};{16};{17};{18};{19};" +
                        "{20};{21};{22};{23};{24};{25};{28};{29};{30};{31};{32};{33};{34};{35};{36};",
                        Urna.Zona, // 0
                        Urna.Sessao, // 1                    
                        voto.Ordem, // 2 
                        voto.DataInicioAguardando, // 3
                        voto.DataTituloDigitadoMesario, // 4
                        voto.DataInicioDigital, // 5
                        voto.DataFimDigital, // 6
                        voto.DataHabilitacao, // 7
                        voto.DataVoto1, // 8
                        voto.DataVoto2, // 9
                        voto.DataVoto3, // 10
                        voto.DataVoto4, // 11
                        voto.DataVoto5, // 12
                        voto.DataFimVoto, // 13
                        voto.SucessoDigital, // 14
                        voto.TentativasDigital, // 15
                        voto.TeclasIndevidas, // 16
                        voto.TempoTotal, // 17
                        voto.TempoVotacao, // 18
                        voto.TempoOcioso, // 19
                        voto.TempoHabilitacao, // 20
                        voto.TempoVoto1, // 21
                        voto.TempoVoto2, // 22
                        voto.TempoVoto3, // 23
                        voto.TempoVoto4, // 24
                        voto.TempoVoto5, // 25
                        Urna.Municipio, // 26
                        Urna.Local, // 27
                        voto.TempoTotalClassificacao, // 28
                        voto.TempoVotacaoClassificacao, // 29
                        voto.TempoOciosoClassificacao, // 30
                        voto.TempoHabilitacaoClassificacao, // 31
                        voto.TempoVoto1Classificacao, // 32
                        voto.TempoVoto2Classificacao, // 33
                        voto.TempoVoto3Classificacao, // 34
                        voto.TempoVoto4Classificacao, // 35
                        voto.TempoVoto5Classificacao // 36
                    );



                    sbSaida.AppendLine(auxLinha);
                }

                String fileEstats = String.Format("{3}\\votos_{0}_{1}_{2}.csv",
                    Urna.Zona, Urna.Sessao, Urna.Local, PathOutput);
                String textFile = sbSaida.ToString();
                DataLib.UtilStr.WriteFile(fileEstats, textFile, false, System.Text.Encoding.Default);

                #endregion

                #region [+ Resultado Urna]

                sbSaida = new StringBuilder();
                auxLinha = String.Format("{0};{1};{2};{3};{4:yyyy-MM-dd HH:mm:ss};{5:yyyy-MM-dd HH:mm:ss};{6};{7};{8};{9};" +
                                         "{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};" +
                                         "{20};{21};{22:N8};{23:N8};{24:N8};{25:N8};{26:N8};{27:N8};{28:N8};{29:N8};" +
                                         "{30:N8};{31:N8};{32:N8};{33:N8};{34:N8};{35:N8};{36:N8};{37:N8};{38:N8};{39:N8};" +
                                         "{40};{41};{42};{43};{44};{45};{46};{47};{48};{49};{50};{51};",
                    Urna.Zona, // 0
                    Urna.Sessao, // 1
                    Urna.Local, // 2
                    Urna.Municipio, // 3
                    Urna.Modelo, // 4
                    Urna.InicioVotacao, // 5
                    Urna.FimVotacao, // 6
                    Urna.Versao, // 7
                    Urna.Votos, // 8
                    Urna.QtdLinhas, // 9
                    Urna.TotalTeclasIndevidas, // 10
                    Urna.TempoTotalTeclasIndevidas, // 11
                    Urna.TempoTotalVotacao, // 12
                    Urna.TempoTotalTotal, // 13
                    Urna.TempoTotalOcioso, // 14
                    Urna.TempoTotalHabilitacao, // 15
                    Urna.TempoTotalVoto1, // 16
                    Urna.TempoTotalVoto2, // 17
                    Urna.TempoTotalVoto3, // 18
                    Urna.TempoTotalVoto4, // 19
                    Urna.TempoTotalVoto5, // 20
                    Urna.EleitoresTeclasIndevidas, // 21
                    Urna.MediaTempoTotal, // 22
                    Urna.MediaTempoVotacao, // 23
                    Urna.MediaTempoOcioso, // 24
                    Urna.MediaTempoHabilitacao, // 25
                    Urna.MediaTempoVoto1, // 26
                    Urna.MediaTempoVoto2, // 27
                    Urna.MediaTempoVoto3, // 28
                    Urna.MediaTempoVoto4, // 29
                    Urna.MediaTempoVoto5, // 30
                    Urna.DesvPadraoTempoTotal, // 31
                    Urna.DesvPadraoTempoVotacao, // 32
                    Urna.DesvPadraoTempoOcioso, // 33
                    Urna.DesvPadraoTempoHabilitacao, // 34
                    Urna.DesvPadraoTempoVoto1, // 35
                    Urna.DesvPadraoTempoVoto2, // 36
                    Urna.DesvPadraoTempoVoto3, // 37
                    Urna.DesvPadraoTempoVoto4, // 38
                    Urna.DesvPadraoTempoVoto5, // 39
                    Urna.MaxRepeticaoClassificacaoTempoTotal, // 40
                    Urna.MaxRepeticaoClassificacaoTempoVotacao, // 41
                    Urna.MaxRepeticaoClassificacaoTempoOcioso, // 42
                    Urna.MaxRepeticaoClassificacaoTempoHabilitacao, // 43
                    Urna.MaxRepeticaoClassificacaoTempoVoto1, // 44
                    Urna.MaxRepeticaoClassificacaoTempoVoto2, // 45
                    Urna.MaxRepeticaoClassificacaoTempoVoto3, // 46
                    Urna.MaxRepeticaoClassificacaoTempoVoto4, // 47
                    Urna.MaxRepeticaoClassificacaoTempoVoto5, // 48
                    Urna.Linha56,                             // 49  
                    Urna.LinhaSessao,                         // 50  
                    Urna.LinhaSolicitacaoSessao               // 51
                    );                        


                sbSaida.AppendLine(auxLinha);

                fileEstats = String.Format("{3}\\urna_{0}_{1}_{2}.csv",
                    Urna.Zona, Urna.Sessao, Urna.Local, PathOutput);
                textFile = sbSaida.ToString();
                DataLib.UtilStr.WriteFile(fileEstats, textFile, false, System.Text.Encoding.Default);

                #endregion

                DateTime fim = DateTime.Now;
                TimeSpan elapsed = fim.Subtract(dataInicio);

                /*
                Console.WriteLine(String.Format("{0} - {1} linhas em {2}s - {3} votos",
                    fileEstats, Urna.QtdLinhas,
                    elapsed.TotalSeconds,
                    Urna.Votos));
                */
                resultado = true;
            }
            catch (Exception ex)
            {
                resultado = false;
            }

            return resultado;
        }
    }
}

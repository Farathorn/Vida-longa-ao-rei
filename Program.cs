﻿using VLAR.Comum;
using VLAR.Comum.Agentes;

public class JogoPadrao
{
    Tabuleiro Jogo = new Tabuleiro(11, 11, 37);

    Defensor defensor;
    Atacante atacante;


    public JogoPadrao()
    {
        Jogo.InserirPeca(new Mercenario(01, new Posicao(0, 03)));
        Jogo.InserirPeca(new Mercenario(02, new Posicao(0, 04)));
        Jogo.InserirPeca(new Mercenario(03, new Posicao(0, 05)));
        Jogo.InserirPeca(new Mercenario(04, new Posicao(0, 06)));
        Jogo.InserirPeca(new Mercenario(05, new Posicao(0, 07)));
        Jogo.InserirPeca(new Mercenario(06, new Posicao(1, 05)));
        Jogo.InserirPeca(new Mercenario(07, new Posicao(3, 00)));
        Jogo.InserirPeca(new Mercenario(08, new Posicao(3, 10)));
        Jogo.InserirPeca(new Mercenario(09, new Posicao(4, 00)));
        Jogo.InserirPeca(new Mercenario(10, new Posicao(4, 10)));
        Jogo.InserirPeca(new Mercenario(11, new Posicao(5, 00)));
        Jogo.InserirPeca(new Mercenario(12, new Posicao(5, 01)));
        Jogo.InserirPeca(new Mercenario(13, new Posicao(5, 10)));
        Jogo.InserirPeca(new Mercenario(14, new Posicao(5, 9)));
        Jogo.InserirPeca(new Mercenario(15, new Posicao(6, 00)));
        Jogo.InserirPeca(new Mercenario(16, new Posicao(6, 10)));
        Jogo.InserirPeca(new Mercenario(17, new Posicao(7, 00)));
        Jogo.InserirPeca(new Mercenario(18, new Posicao(7, 10)));
        Jogo.InserirPeca(new Mercenario(19, new Posicao(9, 05)));
        Jogo.InserirPeca(new Mercenario(20, new Posicao(10, 3)));
        Jogo.InserirPeca(new Mercenario(21, new Posicao(10, 4)));
        Jogo.InserirPeca(new Mercenario(22, new Posicao(10, 5)));
        Jogo.InserirPeca(new Mercenario(23, new Posicao(10, 6)));
        Jogo.InserirPeca(new Mercenario(24, new Posicao(10, 7)));

        Jogo.InserirPeca(new Soldado(25, new Posicao(3, 5)));
        Jogo.InserirPeca(new Soldado(26, new Posicao(4, 4)));
        Jogo.InserirPeca(new Soldado(27, new Posicao(4, 5)));
        Jogo.InserirPeca(new Soldado(28, new Posicao(4, 6)));
        Jogo.InserirPeca(new Soldado(29, new Posicao(5, 3)));
        Jogo.InserirPeca(new Soldado(30, new Posicao(5, 4)));
        Jogo.InserirPeca(new Soldado(31, new Posicao(5, 6)));
        Jogo.InserirPeca(new Soldado(32, new Posicao(5, 7)));
        Jogo.InserirPeca(new Soldado(33, new Posicao(6, 4)));
        Jogo.InserirPeca(new Soldado(34, new Posicao(6, 5)));
        Jogo.InserirPeca(new Soldado(35, new Posicao(6, 6)));
        Jogo.InserirPeca(new Soldado(36, new Posicao(7, 5)));

        Jogo.InserirPeca(new Rei(37, new Posicao(05, 05)));

        atacante = new Atacante("Player 1");
        defensor = new Defensor("Player 2");
    }

    public void DesenharTabuleiro()
    {
        foreach (List<Casa> linha in Jogo.casas)
        {
            Console.Write(Convert.ToChar(int.Parse(Jogo.casas.IndexOf(linha).ToString()) + 65));
            Console.Write("|");
            foreach (Casa casa in linha)
            {
                if (casa.Ocupante is null
                     && casa.tipo is not Casa.Tipo.Refugio
                     && casa.tipo is not Casa.Tipo.Trono)
                    Console.Write("   |");
                else if (casa.Ocupante is Soldado)
                    Console.Write(" S |");
                else if (casa.Ocupante is Rei)
                    Console.Write(" R |");
                else if (casa.Ocupante is Mercenario)
                    Console.Write(" M |");
                else if (casa.tipo is Casa.Tipo.Refugio)
                    Console.Write(" x |");
                else
                    Console.Write(" - |");
            }
            Console.Write("\n");
        }

        Console.WriteLine("   1   2   3   4   5   6   7   8   9  10  11");
    }

    public void Jogar()
    {
        string? input = "0";
        do
        {
            try
            {
                input = Console.ReadLine();
                if (input is not null && input.StartsWith("m "))
                {
                    input = input.Substring(2);

                    string[] strings = input.Split(" ");

                    var coluna = strings[0][1];
                    var linha = strings[0][0];

                    var colunaDestino = strings[1][1];
                    var linhaDestino = strings[1][0];

                    int xOrigem = (int)coluna - 49;
                    int yOrigem = (int)linha - 97;

                    int xDestino = (int)colunaDestino - 49;
                    int yDestino = (int)linhaDestino - 97;

                    Posicao selecionada = new Posicao(yOrigem, xOrigem);
                    Posicao destino = new Posicao(yDestino, xDestino);

                    Casa? subjacente = Jogo.GetCasa(selecionada);
                    Casa? subjacenteDestino = Jogo.GetCasa(destino);

                    if (subjacente is not null && subjacenteDestino is not null)
                    {
                        Peca? movente = subjacente.Ocupante;

                        if (movente is not null)
                            atacante.Movimentar(movente, Posicao.Sentido(selecionada, destino), (int)!(destino - selecionada));

                    }

                    DesenharTabuleiro();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        while (input != "0");
    }

    public static void Main(string[] Args)
    {
        JogoPadrao jogo = new();
        jogo.DesenharTabuleiro();
        jogo.Jogar();
    }
}
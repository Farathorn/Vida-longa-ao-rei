using VLAR.Comum;

public class JogoPadrao
{
    Tabuleiro Jogo = new Tabuleiro(11, 11, 37);

    public JogoPadrao()
    {
        Jogo.InserirPeca(new Mercenario(Jogo, 01, new Posicao(0, 03)));
        Jogo.InserirPeca(new Mercenario(Jogo, 02, new Posicao(0, 04)));
        Jogo.InserirPeca(new Mercenario(Jogo, 03, new Posicao(0, 05)));
        Jogo.InserirPeca(new Mercenario(Jogo, 04, new Posicao(0, 06)));
        Jogo.InserirPeca(new Mercenario(Jogo, 05, new Posicao(0, 07)));
        Jogo.InserirPeca(new Mercenario(Jogo, 06, new Posicao(1, 05)));
        Jogo.InserirPeca(new Mercenario(Jogo, 07, new Posicao(3, 00)));
        Jogo.InserirPeca(new Mercenario(Jogo, 08, new Posicao(3, 11)));
        Jogo.InserirPeca(new Mercenario(Jogo, 09, new Posicao(4, 00)));
        Jogo.InserirPeca(new Mercenario(Jogo, 10, new Posicao(4, 11)));
        Jogo.InserirPeca(new Mercenario(Jogo, 11, new Posicao(5, 00)));
        Jogo.InserirPeca(new Mercenario(Jogo, 12, new Posicao(5, 01)));
        Jogo.InserirPeca(new Mercenario(Jogo, 13, new Posicao(5, 10)));
        Jogo.InserirPeca(new Mercenario(Jogo, 14, new Posicao(5, 11)));
        Jogo.InserirPeca(new Mercenario(Jogo, 15, new Posicao(6, 00)));
        Jogo.InserirPeca(new Mercenario(Jogo, 16, new Posicao(6, 11)));
        Jogo.InserirPeca(new Mercenario(Jogo, 17, new Posicao(7, 00)));
        Jogo.InserirPeca(new Mercenario(Jogo, 18, new Posicao(7, 11)));
        Jogo.InserirPeca(new Mercenario(Jogo, 19, new Posicao(9, 05)));
        Jogo.InserirPeca(new Mercenario(Jogo, 20, new Posicao(11, 3)));
        Jogo.InserirPeca(new Mercenario(Jogo, 21, new Posicao(11, 4)));
        Jogo.InserirPeca(new Mercenario(Jogo, 22, new Posicao(11, 5)));
        Jogo.InserirPeca(new Mercenario(Jogo, 23, new Posicao(11, 6)));
        Jogo.InserirPeca(new Mercenario(Jogo, 24, new Posicao(11, 7)));

        Jogo.InserirPeca(new Soldado(Jogo, 25, new Posicao(3, 5)));
        Jogo.InserirPeca(new Soldado(Jogo, 26, new Posicao(4, 4)));
        Jogo.InserirPeca(new Soldado(Jogo, 27, new Posicao(4, 5)));
        Jogo.InserirPeca(new Soldado(Jogo, 28, new Posicao(4, 6)));
        Jogo.InserirPeca(new Soldado(Jogo, 29, new Posicao(5, 3)));
        Jogo.InserirPeca(new Soldado(Jogo, 30, new Posicao(5, 4)));
        Jogo.InserirPeca(new Soldado(Jogo, 31, new Posicao(5, 6)));
        Jogo.InserirPeca(new Soldado(Jogo, 32, new Posicao(5, 7)));
        Jogo.InserirPeca(new Soldado(Jogo, 33, new Posicao(6, 4)));
        Jogo.InserirPeca(new Soldado(Jogo, 34, new Posicao(6, 5)));
        Jogo.InserirPeca(new Soldado(Jogo, 35, new Posicao(6, 6)));
        Jogo.InserirPeca(new Soldado(Jogo, 36, new Posicao(7, 5)));

        Jogo.InserirPeca(new Rei(Jogo, 37, new Posicao(05, 05)));

    }

}
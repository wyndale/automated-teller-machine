using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellerMachine;

class Program
{
    static void Main()
    {
        Card card1 = new Card(Card.CardType.Debit);
        Card card2 = new Card(Card.CardType.ATMOnly);
        Card card3 = new Card(Card.CardType.PrepaidDebit);
        Card card4 = new Card(Card.CardType.ContactLessDebit);
        Card card5 = new Card(Card.CardType.InternationalDebit);

        ATM account = new ATM();
        account.AddCustomer("Wendel Lapura", "lapurawendel95@gmail.com", 234523321, 2004, 100000m, card1);

        Console.ReadKey();
    }
}
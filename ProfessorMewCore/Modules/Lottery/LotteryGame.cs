using ProfessorMewData.Interfaces.Guild;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProfessorMewCore.Modules.Lottery
{
    public class LotteryGame
    {
        public List<ILotteryUser> Users { get; private set; }
        public List<string> Prizes { get; private set; }

        public LotteryGame(List<ILotteryUser> users, List<string> prizes)
        {
            if (users is null)
            {
                Users = new List<ILotteryUser>();
            }
            else
            {
                Users = users;
            }
            if (prizes is null)
            {
                Prizes = new List<string>();
            }
            else
            {
                Prizes = prizes;
            }
        }

        public ILotteryUser GetWinner(bool removeWinner = false)
        {
            ILotteryUser winner = null;
            using (var cryptoRNG = new CryptoRNGCore.RandomNumberGenerator())
            {
                double r = cryptoRNG.GetDouble(0, 1) * Users.Sum(x => x.Tickets);
                double min = 0,
                    max = 0;

                foreach (var user in Users)
                {
                    max += user.Tickets;
                    if (min <= r && r < max)
                    {
                        winner = user;
                        break;
                    }
                }
            }

            if (removeWinner && !winner.IsEmpty)
            {
                Console.WriteLine(Users.Remove(winner));
            }

            return winner;
        }

        public List<ILotteryUser> GetWinners(int count, bool removeWinner = false)
        {
            List<ILotteryUser> winners = new List<ILotteryUser>();
            for (int i = 0; i < count; i++)
            {
                winners.Add(GetWinner(removeWinner));
            }

            return winners;
        }
    }
}

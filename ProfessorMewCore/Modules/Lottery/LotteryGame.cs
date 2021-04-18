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
        public bool CanGetWinner
        {
            get
            {
                if (Users.Count == 0 || Prizes.Count == 0) return false;
                return true;
            }
        }

        public LotteryGame(List<ILotteryUser> users, List<string> prizes)
        {
            Users = users is null ? new List<ILotteryUser>() : users;
            Prizes = prizes is null ? new List<string>() : prizes;
        }

        public ILotteryUser GetWinner(bool removeWinner = false)
        {
            if(!CanGetWinner)
            {
                throw new ProfessorMewData.Exceptions.Guild.ProfessorMewException("Cannot get winner");
            }

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
                Users.Remove(winner);
            }

            return winner;
        }

        public List<ILotteryUser> GetWinners(int count, bool removeWinner = false)
        {
            if(!CanGetWinner)
            {
                throw new ProfessorMewData.Exceptions.Guild.ProfessorMewException("Cannot get winners");
            }

            var winners = new List<ILotteryUser>();
            for (int i = 0; i < count; i++)
            {
                winners.Add(GetWinner(removeWinner));
            }

            return winners;
        }
    }
}

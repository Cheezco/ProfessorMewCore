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

        /// <summary>
        /// Randomly gets <see cref="ILotteryUser"/> from <see cref="Users"/> list
        /// </summary>
        /// <param name="removeWinner">If <c>true</c> user will be removed from <c>Users</c> list</param>
        /// <returns>Randomly found <see cref="ILotteryUser"/></returns>
        /// <exception cref="ProfessorMewData.Exceptions.Guild.ProfessorMewException">Thrown when either <see cref="Users"/> or <see cref="Prizes"/> is empty</exception>
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

        /// <summary>
        /// Randomly gets <see cref="ILotteryUser">ILotteryUsers</see> from <see cref="Users"/> list
        /// </summary>
        /// <param name="count">Count of <see cref="ILotteryUser">ILotteryUsers</see> that will be returned</param>
        /// <param name="removeWinner">If <c>true</c> user will be removed from <c>Users</c> list</param>
        /// <returns>List of randomly found <see cref="ILotteryUser">ILotteryUsers</see></returns>
        /// <exception cref="ProfessorMewData.Exceptions.Guild.ProfessorMewException">Thrown when either <see cref="Users"/> or <see cref="Prizes"/> is empty</exception>
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

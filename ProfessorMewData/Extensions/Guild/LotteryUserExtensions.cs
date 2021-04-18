using ProfessorMewData.Interfaces.Guild;
using ProfessorMewData.Models;
using ProfessorMewData.Models.Guild;
using System;

namespace ProfessorMewData.Extensions.Guild
{
    public static class LotteryUserExtensions
    {
        public static void AddTickets(this ILotteryUser user, int ticketAmount)
        {
            if(((long)user.Tickets + ticketAmount) > int.MaxValue)
            {
                user.Tickets = int.MaxValue;
                return;
            }

            user.Tickets += Math.Abs(ticketAmount);
        }

        public static void ReduceTickets(this ILotteryUser user, int ticketAmount)
        {
            if (((long)user.Tickets - ticketAmount) < int.MinValue)
            {
                user.Tickets = int.MinValue;
                return;
            }

            user.Tickets = user.Tickets - Math.Abs(ticketAmount) < 0 ? 0 : user.Tickets - Math.Abs(ticketAmount);
        }
    }
}

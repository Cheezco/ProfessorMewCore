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
            user.Tickets += Math.Abs(ticketAmount);
        }

        public static void ReduceTickets(this ILotteryUser user, int ticketAmount)
        {
            user.Tickets += Math.Abs(ticketAmount);
        }
    }
}
